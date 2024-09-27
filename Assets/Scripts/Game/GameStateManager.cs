using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public bool isPuzzleRecentlyCompleted;
    public float[] actualPlayerPosition;
    public float[] actualCameraPosition;
    public bool isPuzzleIncomplete;
    public bool[] lastPuzzleSupports;
    public int lastPuzzlePoints;
    public string actualPuzzleName;

    private GameObject player;
    private string sceneName;
    private string guilty;
    private string firstClue = "";
    private string secondClue = "";
    private string thirdClue = "";
    private int storyPhase;
    private string lastPuzzleComplete;
    private bool[] knownSuspects;
    private bool[] knownTutorials;
    private bool[] knownDialogues;
    private bool isBadEnding;
    
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

    public void SaveData()
    {
        player = GameObject.Find("Player");
        SaveManager.SavePlayerData(player);

        sceneName = SceneManager.GetActiveScene().name;
        guilty = GameLogicManager.Instance.guilty;
        firstClue = GameLogicManager.Instance.firstClue;
        secondClue = GameLogicManager.Instance.secondClue;
        thirdClue = GameLogicManager.Instance.thirdClue;
        storyPhase = GameLogicManager.Instance.storyPhase;
        lastPuzzleComplete = GameLogicManager.Instance.lastPuzzleComplete;
        knownSuspects = GameLogicManager.Instance.knownSuspects;
        knownTutorials = GameLogicManager.Instance.knownTutorials;        
        knownDialogues = GameLogicManager.Instance.knownDialogues;        
        isBadEnding = GameLogicManager.Instance.isBadEnding;
        SaveManager.SaveGameData(sceneName, guilty, firstClue, secondClue, thirdClue, storyPhase, lastPuzzleComplete, 
            knownSuspects, knownTutorials, knownDialogues, isBadEnding);

        SavePuzzleData();

        Debug.Log("Datos guardados");
    }

    public void LoadData()
    {
        GameData gameData = SaveManager.LoadGameData();
        GameLogicManager.Instance.guilty = gameData.gameGuilty;
        GameLogicManager.Instance.firstClue = gameData.gameFirstClue;
        GameLogicManager.Instance.secondClue = gameData.gameSecondClue;
        GameLogicManager.Instance.thirdClue = gameData.gameThirdClue;
        GameLogicManager.Instance.storyPhase = gameData.gameStoryPhase;
        GameLogicManager.Instance.lastPuzzleComplete = gameData.gameLastPuzzleComplete;
        GameLogicManager.Instance.knownSuspects = gameData.gameKnownSuspects;
        GameLogicManager.Instance.knownTutorials = gameData.gameKnownTutorials;
        GameLogicManager.Instance.knownDialogues = gameData.gameKnownDialogues;        
        GameLogicManager.Instance.isBadEnding = gameData.gameIsBadEnding;
        SceneManager.LoadScene(gameData.gameScene);

        LoadPuzzleData();

        // Evento que se dispara cuando la escena está cargada
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Método que se llama cuando una escena es completamente cargada
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerData playerData = SaveManager.LoadPlayerData();
        player = GameObject.Find("Player");

        if (player != null)
        {
            player.transform.position = new Vector3(playerData.position[0], playerData.position[1], playerData.position[2]);
        }
        else
        {
            Debug.LogError("No se ha encontrado al jugador en la escena.");
        }
        Debug.Log("Datos cargados");
    }

    public void ResetData()
    {
        SaveManager.ResetPlayerData();
        SaveManager.ResetGameData();
        SaveManager.ResetPuzzleData();
        Debug.Log("Datos reseteados");
    }

    private void SavePuzzleData()
    {
        PuzzleData puzzleData = SaveManager.LoadPuzzleData();
        bool[] emptyBool = new bool[3];

        if(actualPuzzleName == "Puzzle1")
        {
            SaveManager.SavePuzzleData(lastPuzzleSupports, lastPuzzlePoints, emptyBool, 0, emptyBool, 0, emptyBool, 0, emptyBool, 0, 
                emptyBool, 0, emptyBool, 0, emptyBool, 0);
        }
        else if(actualPuzzleName == "Puzzle2")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, lastPuzzleSupports, 
                lastPuzzlePoints, emptyBool, 0, emptyBool, 0, emptyBool, 0, emptyBool, 0, emptyBool, 0, emptyBool, 0);
        }
        else if(actualPuzzleName == "Puzzle3")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, lastPuzzleSupports, lastPuzzlePoints, emptyBool, 0, emptyBool, 0, emptyBool, 0, 
                emptyBool, 0, emptyBool, 0);
        }
        else if(actualPuzzleName == "Puzzle4")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleData.gamePuzzle3Supports, puzzleData.gamePuzzle3Points, 
                lastPuzzleSupports, lastPuzzlePoints, emptyBool, 0, emptyBool, 0, emptyBool, 0, emptyBool, 0);
        }
        else if(actualPuzzleName == "Puzzle5")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleData.gamePuzzle3Supports, puzzleData.gamePuzzle3Points, 
                puzzleData.gamePuzzle4Supports, puzzleData.gamePuzzle4Points, lastPuzzleSupports, lastPuzzlePoints, emptyBool, 0, 
                emptyBool, 0, emptyBool, 0);
        }
        else if(actualPuzzleName == "Puzzle6")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleData.gamePuzzle3Supports, puzzleData.gamePuzzle3Points, 
                puzzleData.gamePuzzle4Supports, puzzleData.gamePuzzle4Points, puzzleData.gamePuzzle5Supports, 
                puzzleData.gamePuzzle5Points, lastPuzzleSupports, lastPuzzlePoints, emptyBool, 0, emptyBool, 0);
        }
        else if(actualPuzzleName == "Puzzle7")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleData.gamePuzzle3Supports, puzzleData.gamePuzzle3Points, 
                puzzleData.gamePuzzle4Supports, puzzleData.gamePuzzle4Points, puzzleData.gamePuzzle5Supports, 
                puzzleData.gamePuzzle5Points, puzzleData.gamePuzzle6Supports, puzzleData.gamePuzzle6Points, 
                lastPuzzleSupports, lastPuzzlePoints, emptyBool, 0);
        }
        else if(actualPuzzleName == "Puzzle8")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleData.gamePuzzle3Supports, puzzleData.gamePuzzle3Points, 
                puzzleData.gamePuzzle4Supports, puzzleData.gamePuzzle4Points, puzzleData.gamePuzzle5Supports, 
                puzzleData.gamePuzzle5Points, puzzleData.gamePuzzle6Supports, puzzleData.gamePuzzle6Points, 
                puzzleData.gamePuzzle7Supports, puzzleData.gamePuzzle7Points, lastPuzzleSupports, lastPuzzlePoints);
        }
    }

    private void LoadPuzzleData()
    {
        PuzzleData puzzleData = SaveManager.LoadPuzzleData();
        lastPuzzleComplete = GameLogicManager.Instance.lastPuzzleComplete;

        if(lastPuzzleComplete == "")
        {
            lastPuzzleSupports = puzzleData.gamePuzzle1Supports;
            lastPuzzlePoints = puzzleData.gamePuzzle1Points;
        }
        else if(lastPuzzleComplete == "Puzzle1")
        {
            lastPuzzleSupports = puzzleData.gamePuzzle2Supports;
            lastPuzzlePoints = puzzleData.gamePuzzle2Points;
        }
        else if(lastPuzzleComplete == "Puzzle2")
        {
            lastPuzzleSupports = puzzleData.gamePuzzle3Supports;
            lastPuzzlePoints = puzzleData.gamePuzzle3Points;
        }
        else if(lastPuzzleComplete == "Puzzle3")
        {
            lastPuzzleSupports = puzzleData.gamePuzzle4Supports;
            lastPuzzlePoints = puzzleData.gamePuzzle4Points;
        }
        else if(lastPuzzleComplete == "Puzzle4")
        {
            lastPuzzleSupports = puzzleData.gamePuzzle5Supports;
            lastPuzzlePoints = puzzleData.gamePuzzle5Points;
        }
        else if(lastPuzzleComplete == "Puzzle5")
        {
            lastPuzzleSupports = puzzleData.gamePuzzle6Supports;
            lastPuzzlePoints = puzzleData.gamePuzzle6Points;
        }
        else if(lastPuzzleComplete == "Puzzle6")
        {
            lastPuzzleSupports = puzzleData.gamePuzzle7Supports;
            lastPuzzlePoints = puzzleData.gamePuzzle7Points;
        }
        else if(lastPuzzleComplete == "Puzzle7")
        {
            lastPuzzleSupports = puzzleData.gamePuzzle8Supports;
            lastPuzzlePoints = puzzleData.gamePuzzle8Points;
        }
        else
        {
            lastPuzzleSupports = new bool[3];
            lastPuzzlePoints = 0;
        }
    }
}
