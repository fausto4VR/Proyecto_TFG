using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    // REVISAR
    public bool isPuzzleRecentlyCompleted;
    public float[] actualPlayerPosition;
    public float[] actualCameraPosition;
    public bool isPuzzleIncomplete;
    public bool[] lastPuzzleSupports;
    public int lastPuzzlePoints;
    public string actualPuzzleName;
    public bool isLoadGame;
    public bool isNewGame;

    // REVISAR
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
    private int endOpportunities;

    // Este es el contenedor de los textos cargados
    public GameTextDictionary gameText { get; private set; }

    // En el Awake definimos su comportamiento como singleton    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        loadGameTexts();
    }

    // REVISAR
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
        endOpportunities = GameLogicManager.Instance.endOpportunities;
        SaveManager.SaveGameData(sceneName, guilty, firstClue, secondClue, thirdClue, storyPhase, lastPuzzleComplete, 
            knownSuspects, knownTutorials, knownDialogues, isBadEnding, endOpportunities);

        SavePuzzleData();

        Debug.Log("Datos guardados");
    }

    // REVISAR
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
        GameLogicManager.Instance.endOpportunities = gameData.gameEndOpportunities;
        SceneManager.LoadScene(gameData.gameScene);

        LoadPuzzleData();

        // Evento que se dispara cuando la escena está cargada
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // REVISAR
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

    // REVISAR
    public void ResetData()
    {
        SaveManager.ResetPlayerData();
        SaveManager.ResetGameData();
        SaveManager.ResetPuzzleData();
        Debug.Log("Datos reseteados");
    }

    // REVISAR
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

    // REVISAR
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

    // Método para cargar los textos del juego desde el fichero json
    public void loadGameTexts() 
    {
        string path = Path.Combine(Application.streamingAssetsPath, "gameText.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            gameText = JsonUtility.FromJson<GameTextDictionary>(json);

            if (gameText == null)
            {
                Debug.LogError("Hubo un error al parsear el archivo JSON.");
            }
        }
        else
        {
            Debug.LogError("No se ha encontrado el archivo gameText.json en StreamingAssets.");
        }
    }

    // Estructura de datos que representa el diccionario de textos del juego y sirve para parsear el JSON 
    [System.Serializable]
    public class GameTextDictionary
    {
        public TextPuzzle puzzle_1;
        public TextPuzzle puzzle_2;
    }

    // Estructura de datos que representa los textos asociados a un "puzzle"
    [System.Serializable]
    public class TextPuzzle
    {
        public string puzzle_statement_text;
        public string first_support_text;
        public string second_support_text;
        public string third_support_text;
    }
}
