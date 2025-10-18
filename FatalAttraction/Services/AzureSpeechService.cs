using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;

namespace FatalAttraction.Services;

public class AzureSpeechService
{
    private readonly AzureSpeechOptions _options;

    public AzureSpeechService(IOptions<AzureSpeechOptions> options)
    {
        _options = options.Value;
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
            // Extract region from endpoint URL
            var region = ExtractRegionFromEndpoint(_options.Endpoint);
            
            // Configure speech SDK
            var config = SpeechConfig.FromSubscription(_options.ApiKey, region);
            config.SpeechSynthesisVoiceName = MapVoiceToAzureVoice(voice);
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3);

            using var synthesizer = new SpeechSynthesizer(config, null);
            
            // Build SSML with speed control
            var ssml = BuildSsml(text, config.SpeechSynthesisVoiceName, speed);
            
            var result = await synthesizer.SpeakSsmlAsync(ssml);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                return result.AudioData;
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                throw new InvalidOperationException($"Speech synthesis canceled: {cancellation.Reason}. Error: {cancellation.ErrorDetails}");
            }
            else
            {
                throw new InvalidOperationException($"Speech synthesis failed with reason: {result.Reason}");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to synthesize speech: {ex.Message}", ex);
        }
    }

    private string ExtractRegionFromEndpoint(string endpoint)
    {
        // Extract region from endpoint like: https://byerj-mgwqvsgi-westus3.services.ai.azure.com
        var uri = new Uri(endpoint);
        var host = uri.Host; // byerj-mgwqvsgi-westus3.services.ai.azure.com
        var parts = host.Split('.');
        if (parts.Length > 0)
        {
            var firstPart = parts[0]; // byerj-mgwqvsgi-westus3
            var regionParts = firstPart.Split('-');
            if (regionParts.Length > 0)
            {
                return regionParts[^1]; // westus3
            }
        }
        return "westus3"; // Default fallback
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
