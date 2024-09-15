[System.Serializable]
public class GameData
{
    public string gameScene = "";

    public GameData(string sceneName)
    {
        gameScene = sceneName;
    }

    public GameData()
    {
        gameScene = "SampleScene";
    }
}
