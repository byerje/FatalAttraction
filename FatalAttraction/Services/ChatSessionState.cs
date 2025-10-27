namespace FatalAttraction.Services;

public class ChatSessionState
{
    private readonly StoryScenarioService _scenarioService;
    
    public Dictionary<string, List<(string role, string content)>> Conversations { get; } = new();
    public string CurrentScenarioId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public List<string> Prompts { get; set; } = new();
    public bool IsCaseSolved { get; set; } = false;

    public ChatSessionState(StoryScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
        CurrentScenarioId = _scenarioService.GetDefaultScenarioId();
        LoadScenario(CurrentScenarioId);
    }

    public void LoadScenario(string scenarioId)
    {
        var scenario = _scenarioService.GetScenario(scenarioId);
        CurrentScenarioId = scenarioId;
        
        // Clear existing data
        Conversations.Clear();
        Prompts.Clear();
        IsCaseSolved = false; // Reset case solved flag when loading new scenario
        
        // Load scenario-specific data
        Notes = $@"Victim: 
{scenario.VictimName}

Cause of Death:
{scenario.CauseOfDeath}

Time of Death:
{scenario.TimeOfDeath}

Last seen:
{scenario.LastSeen}
";
        
        // Load shared prompts
        Prompts.AddRange(scenario.SharedPrompts);
    }
    
    public string GetCharacterPrompt(string characterKey)
    {
        var scenario = _scenarioService.GetScenario(CurrentScenarioId);
        if (scenario.CharacterPrompts.TryGetValue(characterKey, out var prompt))
        {
            // If case is solved, append the refusal instruction
            if (IsCaseSolved)
            {
                prompt += "\n\nIMPORTANT: The case has been SOLVED and CLOSED. You will NOT discuss this case anymore. When the player asks you anything about the murder or investigation, firmly but politely refuse and say something like: 'That's old news now. The case is closed. Why don't you head back home and pick a different mystery to solve?' or 'That's ancient history. The killer has been caught. Go home and select a new scenario.' Keep your response SHORT and redirect them to choose a new mystery.";
            }
            return prompt;
        }
        
        return $"You are a resident of Larkspur Hollow. You don't know much about the murder.";
    }
    
    public StoryScenario GetCurrentScenario()
    {
        return _scenarioService.GetScenario(CurrentScenarioId);
    }

    public List<(string role, string content)> GetOrCreateConvo(string character)
    {
        if (!Conversations.TryGetValue(character, out var convo))
        {
            convo = new List<(string role, string content)>();
            Conversations[character] = convo;
        }
        return convo;
    }
    
    public void CheckIfCaseSolved(string response)
    {
        // Check if the response contains case-solved language
        var solvedPhrases = new[] { 
            "case closed", "case solved", "mystery solved", 
            "investigation complete", "case is solved", "case is closed",
            "mystery is solved", "you've solved it", "you solved it"
        };
        
        var lowerResponse = response.ToLower();
        if (solvedPhrases.Any(phrase => lowerResponse.Contains(phrase)))
        {
            IsCaseSolved = true;
        }
    }
}
