[System.Serializable]
public class GameData
{
    public string gameScene = "";
    public string gameGuilty = "";
    public string gameFirstClue = "";
    public string gameSecondClue = "";
    public string gameThirdClue = "";
    public int gameStoryPhase = 0;
    public string gameLastPuzzleComplete = "";
    public bool[] gameKnownSuspects = new bool[8];
    public bool[] gameKnownTutorials = new bool[8];
    public bool[] gameKnownDialogues = new bool[8];
    public bool gameIsBadEnding = false;
    public int gameEndOpportunities = 2;

    public GameData(string sceneName, string guilty, string firstClue, string secondClue, string thirdClue, int storyPhase, 
        string lastPuzzleComplete, bool[] knownSuspects, bool[] knownTutorials, bool[] knownDialogues, bool isBadEnding, 
        int endOpportunities)
    {
        gameScene = sceneName;
        gameGuilty = guilty;
        gameFirstClue = firstClue;
        gameSecondClue = secondClue;
        gameThirdClue = thirdClue;
        gameStoryPhase = storyPhase;
        gameLastPuzzleComplete = lastPuzzleComplete;
        gameKnownSuspects = knownSuspects;
        gameKnownTutorials = knownTutorials;
        gameKnownDialogues = knownDialogues;        
        gameIsBadEnding = isBadEnding;
        gameEndOpportunities = endOpportunities;
    }

    public GameData()
    {
        gameScene = "SampleScene";
        gameGuilty = "";
        gameFirstClue = "";
        gameSecondClue = "";
        gameThirdClue = "";
        gameStoryPhase = 0;
        gameLastPuzzleComplete = "";        
        gameIsBadEnding = false;
        gameEndOpportunities = 2;
        for (int i = 0; i < gameKnownSuspects.Length; i++)
        {
            gameKnownSuspects[i] = false;
        }
        for (int i = 0; i < gameKnownTutorials.Length; i++)
        {
            gameKnownTutorials[i] = false;
        }
        gameKnownDialogues[0] = true;
        for (int i = 1; i < gameKnownDialogues.Length; i++)
        {
            gameKnownDialogues[i] = false;
        }
    }
}