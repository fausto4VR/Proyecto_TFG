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

    public GameData(string sceneName, string guilty, string firstClue, string secondClue, string thirdClue, int storyPhase, 
        string lastPuzzleComplete, bool[] knownSuspects)
    {
        gameScene = sceneName;
        gameGuilty = guilty;
        gameFirstClue = firstClue;
        gameSecondClue = secondClue;
        gameThirdClue = thirdClue;
        gameStoryPhase = storyPhase;
        gameLastPuzzleComplete = lastPuzzleComplete;
        gameKnownSuspects = knownSuspects;
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
    }
}
