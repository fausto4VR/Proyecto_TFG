using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public string gameScene;
    public string gameGuilty;
    public List<string> gameClues;
    public int gameStoryPhaseAux;
    public StoryPhase gameStoryPhase;
    public bool[] gameKnownSuspects = new bool[8];
    public bool[] gameKnownTutorials = new bool[8];
    public bool[] gameKnownDialogues = new bool[8];
    public bool gameIsBadEnding;
    public int gameEndOpportunities;

    public GameData(string sceneName, string guilty, List<string> clues, int storyPhaseAux, StoryPhase storyPhase,
         bool[] knownSuspects, bool[] knownTutorials, bool[] knownDialogues, bool isBadEnding, int endOpportunities)
    {
        gameScene = sceneName;
        gameGuilty = guilty;
        gameClues = new List<string>(clues);
        gameStoryPhaseAux = storyPhaseAux;
        gameStoryPhase = storyPhase;
        gameKnownSuspects = (bool[])knownSuspects.Clone();
        gameKnownTutorials = (bool[])knownTutorials.Clone();
        gameKnownDialogues = (bool[])knownDialogues.Clone();        
        gameIsBadEnding = isBadEnding;
        gameEndOpportunities = endOpportunities;
    }

    public GameData()
    {
        gameScene = "SampleScene";
        gameGuilty = "";
        gameClues = new List<string> { "", "", "" }; 
        gameStoryPhaseAux = 0;
        gameStoryPhase = StoryStateManager.CreateFirstPhase();
        gameIsBadEnding = false;
        gameEndOpportunities = 2;
        gameKnownDialogues[0] = true;
    }
}