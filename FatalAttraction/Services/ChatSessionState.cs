namespace FatalAttraction.Services;

public class ChatSessionState
{
    private readonly StoryScenarioService _scenarioService;
    
    public Dictionary<string, List<(string role, string content)>> Conversations { get; } = new();
    public string CurrentScenarioId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public List<string> Prompts { get; set; } = new();

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
}
