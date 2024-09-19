using System.Collections.Generic;
using UnityEngine;

public class GameLogicManager : MonoBehaviour
{
    public static GameLogicManager Instance;
    public List<string> guiltyNames = new List<string> { "guilty1", "guilty2", "guilty3", "guilty4", "guilty5", "guilty6", "guilty7", "guilty8" };
    public string guilty;
    public string firstClue = "";
    public string secondClue = "";
    public string thirdClue = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        FoundGuilty();
        FoundClues();
        GameStateManager.Instance.SaveData();            
    }

    private void FoundGuilty()
    {
        GameData gameData = SaveManager.LoadGameData();
        
        if(gameData.gameGuilty == "" || gameData.gameGuilty == null)
        {
            int randomIndex = Random.Range(0, guiltyNames.Count);
            guilty = guiltyNames[randomIndex];
        }
        else
        {
            guilty = gameData.gameGuilty;
        }
    }

    private void FoundClues()
    {
        GameData gameData = SaveManager.LoadGameData();

        if(!string.IsNullOrEmpty(gameData.gameFirstClue))
        {
            firstClue = gameData.gameFirstClue;
        }

        if(!string.IsNullOrEmpty(gameData.gameSecondClue))
        {
            secondClue = gameData.gameSecondClue;
        }

        if(!string.IsNullOrEmpty(gameData.gameThirdClue))
        {
            thirdClue = gameData.gameThirdClue;
        }
    }
}
