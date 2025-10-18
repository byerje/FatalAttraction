namespace FatalAttraction.Services;

public class AzureSpeechOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty; // Azure region for Speech Service
}
