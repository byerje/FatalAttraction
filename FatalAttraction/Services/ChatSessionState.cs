namespace FatalAttraction.Services;

public class ChatSessionState
{
    public Dictionary<string, List<(string role, string content)>> Conversations { get; } = new();
    public string Notes { get; set; } =  @"Victim: 
Evelyn Cross

Cause of Death:
Blunt trauma to the head

Time of Death:
Between 9 PM and midnight

Last seen:
Leaving the Theater after rehearsal
";

    public List<string> Prompts { get; set; } = new();

    public ChatSessionState()
    {
        Prompts.Add("In the quiet town of Larkspur Hollow, an attractive little town, the body of Evelyn Cross, a beloved actress who once performed at the local theater, is found dead in the park at dawn. The townsfolk whisper it was a crime of passion — but everyone seems to have been attracted to Evelyn in some way, and everyone’s hiding something. The player must interview each character to uncover motives, lies, and secrets — and ultimately identify the killer. ");
        Prompts.Add("Cause of Death: Blunt trauma to the head.");
        Prompts.Add("Time of Death: Between 9 PM and midnight. ");
        Prompts.Add("Last seen: Leaving the Theater after rehearsal. ");
        Prompts.Add("Theme: Each suspect’s “attraction” (romantic, financial, power, or admiration) drove their behavior.");
        Prompts.Add("The key people and places in town are:\n\n" +
            "• Martin Harlow – Banker: Professional but nervous, secretly altered a loan for Evelyn and feared exposure.\n" +
            "• Mildred Gray – Librarian: Elderly and gossipy, knows everyone’s business and hints of affairs.\n" +
            "• Clara Finch – Baker: Kind but anxious, overheard arguments between Evelyn and the blacksmith.\n" +
            "• Edwin Pike – Postal Worker: Cynical and nosy, intercepted a love letter from Evelyn to the mayor.\n" +
            "• Lucas Vale – Innkeeper: Charming but conflicted, was having an affair with Evelyn and feared scandal.\n" +
            "• Hugo Brandt – Blacksmith: Gruff and proud, owed Evelyn money and argued with her before her death.\n" +
            "• Nina Holst – Market Clerk: Observant and chatty, recalls Evelyn buying wine and saying, 'Tonight, it all ends.'\n" +
            "• Ben Langford – Mayor: Authoritative and composed, secretly fears exposure of her affair with Evelyn.\n" +
            "• Father Thomas Alder – Preacher: Gentle and cryptic, knows confessions related to the murder but speaks in riddles.\n" +
            "• Hazel Dean – Theater Ticket Taker: Shy admirer of Evelyn, saw her leave the theater and a man in a dark coat nearby.\n" +
            "• Officer Grant Miles – Policeman: Fair and logical, leads the investigation and knows more than he lets on.\n" +
            "• Lydia Rowe – Teacher: Compassionate and insightful, discovered Evelyn planned to leave town that night.\n\n" +
            "Each resident has their own suspicions and observations. Some protect secrets, others seek truth — but only one of them is the killer.");
        Prompts.Add("Don't be long-winded in your response. Focus on the character's motives and motivations. Don't be afraid to ask follow-up questions. Remember, the player is trying to uncover the killer's motives and motivations.");
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
