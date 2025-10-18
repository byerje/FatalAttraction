using FatalAttraction.Components;
using FatalAttraction.Services;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure AI services
var aiConfig = builder.Configuration.GetSection("AI");

builder.Services.AddKernel();

// Validate AI configuration values for Chat Completion
var aiEndpoint = aiConfig["endpoint"];
var aiApiKey = aiConfig["apikey"];

if (string.IsNullOrWhiteSpace(aiEndpoint))
{
    throw new InvalidOperationException("AI endpoint is not configured. Please set the 'AI:endpoint' value in appsettings.json or environment variables.");
}
if (string.IsNullOrWhiteSpace(aiApiKey))
{
    throw new InvalidOperationException("AI apikey is not configured. Please set the 'AI:apikey' value in appsettings.json or environment variables.");
}

// Add Azure OpenAI Chat Completion
builder.Services.AddAzureOpenAIChatCompletion("gpt-4", aiEndpoint, aiApiKey);

// Configure TTS - Using Azure AI Speech Service (not Azure OpenAI)
var ttsEndpoint = aiConfig["ttsEndpoint"];
var ttsApiKey = aiConfig["ttsApiKey"];

// If TTS endpoint is not configured, throw error since Azure AI Speech requires it
if (string.IsNullOrWhiteSpace(ttsEndpoint) || string.IsNullOrWhiteSpace(ttsApiKey))
{
    throw new InvalidOperationException("TTS endpoint and API key must be configured for Azure AI Speech Service. Set 'AI:ttsEndpoint' and 'AI:ttsApiKey' in appsettings.json.");
}

// Configure TTS options for Azure AI Speech Service
builder.Services.Configure<AzureSpeechOptions>(options =>
{
    options.Endpoint = ttsEndpoint!;
    options.ApiKey = ttsApiKey!;
    options.Region = string.Empty; // Extracted from endpoint
});

// Register TTS service
builder.Services.AddSingleton<AzureSpeechService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// TTS API endpoint
app.MapPost("/api/tts", async (TtsRequest request, AzureSpeechService ttsService) =>
{
    try
    {
        var voice = request.Voice ?? "alloy";
        var audioBytes = await ttsService.SynthesizeAsync(request.Text, voice, request.Speed);
        return Results.File(audioBytes, "audio/mpeg");
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});

app.Run();

// TTS Request model
record TtsRequest(string Text, string? Voice = null, float Speed = 1.0f);
