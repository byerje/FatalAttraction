# Test Azure OpenAI TTS Deployments
# This script will help you find the correct deployment name

$endpoint = "https://byerj-mgwez75d-eastus2.openai.azure.com"
$apiKey = "CMFOnaQUIQDzniCAjoA4qPWWmvqyVr3VgggQY5rNgY7LadTxnmQuJQQJ99BJACHYHv6XJ3w3AAAAACOGyB1t"
$apiVersion = "2024-08-01-preview"

Write-Host "Testing Azure OpenAI TTS Deployments..." -ForegroundColor Cyan
Write-Host ""

# List of common TTS deployment names to try
$deploymentNames = @(
    "tts",
    "tts-1",
    "tts-hd",
    "tts-1-hd",
    "gpt-4o-realtime-preview",
    "gpt-4o-audio-preview"
)

foreach ($deployment in $deploymentNames) {
    Write-Host "Trying deployment: $deployment" -ForegroundColor Yellow
    
    $url = "$endpoint/openai/deployments/$deployment/audio/speech?api-version=$apiVersion"
    
    $body = @{
        model = $deployment
        input = "Testing"
        voice = "alloy"
    } | ConvertTo-Json
    
    try {
        $response = Invoke-WebRequest -Uri $url -Method POST `
            -Headers @{
                "api-key" = $apiKey
                "Content-Type" = "application/json"
            } `
            -Body $body `
            -ErrorAction Stop
        
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ SUCCESS! Deployment '$deployment' works!" -ForegroundColor Green
            Write-Host "   Use this value in your appsettings: `"ttsDeployment`": `"$deployment`"" -ForegroundColor Green
            Write-Host ""
            break
        }
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 404) {
            Write-Host "   ❌ 404 - Deployment not found" -ForegroundColor Red
        }
        elseif ($statusCode -eq 401) {
            Write-Host "   ❌ 401 - Authentication failed (check API key)" -ForegroundColor Red
        }
        else {
            Write-Host "   ❌ Error $statusCode - $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    Write-Host ""
}

Write-Host ""
Write-Host "If none worked, check Azure Portal:" -ForegroundColor Cyan
Write-Host "1. Go to https://oai.azure.com/" -ForegroundColor White
Write-Host "2. Select your resource" -ForegroundColor White
Write-Host "3. Go to Deployments tab" -ForegroundColor White
Write-Host "4. Look for TTS or Audio models" -ForegroundColor White
