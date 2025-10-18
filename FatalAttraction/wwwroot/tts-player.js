// Text-to-Speech Player
window.ttsPlayer = {
    currentAudio: null,
    currentUrl: null,
    
    play: async function(text, voice = 'alloy', speed = 1.0) {
        console.log('TTS Player called with:', { text: text.substring(0, 50) + '...', voice, speed });
        
        // Stop any currently playing audio
        this.stop();
        
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
            
            this.currentUrl = URL.createObjectURL(blob);
            this.currentAudio = new Audio(this.currentUrl);
            
            this.currentAudio.onended = () => {
                console.log('Audio playback ended');
                this.cleanup();
            };
            
            this.currentAudio.onerror = (e) => {
                console.error('Audio playback error:', e);
                this.cleanup();
            };
            
            console.log('Starting audio playback...');
            await this.currentAudio.play();
            console.log('Audio is playing');
        } catch (error) {
            console.error('TTS playback error:', error);
            this.cleanup();
            throw error; // Re-throw so Blazor can catch it
        }
    },
    
    stop: function() {
        if (this.currentAudio) {
            console.log('Stopping current audio playback');
            this.currentAudio.pause();
            this.currentAudio.currentTime = 0;
            this.cleanup();
        }
    },
    
    cleanup: function() {
        if (this.currentUrl) {
            URL.revokeObjectURL(this.currentUrl);
            this.currentUrl = null;
        }
        this.currentAudio = null;
    }
};
