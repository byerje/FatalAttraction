// Text-to-Speech Player
window.ttsPlayer = {
    play: async function(text, voice = 'alloy', speed = 1.0) {
        console.log('TTS Player called with:', { text: text.substring(0, 50) + '...', voice, speed });
        
        try {
            console.log('Fetching from /api/tts...');
            const response = await fetch('/api/tts', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ text, voice, speed })
            });

            console.log('TTS API Response status:', response.status, response.statusText);

            if (!response.ok) {
                const errorText = await response.text();
                console.error('TTS API error:', response.status, response.statusText, errorText);
                throw new Error(`TTS API returned ${response.status}: ${response.statusText}`);
            }

            console.log('Creating audio blob...');
            const blob = await response.blob();
            console.log('Blob size:', blob.size, 'Type:', blob.type);
            
            const audioUrl = URL.createObjectURL(blob);
            const audio = new Audio(audioUrl);
            
            audio.onended = () => {
                console.log('Audio playback ended');
                URL.revokeObjectURL(audioUrl);
            };
            
            audio.onerror = (e) => {
                console.error('Audio playback error:', e);
            };
            
            console.log('Starting audio playback...');
            await audio.play();
            console.log('Audio is playing');
        } catch (error) {
            console.error('TTS playback error:', error);
            throw error; // Re-throw so Blazor can catch it
        }
    }
};
