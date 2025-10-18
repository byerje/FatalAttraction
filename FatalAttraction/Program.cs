using FatalAttraction.Components;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var aiConfig = builder.Configuration.GetSection("AI");
builder.Services.AddKernel();

// Validate AI configuration values to avoid passing possible nulls to the SDK
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

builder.Services.AddAzureOpenAIChatCompletion("gpt-4", aiEndpoint, aiApiKey);
builder.Services.AddScoped<FatalAttraction.Services.ChatSessionState>();

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

app.Run();
