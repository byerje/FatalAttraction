# Multi-Scenario Murder Mystery Implementation

## Summary
Successfully implemented a flexible scenario system that allows players to experience different murder mysteries in Larkspur Hollow with the same characters but different murderers, clues, and storylines.

## Files Created
1. **Services/StoryScenario.cs** - Model class defining scenario properties
2. **Services/StoryScenarioService.cs** - Service managing multiple scenarios with two complete mysteries

## Files Modified
1. **Services/ChatSessionState.cs** - Refactored to use scenario service, supports scenario switching
2. **Program.cs** - Registered StoryScenarioService as singleton
3. **Components/Pages/Home.razor** - Added scenario selector dropdown, dynamic police report
4. **All 12 Character Pages** - Updated to use dynamic prompts from ChatState:
   - Bakery.razor
   - Bank.razor
   - Blacksmith.razor
   - Church.razor
   - Inn.razor
   - Library.razor
   - Market.razor
   - Park.razor
   - PostOffice.razor
   - School.razor
   - Theater.razor
   - TownHall.razor

## Scenarios Implemented

### Scenario 1: The Banker's Fatal Attraction (Original)
- **Murderer:** Martin Harlow (Banker)
- **Victim:** Evelyn Cross
- **Method:** Blunt trauma to the head with a wrench
- **Motive:** Crime of passion - secret attraction and fear of exposure
- **Time:** Between 9 PM and midnight
- **Key Clues:**
  - Theater worker saw banker in dark coat
  - Wrench fingerprints match banker
  - Various affairs and financial improprieties

### Scenario 2: The Mayor's Dark Secret (New)
- **Murderer:** Ben Langford (Mayor)
- **Victim:** Evelyn Cross
- **Method:** Arsenic poisoning in wine
- **Motive:** Political corruption - Evelyn discovered embezzlement
- **Time:** Between 10 PM and 1 AM
- **Key Clues:**
  - Arsenic from mayor's office
  - Wine glass with mayor's fingerprints
  - Evelyn's notes on corruption
  - Letters to journalist about exposing scandal

## How It Works

### Scenario Selection
- Players select a mystery from dropdown on Home page
- Selection triggers `ChatState.LoadScenario(scenarioId)`
- All conversations reset when switching scenarios
- Police report details update dynamically

### Dynamic Character Prompts
Each character page now calls:
```csharp
var characterPrompt = ChatState.GetCharacterPrompt("baker");
history.AddSystemMessage(characterPrompt);
```

Instead of hardcoded prompts, ensuring characters respond appropriately to the active scenario.

### Architecture Benefits
1. **Maintainable:** Easy to add new scenarios without touching character pages
2. **Flexible:** Each scenario can have completely different story, clues, and murderer
3. **Scalable:** Add unlimited scenarios by extending StoryScenarioService
4. **Clean:** Separation of concerns - scenario data separate from UI logic

## Adding New Scenarios

To add a new scenario:

1. In `StoryScenarioService.LoadScenarios()`, create new StoryScenario object:
```csharp
var newScenario = new StoryScenario
{
    Id = "unique-id",
    Title = "Display Name",
    VictimName = "Victim Name",
    CauseOfDeath = "How they died",
    TimeOfDeath = "When",
    LastSeen = "Where last seen",
    IntroText = "Story introduction...",
    MurdererCharacterKey = "character-key",
    SolutionConfirmation = "What officer says when solved"
};
```

2. Add shared prompts:
```csharp
newScenario.SharedPrompts.Add("Background information...");
```

3. Add character-specific prompts for all 12 characters:
```csharp
newScenario.CharacterPrompts["baker"] = "Character behavior...";
newScenario.CharacterPrompts["banker"] = "Character behavior...";
// ... all 12 characters
```

4. Register the scenario:
```csharp
_scenarios[newScenario.Id] = newScenario;
```

## Testing
- Build: ✅ Success (0 warnings, 0 errors)
- All character pages updated: ✅ 12/12
- Scenario switching: ✅ Implemented
- Dynamic police report: ✅ Working

## Next Steps (Optional)
1. Add more scenarios (suggested: Blacksmith debt revenge, Innkeeper jealousy)
2. Persist selected scenario to browser storage
3. Add "New Game" button to explicitly reset progress
4. Create JSON-based scenario loading for easier content management
5. Add difficulty levels or hint systems per scenario

## Branch Information
- Branch: `feature/multi-scenario-support`
- Commit: ce9f261
- Status: Ready for testing and merge
