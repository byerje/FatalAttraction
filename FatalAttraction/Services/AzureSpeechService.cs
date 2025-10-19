using Microsoft.Extensions.Options;
using System.Text;

namespace FatalAttraction.Services;

public class AzureSpeechService
{
    private readonly AzureSpeechOptions _options;
    private readonly HttpClient _httpClient;

    public AzureSpeechService(IOptions<AzureSpeechOptions> options, IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<byte[]> SynthesizeAsync(string text, string? voice = null)
    {
        return await SynthesizeAsync(text, voice ?? "en-US-AvaNeural", 1.0f);
    }

    public async Task<byte[]> SynthesizeAsync(string text, string voice, float speed)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be null or empty", nameof(text));
        }

        try
        {
            var region = string.IsNullOrWhiteSpace(_options.Region) ? "westus3" : _options.Region;
            var voiceName = MapVoiceToAzureVoice(voice);
            
            // For Azure AI Foundry (multi-service) resources
            var endpoint = $"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1";
            
            // Log for debugging (remove in production)
            Console.WriteLine($"TTS Request - Region: {region}, Voice: {voiceName}, Endpoint: {endpoint}");
            Console.WriteLine($"TTS API Key length: {_options.ApiKey.Length} chars, first 10: {_options.ApiKey.Substring(0, Math.Min(10, _options.ApiKey.Length))}...");
            
            // Build SSML
            var ssml = BuildSsml(text, voiceName, speed);
            
            // Create request
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            // Azure AI Foundry multi-service requires both key and region headers
            request.Headers.Add("Ocp-Apim-Subscription-Key", _options.ApiKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", region);
            request.Headers.Add("X-Microsoft-OutputFormat", "audio-16khz-32kbitrate-mono-mp3");
            request.Headers.Add("User-Agent", "FatalAttraction");
            request.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");
            
            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                var requestId = response.Headers.TryGetValues("x-ms-request-id", out var ids) ? ids.FirstOrDefault() : "unknown";
                throw new InvalidOperationException($"Speech synthesis failed: {response.StatusCode}. Error: {error}. Region: {region}. Request ID: {requestId}");
            }
            
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to synthesize speech: {ex.Message}", ex);
        }
    }

    private string MapVoiceToAzureVoice(string voice)
    {
        // Map OpenAI-style voice names to Azure Neural voices
        return voice.ToLower() switch
        {
            "alloy" => "en-US-AndrewNeural",
            "echo" => "en-US-BrianNeural",
            "fable" => "en-US-EmmaNeural",
            "onyx" => "en-US-GuyNeural",
            "nova" => "en-US-AvaNeural",
            "shimmer" => "en-US-JennyNeural",
            _ => voice // If already an Azure voice name, use it as-is
        };
    }

    private string BuildSsml(string text, string voiceName, float speed)
    {
        var prosodyRate = speed switch
        {
            < 0.75f => "x-slow",
            < 0.9f => "slow",
            < 1.1f => "medium",
            < 1.5f => "fast",
            _ => "x-fast"
        };

        return $@"<speak version='1.0' xml:lang='en-US'>
                    <voice name='{voiceName}'>
                        <prosody rate='{prosodyRate}'>
                            {System.Security.SecurityElement.Escape(text)}
                        </prosody>
                    </voice>
                  </speak>";
    }
}
