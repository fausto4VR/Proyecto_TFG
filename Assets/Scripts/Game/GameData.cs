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

    public GameData(string sceneName, string guilty, string firstClue, string secondClue, string thirdClue, int storyPhase, 
        string lastPuzzleComplete, bool[] knownSuspects, bool[] knownTutorials)
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
        for (int i = 0; i < gameKnownSuspects.Length; i++)
        {
            gameKnownSuspects[i] = false;
        }
        for (int i = 0; i < gameKnownTutorials.Length; i++)
        {
            gameKnownTutorials[i] = false;
        }
    }
}
