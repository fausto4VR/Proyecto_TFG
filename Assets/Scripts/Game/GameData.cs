[System.Serializable]
public class GameData
{
    public string gameScene = "";
    public string gameGuilty = "";
    public string gameFirstClue = "";
    public string gameSecondClue = "";
    public string gameThirdClue = "";
    public int gameStoryPhase = 0;

    public GameData(string sceneName, string guilty, string firstClue, string secondClue, string thirdClue, int storyPhase)
    {
        gameScene = sceneName;
        gameGuilty = guilty;
        gameFirstClue = firstClue;
        gameSecondClue = secondClue;
        gameThirdClue = thirdClue;
        gameStoryPhase = storyPhase;
    }

    public GameData()
    {
        gameScene = "SampleScene";
        gameGuilty = "";
        gameFirstClue = "";
        gameSecondClue = "";
        gameThirdClue = "";
        gameStoryPhase = 0;
    }
}
