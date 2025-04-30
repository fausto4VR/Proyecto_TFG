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
    public bool isLoadGame;
    public bool isNewGame;
    // -----------------------------------------------------------------

    // Contenedor de los textos cargados
    public GameTextDictionary gameText { get; private set; }

    // Contenedor de las conversaciones cargadas
    public GameConversationDictionary gameConversations { get; private set; }

    
    // En el Awake se define su comportamiento como singleton. Además se genera  la clave de encriptación y se cargan los 
    // textos del juego, las conversaciones y las fases de la historia   
    void Awake()
    {        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Se establece la tasa máxima de frames
        Application.targetFrameRate = 120;
        
        SaveManager.GenerateKey();
        LoadGameTexts();
        LoadGameConversations();
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
        GameLogicManager.Instance.TemporalPlayerState = GameLogicManager.Instance.Player
            .GetComponent<PlayerLogicManager>().DefaultStateInitialized;
        
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
        StoryPhase storyPhase = GameLogicManager.Instance.CurrentStoryPhase; 
        bool[] knownClues = GameLogicManager.Instance.KnownClues; 
        bool[] knownSuspects = GameLogicManager.Instance.KnownSuspects;
        Dictionary<string, bool> knownTutorials = GameLogicManager.Instance.KnownTutorials;        
        Dictionary<string, bool> knownDialogues = GameLogicManager.Instance.KnownDialogues;        
        bool isBadEnding = GameLogicManager.Instance.IsBadEnding;
        int endOpportunities = GameLogicManager.Instance.EndOpportunities;

        GameData gameData = new GameData(sceneName, guilty, clues, storyPhase, knownClues, knownSuspects, 
            knownTutorials, knownDialogues, isBadEnding, endOpportunities);

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
        GameLogicManager.Instance.CurrentStoryPhase = gameData.gameStoryPhase.ToStoryPhase();        
        GameLogicManager.Instance.KnownClues = gameData.gameKnownClues;
        GameLogicManager.Instance.KnownSuspects = gameData.gameKnownSuspects;
        GameLogicManager.Instance.KnownTutorials = gameData.GetKnownTutorials();
        GameLogicManager.Instance.KnownDialogues = gameData.GetKnownDialogues();        
        GameLogicManager.Instance.IsBadEnding = gameData.gameIsBadEnding;
        GameLogicManager.Instance.EndOpportunities = gameData.gameEndOpportunities;

        SceneManager.LoadScene(gameData.gameScene);
    }

    // Método para cargar los datos y obtener los parámatros de los puzles
    private void LoadPuzzleData()
    {
        PuzzleData puzzleData = SaveManager.LoadPuzzleData();
        GameLogicManager.Instance.PuzzleStateList = puzzleData.gamePuzzleStates;
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
        public TextPuzzle puzzle1;
        public TextPuzzle puzzle2;        
        public TextPuzzle puzzle3;
        public TextPuzzle puzzle4;
        public TextPuzzle puzzle5;
        public TextPuzzle puzzle6;
        public TextPuzzle puzzle7;
        public TextPuzzle puzzle8;
    }

    // Estructura de datos que representa los textos asociados a un "puzzle"
    [System.Serializable]
    public class TextPuzzle
    {
        public string puzzleStatementText;
        public string firstSupportText;
        public string secondSupportText;
        public string thirdSupportText;
    }

    // Método para cargar las conversaciones del juego desde el fichero json
    private void LoadGameConversations() 
    {
        string path = Path.Combine(Application.streamingAssetsPath, "GameConversations.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            gameConversations = JsonUtility.FromJson<GameConversationDictionary>(json);

            if (gameConversations == null)
            {
                Debug.LogError("Hubo un error al parsear el archivo JSON GameConversations.");
            }
        }
        else
        {
            Debug.LogError("No se ha encontrado el archivo GameConversations.json en StreamingAssets.");
        }
    }

    // Estructura de datos que representa el diccionario de conversaciones del juego y sirve para parsear el JSON 
    [System.Serializable]
    public class GameConversationDictionary
    {        
        public List<DialogueLine> defaultInspectConversation;
        public List<InspectInformation> inspectConversations;
        public List<MultipleChoiceInformation> multipleChoiceConversations;
        public List<StoryPhaseInformation> storyPhaseConversations;
        public List<EndConversationInformation> theEndConversations;
        public List<TutorialInformation> tutorialTexts;
    }

    // Estructura de datos que representa el diálogo al inpeccionar un objeto 
    [System.Serializable]
    public class InspectInformation
    {
        public string objectName;
        public List<DialogueLine> currentDialogue;
        public List<DialogueLine> afterRecentDialogue;
        public List<DialogueLine> afterDistantDialogue;
    }

    // Estructura de datos que representa un diálogo con varias opciones
    [System.Serializable]
    public class MultipleChoiceInformation
    {
        public string objectName;
        public List<DialogueLine> firstDialogue;
        public List<DialogueLine> secondDialogue;
        public List<DialogueLine> thirdDialogue;
    }

    // Estructura de datos que representa un diálogo que cambia en función de la fase de la historia
    [System.Serializable]
    public class StoryPhaseInformation
    {
        public string objectName;
        public List<StoryPhaseDialogue> dialogues;
    }

    // Estructura de datos que representa un diálogo concreto en una fase de la historia
    [System.Serializable]
    public class StoryPhaseDialogue
    {
        public string phase;
        public string subphase;
        public List<DialogueLine> dialogue;
    }

    // Estructura de datos que representa un diálogo de uno de los personajes del final del juego
    [System.Serializable]
    public class EndConversationInformation
    {
        public string objectName;
        public List<EndDialogue> dialogues;
    }

    // Estructura de datos que representa un diálogo concreto de uno de los personajes del final del juego
    [System.Serializable]
    public class EndDialogue
    {
        public string guilty;
        public List<DialogueLine> dialogue;
    }

    // Estructura de datos que representa el texto que muestra un tutorial
    [System.Serializable]
    public class TutorialInformation
    {
        public string objectName;
        public List<DialogueLine> text;
    }

    // Estructura de datos que representa una línea de diálogo
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        public string line;
    }
}
