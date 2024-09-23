using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogicManager : MonoBehaviour
{
    public static GameLogicManager Instance;
    public List<string> guiltyNames = new List<string> { "guilty1", "guilty2", "guilty3", "guilty4", "guilty5", "guilty6", "guilty7", "guilty8" };
    public string guilty;
    public string firstClue;
    public string secondClue;
    public string thirdClue;
    public int storyPhase;
    public string lastPuzzleComplete;
    public bool[] knownSuspects;
    public bool[] knownTutorials;
    public bool[] knownDialogues;
    
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
        FoundStatistics(gameData);
        if(SceneManager.GetActiveScene().name == "SampleScene")
        {
            GameStateManager.Instance.SaveData(); 
        }    
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

    private void FoundStatistics(GameData gameData)
    {
        if(!string.IsNullOrEmpty(gameData.gameLastPuzzleComplete))
        {
            lastPuzzleComplete = gameData.gameLastPuzzleComplete;
        }

        if(gameData.gameKnownSuspects != null && gameData.gameKnownSuspects.Length > 0)
        {
            knownSuspects = gameData.gameKnownSuspects;
        }
        else
        {
            knownSuspects = new bool[8]; 
            for (int i = 0; i < guiltyNames.Count; i++)
            {
                knownSuspects[i] = false;
            }
        }

        if(gameData.gameKnownTutorials != null && gameData.gameKnownTutorials.Length > 0)
        {
            knownTutorials = gameData.gameKnownTutorials;
        }
        else
        {
            knownTutorials = new bool[8]; 
            for (int i = 0; i < knownTutorials.Length; i++)
            {
                knownTutorials[i] = false;
            }
        }

        if(gameData.gameKnownDialogues != null && gameData.gameKnownDialogues.Length > 0)
        {
            knownDialogues = gameData.gameKnownDialogues;
        }
        else
        {
            knownDialogues = new bool[8];
            knownDialogues[0] = true; 
            for (int i = 1; i < knownDialogues.Length; i++)
            {
                knownDialogues[i] = false;
            }
        }
    }
}
