// Speech Recognition (Speech-to-Text)
window.speechRecognition = {
    recognition: null,
    isListening: false,

    initialize: function() {
        // Check for browser support
        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        
        if (!SpeechRecognition) {
            console.error('Speech recognition not supported in this browser');
            return false;
        }

        this.recognition = new SpeechRecognition();
        this.recognition.continuous = false; // Stop after one result
        this.recognition.interimResults = false; // Only final results
        this.recognition.lang = 'en-US';
        
        console.log('Speech recognition initialized');
        return true;
    },

    startListening: function(dotNetHelper) {
        if (!this.recognition) {
            if (!this.initialize()) {
                return false;
            }
        }

        if (this.isListening) {
            console.log('Already listening');
            return false;
        }

        console.log('Starting speech recognition...');
        this.isListening = true;

        this.recognition.onresult = (event) => {
            const transcript = event.results[0][0].transcript;
            console.log('Speech recognized:', transcript);
            dotNetHelper.invokeMethodAsync('OnSpeechRecognized', transcript);
        };

        this.recognition.onerror = (event) => {
            console.error('Speech recognition error:', event.error);
            this.isListening = false;
            dotNetHelper.invokeMethodAsync('OnSpeechError', event.error);
        };

        this.recognition.onend = () => {
            console.log('Speech recognition ended');
            this.isListening = false;
            dotNetHelper.invokeMethodAsync('OnSpeechEnded');
        };

        try {
            this.recognition.start();
            return true;
        } catch (error) {
            console.error('Failed to start speech recognition:', error);
            this.isListening = false;
            return false;
        }
    },

    stopListening: function() {
        if (this.recognition && this.isListening) {
            console.log('Stopping speech recognition...');
            this.recognition.stop();
            this.isListening = false;
            return true;
        }
        return false;
    },

    isSupported: function() {
        return !!(window.SpeechRecognition || window.webkitSpeechRecognition);
    }
};
