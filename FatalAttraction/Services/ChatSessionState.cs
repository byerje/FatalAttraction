namespace FatalAttraction.Services;

public class ChatSessionState
{
    public Dictionary<string, List<(string role, string content)>> Conversations { get; } = new();
    public string Notes { get; set; } = string.Empty;

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
