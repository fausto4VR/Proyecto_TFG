[System.Serializable]
public class GameData
{
    public string gameScene = "";
    public string gameGuilty = "";
    public string gameFirstClue = "";
    public string gameSecondClue = "";
    public string gameThirdClue = "";

    public GameData(string sceneName, string guilty, string firstClue, string secondClue, string thirdClue)
    {
        gameScene = sceneName;
        gameGuilty = guilty;
        gameFirstClue = firstClue;
        gameSecondClue = secondClue;
        gameThirdClue = thirdClue;
    }

    public GameData()
    {
        gameScene = "SampleScene";
        gameGuilty = "";
        gameFirstClue = "";
        gameSecondClue = "";
        gameThirdClue = "";
    }
}
