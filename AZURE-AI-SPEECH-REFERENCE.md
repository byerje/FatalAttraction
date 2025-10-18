# Azure AI Speech Service - Voice Reference

## ✅ Successfully Switched to Azure AI Speech Service!

Your app now uses **Azure AI Speech Service** instead of Azure OpenAI for text-to-speech.

## Configuration

Current settings in `appsettings.Development.json`:
```json
"AI": {
  "ttsEndpoint": "https://byerj-mgwqvsgi-westus3.services.ai.azure.com/models",
  "ttsApiKey": "YOUR_KEY",
  "ttsDeployment": "N/A"  // Not used for Azure AI Speech
}
```

## Voice Mappings

The app automatically maps OpenAI-style voice names to Azure Neural voices:

| Your Code | Azure Voice | Description |
|-----------|-------------|-------------|
| `alloy` | en-US-AndrewNeural | Male, neutral and balanced |
| `echo` | en-US-BrianNeural | Male, clear and expressive |
| `fable` | en-US-EmmaNeural | Female, warm and engaging |
| `onyx` | en-US-GuyNeural | Male, deep and authoritative |
| `nova` | en-US-AvaNeural | Female, energetic and lively |
| `shimmer` | en-US-JennyNeural | Female, soft and gentle |

## How It Works

1. **Your Razor page calls**: `await JS.InvokeVoidAsync("ttsPlayer.play", text, "nova", 1.0f);`
2. **JavaScript sends to**: `/api/tts` endpoint
3. **API calls**: Azure AI Speech Service
4. **Service maps**: "nova" → "en-US-AvaNeural"
5. **Returns**: MP3 audio data
6. **Browser plays**: Audio via HTML5 Audio element

## Speed Control

Speed parameter is mapped to Azure prosody rates:
- `< 0.75` → "x-slow"
- `< 0.9` → "slow"  
- `< 1.1` → "medium"
- `< 1.5` → "fast"
- `>= 1.5` → "x-fast"

## Next Steps

1. **RESTART your app** (Stop and Start - Program.cs changed)
2. **Refresh browser**
3. **Test Bakery page** - ask a question
4. **Audio should now play!** 🎉

## Using Azure Voice Names Directly

You can also use Azure voice names directly if you want more options:

```csharp
await JS.InvokeVoidAsync("ttsPlayer.play", text, "en-US-AriaNeural", 1.0f);
```

See full list of voices: https://learn.microsoft.com/en-us/azure/ai-services/speech-service/language-support?tabs=tts

## Troubleshooting

If you still get errors:
- Check that `ttsApiKey` is correct (should be a long string, not a GUID)
- Verify the region in the endpoint matches your Azure resource
- Make sure your subscription has Speech Service enabled
