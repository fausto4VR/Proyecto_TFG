using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Variable Section")]
    [SerializeField] private string mainScene = "SampleScene";

    // QUITAR ----------------------------------------------------------
    [Header("QUITAR")]
    public bool isPuzzleIncomplete;
    public bool isLoadGame;
    public bool isNewGame;
    // -----------------------------------------------------------------


    // Contenedor de los textos cargados
    public GameTextDictionary gameText { get; private set; }
    
    // En el Awake se define su comportamiento como singleton. Además se genera  la clave de encriptación y se cargan los 
    // textos del juego y las fases de la historia   
    void Awake()
    {        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SaveManager.GenerateKey();
        LoadGameTexts();
        StoryStateManager.LoadGameStory();
    }

    // QUITAR ----------------------------------------------------------
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            LoadGameTexts();
            StoryStateManager.LoadGameStory();
        }        
    }
    // -----------------------------------------------------------------

    // Método para consultar la escena principal
    public string MainScene
    {
        get { return mainScene; }
    }

    // Método para guardar los datos
    public void SaveData()
    {
        SavePlayerData();
        SaveGameData();
        SavePuzzleData();
        Debug.Log("Datos guardados");
    }

    // Método para cargar los datos
    public void LoadData()
    {
        LoadGameData();
        LoadPuzzleData();

        // Evento que se dispara cuando la escena está cargada
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Debug.Log("Datos cargados");
    }
    
    // Método para reiniciar los datos
    public void ResetData()
    {
        SaveManager.ResetPlayerData();
        SaveManager.ResetGameData();
        SaveManager.ResetPuzzleData();
        Debug.Log("Datos reseteados");
    }

    // Método para obtener los parámatros del jugador y guardarlos
    private void SavePlayerData()
    {  
        GameObject player = GameLogicManager.Instance.Player;       
        float positionX = player.transform.position.x;
        float positionY = player.transform.position.y;
        float positionZ = player.transform.position.z;

        PlayerData playerData = new PlayerData(positionX, positionY, positionZ);
        SaveManager.SavePlayerData(playerData);
    }

    // Método para obtener los parámatros del juego y guardarlos
    private void SaveGameData()
    {      
        string sceneName = SceneManager.GetActiveScene().name;
        string guilty = GameLogicManager.Instance.Guilty;
        List<string> clues = GameLogicManager.Instance.Clues;
        int storyPhaseAux = GameLogicManager.Instance.StoryPhaseAux; // QUITAR
        StoryPhase storyPhase = GameLogicManager.Instance.CurrentStoryPhase; 
        bool[] knownSuspects = GameLogicManager.Instance.KnownSuspects;
        bool[] knownTutorials = GameLogicManager.Instance.KnownTutorials;        
        bool[] knownDialogues = GameLogicManager.Instance.KnownDialogues;        
        bool isBadEnding = GameLogicManager.Instance.IsBadEnding;
        int endOpportunities = GameLogicManager.Instance.EndOpportunities;

        GameData gameData = new GameData(sceneName, guilty, clues, storyPhaseAux, storyPhase, knownSuspects, knownTutorials, 
            knownDialogues, isBadEnding, endOpportunities);

        SaveManager.SaveGameData(gameData);
    }

    // Método para obtener los parámatros de los puzles y guardarlos
    private void SavePuzzleData()
    {
        List<PuzzleState> puzzleStateList = GameLogicManager.Instance.PuzzleStateList;
        PuzzleData puzzleData = new PuzzleData(puzzleStateList);
        SaveManager.SavePuzzleData(puzzleData);
    }

    // Método para cargar los datos y obtener los parámatros del juego
    private void LoadGameData()
    {
        GameData gameData = SaveManager.LoadGameData();

        GameLogicManager.Instance.Guilty = gameData.gameGuilty;
        GameLogicManager.Instance.Clues = gameData.gameClues;
        GameLogicManager.Instance.StoryPhaseAux = gameData.gameStoryPhaseAux; // QUITAR
        GameLogicManager.Instance.CurrentStoryPhase = gameData.gameStoryPhase;
        GameLogicManager.Instance.KnownSuspects = gameData.gameKnownSuspects;
        GameLogicManager.Instance.KnownTutorials = gameData.gameKnownTutorials;
        GameLogicManager.Instance.KnownDialogues = gameData.gameKnownDialogues;        
        GameLogicManager.Instance.IsBadEnding = gameData.gameIsBadEnding;
        GameLogicManager.Instance.EndOpportunities = gameData.gameEndOpportunities;

        SceneManager.LoadScene(gameData.gameScene);
    }

    // Método que se llama cuando una escena es completamente cargada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerData playerData = SaveManager.LoadPlayerData();
        GameObject player = GameLogicManager.Instance.Player;

        if (player != null)
        {
            player.transform.position = new Vector3(playerData.position[0], playerData.position[1], playerData.position[2]);
        }
        else
        {
            Debug.LogError("No se ha encontrado al jugador en la escena.");
        }
    }

    // Método para cargar los datos y obtener los parámatros de los puzles
    private void LoadPuzzleData()
    {
        PuzzleData puzzleData = SaveManager.LoadPuzzleData();
        GameLogicManager.Instance.PuzzleStateList = puzzleData.gamePuzzleStates;
    }

    // Método para cargar los textos del juego desde el fichero json
    private void LoadGameTexts() 
    {
        string path = Path.Combine(Application.streamingAssetsPath, "GameText.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            gameText = JsonUtility.FromJson<GameTextDictionary>(json);

            if (gameText == null)
            {
                Debug.LogError("Hubo un error al parsear el archivo JSON GameText.");
            }
        }
        else
        {
            Debug.LogError("No se ha encontrado el archivo GameText.json en StreamingAssets.");
        }
    }

    // Estructura de datos que representa el diccionario de textos del juego y sirve para parsear el JSON 
    [System.Serializable]
    public class GameTextDictionary
    {
        public List<string> guiltyNames;
        public TextPuzzle puzzle_1;
        public TextPuzzle puzzle_2;        
        public TextPuzzle puzzle_3;
        public TextPuzzle puzzle_4;
        public TextPuzzle puzzle_5;
        public TextPuzzle puzzle_6;
        public TextPuzzle puzzle_7;
        public TextPuzzle puzzle_8;
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
