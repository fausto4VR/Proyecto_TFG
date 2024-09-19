using System.Collections.Generic;
using UnityEngine;

public class GameLogicManager : MonoBehaviour
{
    public static GameLogicManager Instance;
    public List<string> guiltyNames = new List<string> { "guilty1", "guilty2", "guilty3", "guilty4", "guilty5", "guilty6", "guilty7", "guilty8" };
    public string guilty;
    public string firstClue;
    public string secondClue;
    public string thirdClue;
    public int storyPhase;
    private GameData gameData;

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
        gameData = SaveManager.LoadGameData();
        FoundGuilty(gameData);
        FoundClues(gameData);
        FoundGamePhase(gameData);
        GameStateManager.Instance.SaveData();     
    }

    private void FoundGuilty(GameData gameData)
    {
        if(string.IsNullOrEmpty(gameData.gameGuilty))
        {
            int randomIndex = Random.Range(0, guiltyNames.Count);
            guilty = guiltyNames[randomIndex];
        }
        else
        {
            guilty = gameData.gameGuilty;
        }
    }

    private void FoundClues(GameData gameData)
    {
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
    private void FoundGamePhase(GameData gameData)
    {
        if(gameData.gameStoryPhase != 0)
        {
            storyPhase = gameData.gameStoryPhase;
        }
        else
        {
            storyPhase = 0;
        }
    }
}
