using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GameData
{
    public bool isGameStarted;
    public string gameScene;
    public string gameGuilty;
    public List<string> gameClues;
    public SerializableStoryPhase gameStoryPhase;
    public bool[] gameKnownClues = new bool[3];
    public bool[] gameKnownSuspects = new bool[8];
    public List<StringBoolPair> gameKnownTutorials = new List<StringBoolPair>();
    public List<StringBoolPair> gameKnownDialogues = new List<StringBoolPair>();
    public bool gameIsBadEnding;
    public int gameEndOpportunities;

    public GameData(bool isGameStarted, string sceneName, string guilty, List<string> clues, StoryPhase storyPhase, 
        bool[] knownClues, bool[] knownSuspects, Dictionary<string, bool> knownTutorials, 
        Dictionary<string, bool> knownDialogues, bool isBadEnding, int endOpportunities)
    {
        this.isGameStarted = isGameStarted;
        gameScene = sceneName;
        gameGuilty = guilty;
        gameClues = new List<string>(clues);
        gameStoryPhase = new SerializableStoryPhase(storyPhase);
        gameKnownClues = (bool[])knownClues.Clone();
        gameKnownSuspects = (bool[])knownSuspects.Clone();
        gameIsBadEnding = isBadEnding;
        gameEndOpportunities = endOpportunities;

        gameKnownTutorials = knownTutorials
            .Select(pair => new StringBoolPair { key = pair.Key, value = pair.Value })
            .ToList();

        gameKnownDialogues = knownDialogues
            .Select(pair => new StringBoolPair { key = pair.Key, value = pair.Value })
            .ToList();
    }

    public GameData()
    {
        isGameStarted = false;
        gameScene = "HomeScene";
        gameGuilty = "";
        gameClues = new List<string> { "", "", "" };
        gameStoryPhase = new SerializableStoryPhase(StoryStateManager.CreateFirstPhase());
        gameIsBadEnding = false;
        gameEndOpportunities = 2;
    }

    public Dictionary<string, bool> GetKnownTutorials()
    {
        return gameKnownTutorials.ToDictionary(pair => pair.key, pair => pair.value);
    }

    public Dictionary<string, bool> GetKnownDialogues()
    {
        return gameKnownDialogues.ToDictionary(pair => pair.key, pair => pair.value);
    }
}

[System.Serializable]
public class StringBoolPair
{
    public string key;
    public bool value;
}

[System.Serializable]
public class SerializableStoryPhase
{
    public StoryPhaseOption phaseName;
    public List<SerializableStorySubphase> storySubphases;
    public int currentSubphaseIndex;
    public List<string> subphaseObjectNames;

    public SerializableStoryPhase(StoryPhase phase)
    {
        phaseName = phase.phaseName;
        storySubphases = phase.storySubphases
            .Select(s => new SerializableStorySubphase(s))
            .ToList();

        currentSubphaseIndex = phase.storySubphases.IndexOf(phase.currentSubphase);
        subphaseObjectNames = new List<string>(phase.subphaseObjectNames);
    }

    public StoryPhase ToStoryPhase()
    {
        var realSubphases = storySubphases.Select(s => s.ToStorySubphase()).ToList();

        var newPhase = new StoryPhase(phaseName, realSubphases)
        {
            subphaseObjectNames = new List<string>(subphaseObjectNames)
        };

        if (currentSubphaseIndex == 0)
        newPhase.currentSubphase = newPhase.storySubphases[0];
        else
        newPhase.currentSubphase = newPhase.storySubphases[currentSubphaseIndex];

        return newPhase;
    }
}

[System.Serializable]
public class SerializableStorySubphase
{
    public StorySubphaseType subphaseType;
    public string subphaseName;
    public int actions;

    public SerializableStorySubphase(StorySubphase subphase)
    {
        subphaseType = subphase.subphaseType;
        subphaseName = subphase.subphaseName;
        actions = subphase.actions;
    }

    public StorySubphase ToStorySubphase()
    {
        return new StorySubphase(subphaseType, subphaseName, actions);
    }
}