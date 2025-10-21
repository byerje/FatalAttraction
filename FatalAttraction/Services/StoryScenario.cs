namespace FatalAttraction.Services;

public class StoryScenario
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string VictimName { get; set; } = string.Empty;
    public string CauseOfDeath { get; set; } = string.Empty;
    public string TimeOfDeath { get; set; } = string.Empty;
    public string LastSeen { get; set; } = string.Empty;
    public string IntroText { get; set; } = string.Empty;
    public List<string> SharedPrompts { get; set; } = new();
    public Dictionary<string, string> CharacterPrompts { get; set; } = new();
    public string MurdererCharacterKey { get; set; } = string.Empty;
    public string SolutionConfirmation { get; set; } = string.Empty;
}
