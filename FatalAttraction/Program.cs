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

// Read from environment variables first, then fall back to configuration
var aiEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") 
    ?? aiConfig["endpoint"];
var aiApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") 
    ?? aiConfig["apikey"];

if (string.IsNullOrWhiteSpace(aiEndpoint))
{
    throw new InvalidOperationException("AI endpoint is not configured. Set the 'AZURE_OPENAI_ENDPOINT' environment variable.");
}
if (string.IsNullOrWhiteSpace(aiApiKey))
{
    throw new InvalidOperationException("AI API key is not configured. Set the 'AZURE_OPENAI_API_KEY' environment variable.");
}

// Add Azure OpenAI Chat Completion
builder.Services.AddAzureOpenAIChatCompletion("gpt-4", aiEndpoint, aiApiKey);
builder.Services.AddScoped<FatalAttraction.Services.ChatSessionState>();

// Configure TTS - Using Azure AI Speech Service (not Azure OpenAI)
var ttsEndpoint = Environment.GetEnvironmentVariable("AZURE_SPEECH_ENDPOINT") 
    ?? aiConfig["ttsEndpoint"];
var ttsApiKey = Environment.GetEnvironmentVariable("AZURE_SPEECH_API_KEY") 
    ?? aiConfig["ttsApiKey"];

// If TTS endpoint is not configured, throw error since Azure AI Speech requires it
if (string.IsNullOrWhiteSpace(ttsEndpoint) || string.IsNullOrWhiteSpace(ttsApiKey))
{
    throw new InvalidOperationException("TTS endpoint and API key must be configured. Set 'AZURE_SPEECH_ENDPOINT' and 'AZURE_SPEECH_API_KEY' environment variables.");
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
