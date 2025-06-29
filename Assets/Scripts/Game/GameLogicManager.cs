using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameLogicManager : MonoBehaviour
{
    public static GameLogicManager Instance { get; private set; }
    
    [Header("Configuration Data")]
    [SerializeField] private GameConfigurationData logicConfiguration;

    private string sceneToDestroy => logicConfiguration.sceneToDestroy;
    private Vector3 newGameStartPosition => logicConfiguration.newGameStartPosition;
    private int firstClueSubphaseIndex => logicConfiguration.firstClueSubphaseIndex;
    private int secondClueSubphaseIndex => logicConfiguration.secondClueSubphaseIndex;
    private int thirdClueSubphaseIndex => logicConfiguration.thirdClueSubphaseIndex;

    private GameObject player;
    private GameObject virtualCamera;
    private List<string> guiltyNames = new List<string>();
    private string guilty;
    private List<string> clues = new List<string>();
    private StoryPhase storyPhase;
    private string lastPuzzleComplete;
    private bool[] knownClues;
    private bool[] knownSuspects;
    private Dictionary<string, bool> knownTutorials;
    private Dictionary<string, bool> knownDialogues;
    private bool isBadEnding;
    private int endOpportunities;
    private List<PuzzleState> puzzleStateList;    
    private int totalPuzzlePoints;
    private int maxPuzzlePoints;

    private float[] temporarilyPlayerPosition = new float[3];
    private float[] temporarilyCameraPosition = new float[3];
    private string temporarilyPuzzleObject;
    private PlayerState temporarilyPlayerState;
    private List<string> firstClueGroup1;
    private List<string> firstClueGroup2;
    private List<string> secondClueGroup1;
    private List<string> secondClueGroup2;
    private List<string> thirdClueGroup1;
    private List<string> thirdClueGroup2;
    private bool isPuzzleCompleted;
    private bool isPuzzleIncomplete;
    private bool isPlayerInicialized;


    // En el Awake se define su comportamiento como singleton 
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
        guiltyNames = GameStateManager.Instance.gameText.guiltyNames;
        
        // Se usan lista para las comparaciones entre los sospechosos y las pistas
        firstClueGroup1 = new List<string> { guiltyNames[0], guiltyNames[1], guiltyNames[6], guiltyNames[7] };
        firstClueGroup2 = new List<string> { guiltyNames[2], guiltyNames[3], guiltyNames[4], guiltyNames[5] };
        secondClueGroup1 = new List<string> { guiltyNames[0], guiltyNames[1], guiltyNames[4], guiltyNames[5] };
        secondClueGroup2 = new List<string> { guiltyNames[2], guiltyNames[3], guiltyNames[6], guiltyNames[7] };
        thirdClueGroup1 = new List<string> { guiltyNames[0], guiltyNames[2], guiltyNames[4], guiltyNames[6] };
        thirdClueGroup2 = new List<string> { guiltyNames[1], guiltyNames[3], guiltyNames[5], guiltyNames[7] };
        
        GameData gameData = SaveManager.LoadGameData();
        FoundGuilty(gameData);
        FoundClues(gameData);
        FoundGamePhase(gameData);
        FoundKnownInformation(gameData);
        FoundEnding(gameData);

        PuzzleData puzzleData = SaveManager.LoadPuzzleData();
        FoundPuzzles(puzzleData);

        if (player != null)
        {    
            player.GetComponent<PlayerLogicManager>().InitializeFirstState();
            temporarilyPlayerState = player.GetComponent<PlayerLogicManager>().PlayerState;
        }

        if(GameStateManager.Instance.IsNewGame && SceneManager.GetActiveScene().name == GameStateManager.Instance.MainScene)
        {
            Debug.Log("Se ha comenzado una nueva partida.");            

            // Para empezar en un juego nuevo al lado de la cama
            if (player != null) player.transform.position = newGameStartPosition;
            else Debug.LogError("No se ha podido situar al jugador en la posición inicial al comenzar una nueva partida.");

            GameStateManager.Instance.IsGameStarted = true;

            GameStateManager.Instance.SaveData();
        }

        if(GameStateManager.Instance.IsLoadGame)
        {
            GameStateManager.Instance.LoadData(false); 
        }

        UpdateKnownClues();
    }

    // Métodos para obtener y para cambiar el objeto player, es decir, el avatar del jugador
    public GameObject Player
    {
        get { return player; }
        set { player = value; }
    }

    // Métodos para obtener y para cambiar el objeto virtual camera, es decir, la cámara del juego
    public GameObject VirtualCamera
    {
        get { return virtualCamera; }
        set { virtualCamera = value; }
    }

    // Método para obtener los posibles culpables
    public List<string> GuiltyNames
    {
        get { return new List<string>(guiltyNames); }
        set { guiltyNames = new List<string>(value); }
    }

    // Método para obtener el grupo 1 de sospechosos de la primera pista
    public List<string> FirstClueGroup1
    {
        get { return new List<string>(firstClueGroup1); }
    }

    // Método para obtener el grupo 1 de sospechosos de la primera pista
    public List<string> FirstClueGroup2
    {
        get { return new List<string>(firstClueGroup2); }
    }

    // Método para obtener el grupo 1 de sospechosos de la segunda pista
    public List<string> SecondClueGroup1
    {
        get { return new List<string>(secondClueGroup1); }
    }

    // Método para obtener el grupo 2 de sospechosos de la segunda pista
    public List<string> SecondClueGroup2
    {
        get { return new List<string>(secondClueGroup2); }
    }

    // Método para obtener el grupo 1 de sospechosos de la tercera pista
    public List<string> ThirdClueGroup1
    {
        get { return new List<string>(thirdClueGroup1); }
    }

    // Método para obtener el grupo 2 de sospechosos de la tercera pista
    public List<string> ThirdClueGroup2
    {
        get { return new List<string>(thirdClueGroup2); }
    }

    // Métodos para obtener y para cambiar el culpable
    public string Guilty
    {
        get { return guilty; }
        set {             
            if (value == null || value == "")
            {
                FoundGuilty(null);
            }
            else
            {
                guilty = value;
            }
        }
    }

    // Métodos para obtener y para cambiar las pistas 
    public List<string> Clues
    {
        get { return new List<string>(clues); }
        set {             
            if (clues == null || clues.Count == 0 || clues.All(element => element != "" || element != null))
            {
                FoundClues(null);
            }
            else
            {
                clues = value;
            }
        }
    }

    // Métodos para obtener y para cambiar la fase de la historia
    public StoryPhase CurrentStoryPhase
    {
        get { return storyPhase; }
        set { storyPhase = value; }
    }

    // Métodos para obtener y para cambiar el último puzle completado
    public string LastPuzzleComplete
    {
        get { return lastPuzzleComplete; }
        set { lastPuzzleComplete = value; }
    }

    // Métodos para obtener y para cambiar los sospechosos conocidos 
    public bool[] KnownClues
    {
        get { return (bool[])knownClues.Clone(); }
        set { knownClues = (bool[])value.Clone(); }
    }

    // Métodos para obtener y para cambiar los sospechosos conocidos 
    public bool[] KnownSuspects
    {
        get { return (bool[])knownSuspects.Clone(); }
        set { knownSuspects = (bool[])value.Clone(); }
    }

    // Métodos para obtener y para cambiar los tutoriales conocidos
    public Dictionary<string, bool> KnownTutorials
    {
        get { return new Dictionary<string, bool>(knownTutorials); }
        set { knownTutorials = new Dictionary<string, bool>(value); }
    }

    // Métodos para obtener y para cambiar los diálogos conocidos
    public Dictionary<string, bool> KnownDialogues
    {
        get { return new Dictionary<string, bool>(knownDialogues); }
        set { knownDialogues = new Dictionary<string, bool>(value); }
    }

    // Métodos para obtener y para cambiar el tipo de final
    public bool IsBadEnding
    {
        get { return isBadEnding; }
        set { isBadEnding = value; }
    }

    // Métodos para obtener y para cambiar las oportunidades finales de descubrir al culpable
    public int EndOpportunities
    {
        get { return endOpportunities; }
        set { endOpportunities = value; }
    }

    // Métodos para obtener y para cambiar la lista de puzles
    public List<PuzzleState> PuzzleStateList
    {
        get { return new List<PuzzleState>(puzzleStateList); }
        set { puzzleStateList = new List<PuzzleState>(value); }
    }

    // Métodos para obtener y para cambiar el número total de puntos obtenidos
    public int TotalPuzzlePoints
    {
        get { return totalPuzzlePoints; }
        set { totalPuzzlePoints = value; }
    }

    // Métodos para obtener y para cambiar el número máximo de puntos obtenidos
    public int MaxPuzzlePoints
    {
        get { return maxPuzzlePoints; }
        set { maxPuzzlePoints = value; }
    }

    // Métodos para obtener y para cambiar si se ha completado el último puzle
    public bool IsPuzzleCompleted
    {
        get { return isPuzzleCompleted; }
        set { isPuzzleCompleted = value; }
    }

    // Métodos para obtener y para cambiar si se ha regresado del último puzle sin completar
    public bool IsPuzzleIncomplete
    {
        get { return isPuzzleIncomplete; }
        set { isPuzzleIncomplete = value; }
    }

    // Métodos para obtener y para cambiar si se ha inicializado el jugador
    public bool IsPlayerInicialized
    {
        get { return isPlayerInicialized; }
        set { isPlayerInicialized = value; }
    }

    // Método para obtener el nombre del objeto que lanza el puzle
    public string TemporalPuzzleObject
    {
        get { return temporarilyPuzzleObject; }
    }

    // Métodos para obtener y para cambiar el estado temporal del jugador
    public PlayerState TemporalPlayerState
    {
        get { return temporarilyPlayerState; }
        set { temporarilyPlayerState = value; }
    }

    // Métodos para obtener el componente que gestiona el UI general
    public GameUIManager UIManager
    {
        get { return GetComponent<GameUIManager>(); }
    }

    // Método para cargar la información de quien es el culpable o asignarlo si no existe
    private void FoundGuilty(GameData gameData)
    {
        if(gameData != null && !string.IsNullOrEmpty(gameData.gameGuilty))
        {
            guilty = gameData.gameGuilty;
        }
        else
        {
            int randomIndex = Random.Range(0, guiltyNames.Count);
            guilty = guiltyNames[randomIndex];
        }
    }

    // Método para cargar la información de las pistas o asignarlas en función del culpable si no existe
    private void FoundClues(GameData gameData)
    {
        if (gameData != null && !(gameData.gameClues == null || gameData.gameClues.All(clue => clue == "")))
        {
            clues = gameData.gameClues;
        }
        else if (string.IsNullOrEmpty(guilty))
        {
            Debug.LogError("El nombre del culpable no ha sido asignado correctamente.");
        }
        else
        {
            if (firstClueGroup1.Contains(guilty))
            {
                clues.Insert(0, "Tiene los ojos marrones");
            }
            else if (firstClueGroup2.Contains(guilty))
            {
                clues.Insert(0, "Tiene los ojos verdes");
            }

            if (secondClueGroup1.Contains(guilty))
            {
                clues.Insert(1, "Mechón de pelo negro");
            }
            else if (secondClueGroup2.Contains(guilty))
            {
                clues.Insert(1, "Mechón de pelo rubio");
            }

            if (thirdClueGroup1.Contains(guilty))
            {
                clues.Insert(2, "Tiene una cicatriz");
            }
            else if (thirdClueGroup2.Contains(guilty))
            {
                clues.Insert(2, "Tiene un pendiente");
            }
        }        
    }

    // Método para cargar la información de la fase de historia en la que se encuentra el jugador o asignaer la primera si no existe
    private void FoundGamePhase(GameData gameData)
    {
        if(gameData != null && gameData.gameStoryPhase != null && gameData.gameStoryPhase.storySubphases != null 
            && gameData.gameStoryPhase.storySubphases.Count > 0)
        {
            storyPhase = gameData.gameStoryPhase.ToStoryPhase();
        }
        else
        {
            storyPhase = StoryStateManager.CreateFirstPhase();
        }
    }

    // Método para cargar la información de apartados que conoce el jugador o asignar los valores por defecto en caso contrario
    private void FoundKnownInformation(GameData gameData)
    {
        if(gameData != null && gameData.gameKnownClues?.Length > 0)
        {
            knownClues = gameData.gameKnownClues;
        }
        else
        {
            knownClues = new bool[3];
        }

        if(gameData != null && gameData.gameKnownSuspects?.Length > 0)
        {
            knownSuspects = gameData.gameKnownSuspects;
        }
        else
        {
            knownSuspects = new bool[8];
        }

        if (gameData != null && gameData.gameKnownTutorials != null && gameData.gameKnownTutorials.Count > 0)
        {
            knownTutorials = new Dictionary<string, bool>(gameData.GetKnownTutorials());
        }
        else
        {
            knownTutorials = new Dictionary<string, bool>();
        }

        if (gameData != null && gameData.gameKnownDialogues != null && gameData.gameKnownDialogues.Count > 0)
        {
            knownDialogues = new Dictionary<string, bool>(gameData.GetKnownDialogues());
        }
        else
        {
            knownDialogues = new Dictionary<string, bool>();
        }
    }    

    // Método para asegurarse de que las pistas van en consonancia con la fase de la historia
    public void UpdateKnownClues()
    {
        List<string> subphaseseList = StoryStateManager.CreateSubphasesList();
        int subphaseIndex = subphaseseList.IndexOf(storyPhase.GetPhaseToString());

        if (subphaseIndex == -1) Debug.LogError("Ha habido un error al encontrar la fase actual en la lista de subfases.");
        else if (subphaseIndex > thirdClueSubphaseIndex) knownClues = new bool[] {true, true, true};
        else if (subphaseIndex > secondClueSubphaseIndex) knownClues = new bool[] {true, true, false};
        else if (subphaseIndex > firstClueSubphaseIndex) knownClues = new bool[] {true, false, false};
    }

    // Método para cargar la información del final o asignar los valores por defecto en caso contrario
    private void FoundEnding(GameData gameData)
    {
        if(gameData != null && gameData.gameEndOpportunities is >= 0 and < 2)
        {
            endOpportunities = gameData.gameEndOpportunities;
        }
        else
        {
            endOpportunities = 2;
        }

        if(gameData != null && gameData.gameIsBadEnding)
        {
            isBadEnding = true;
        }
        else
        {
            isBadEnding = false;
        }
    }

    // Método para cargar la información de los puzles o crear una lista vacía de PuzzleState
    private void FoundPuzzles(PuzzleData puzzleData)
    {
        if (puzzleData != null && puzzleData.gamePuzzleStates != null && puzzleData.gamePuzzleStates.Count > 0)
        {
            puzzleStateList = puzzleData.gamePuzzleStates;
            PuzzleState lastCompletedPuzzle = puzzleData.gamePuzzleStates.Where(p => p.gameIsPuzzleComplete).LastOrDefault();
            lastPuzzleComplete = lastCompletedPuzzle != null ? lastCompletedPuzzle.gamePuzzleName : "";
            totalPuzzlePoints = puzzleData.gameTotalPuzzlePoints;
            maxPuzzlePoints = puzzleData.gameMaxPuzzlePoints;
        }
        else
        {
            puzzleStateList = new List<PuzzleState>();
            lastPuzzleComplete = "";
            totalPuzzlePoints = 0;
            maxPuzzlePoints = 0;
        }
    }

    // Método que se llama cuando se activa el objeto y sirve para suscribirse a eventos
    void OnEnable()
    {
        if (this == null || gameObject == null) return;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    // Método que se llama cuando se desactiva o se destruye el objeto y sirve para desuscribirse a eventos
    void OnDisable()
    {
        if (this == null || gameObject == null) return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // Método que se ejecuta cada vez que se carga una escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Se destruye el objeto donde está este script cuando se llega a una escena en concreto
        if (scene.name == sceneToDestroy)
        {
            Destroy(gameObject);
        }

        // Se busca el objeto player que haya en esta escena
        GameObject newPlayer = GameObject.Find("Player");
        if (newPlayer != null)
        {
            player = newPlayer;
            isPlayerInicialized = true;
        }

        // Se busca el objeto virtual camera que haya en esta escena
        GameObject newVirtualCamera = GameObject.Find("Virtual Camera");
        if (newVirtualCamera != null)
        {
            virtualCamera = newVirtualCamera;
        }

        // Se busca el objeto canvas que haya en esta escena
        if (!scene.name.Contains("Puzzle") && scene.name != sceneToDestroy) GetComponent<GameUIManager>().FindCanvas();
        
        // Se busca el objeto soundtrack que haya en esta escena
        GetComponent<GameUIManager>().FindAudio();

        // Se activa el NPC correspondiente en función del final del juego
        if (scene.name == GameStateManager.Instance.MainScene)
        {
            GameData gameData = SaveManager.LoadGameData();
            GameObject theEndObject = GameObject.Find("The End Object");

            if (gameData != null && !gameData.gameIsBadEnding && gameData.gameEndOpportunities == 0
                && gameData.gameStoryPhase.ToStoryPhase().phaseName == StoryPhaseOption.Ending)
            {
                GameObject victim = theEndObject.transform.Find("Victim").gameObject;
                victim.transform.GetChild(0).gameObject.SetActive(true);
            }
            else if (gameData != null && gameData.gameIsBadEnding && gameData.gameEndOpportunities == 0
                && gameData.gameStoryPhase.ToStoryPhase().phaseName == StoryPhaseOption.Ending)
            {
                GameObject father = theEndObject.transform.Find("Father").gameObject;
                father.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    // Método que se ejecuta cada vez que se descarga una escena
    private void OnSceneUnloaded(Scene current)
    {
        Player = null;
        isPlayerInicialized = false;
    }

    // Método para guardar la posición del jugador y de la cámara antes de lanzar un puzle
    public void SaveTemporarilyPosition(string puzzleObjectName)
    {
        temporarilyPlayerState = Player.GetComponent<PlayerLogicManager>().PlayerState;
        temporarilyPuzzleObject = puzzleObjectName;

        temporarilyPlayerPosition[0] = player.transform.position.x;
        temporarilyPlayerPosition[1] = player.transform.position.y;
        temporarilyPlayerPosition[2] = player.transform.position.z;

        temporarilyCameraPosition[0] = virtualCamera.transform.position.x;
        temporarilyCameraPosition[1] = virtualCamera.transform.position.y;
        temporarilyCameraPosition[2] = virtualCamera.transform.position.z;
    }

    // Método para cargar la posición del jugador y de la cámara antes de lanzar un puzle
    public void LoadTemporarilyPosition()
    {
        player.transform.position = new Vector3(temporarilyPlayerPosition[0], temporarilyPlayerPosition[1], 
            temporarilyPlayerPosition[2]);

        virtualCamera.transform.position = new Vector3(temporarilyCameraPosition[0], temporarilyCameraPosition[1], 
            temporarilyCameraPosition[2]);
    }
}
