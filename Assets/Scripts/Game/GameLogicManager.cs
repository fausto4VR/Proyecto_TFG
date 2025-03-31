using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameLogicManager : MonoBehaviour
{
    public static GameLogicManager Instance { get; private set; }
    
    [Header("Variable Section")]
    [SerializeField] private string sceneToDestroy = "MenuScene";

    private List<string> guiltyNames = new List<string>();
    private string guilty;
    private List<string> clues = new List<string>();
    private int storyPhaseAux;
    private StoryPhase storyPhase;
    private string lastPuzzleComplete;
    private bool[] knownSuspects;
    private bool[] knownTutorials;
    private bool[] knownDialogues;
    private bool isBadEnding;
    private int endOpportunities;
    private List<PuzzleState> puzzleStateList;

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
        
        GameData gameData = SaveManager.LoadGameData();
        FoundGuilty(gameData);
        FoundClues(gameData);
        FoundGamePhaseAux(gameData); // QUITAR
        FoundGamePhase(gameData);
        FoundKnownInformation(gameData);
        FoundEnding(gameData);

        PuzzleData puzzleData = SaveManager.LoadPuzzleData();
        FoundPuzzles(puzzleData);

        // QUITAR ----------------------------------------------------------
        if(GameStateManager.Instance.isLoadGame && SceneManager.GetActiveScene().name == GameStateManager.Instance.MainScene)
        {
            GameStateManager.Instance.LoadData();
            GameStateManager.Instance.isLoadGame = false; 
        }

        if(GameStateManager.Instance.isNewGame && SceneManager.GetActiveScene().name == GameStateManager.Instance.MainScene)
        {
            storyPhaseAux = 1; // QUITAR
            GameStateManager.Instance.SaveData();
            GameStateManager.Instance.isNewGame = false; 
        }
        // -----------------------------------------------------------------      
    }

    void Update()
    {
        // QUITAR ----------------------------------------------------------
        if(GameStateManager.Instance.isLoadGame && SceneManager.GetActiveScene().name == GameStateManager.Instance.MainScene)
        {
            GameStateManager.Instance.LoadData();
            GameStateManager.Instance.isLoadGame = false; 
        }
        // -----------------------------------------------------------------

        // QUITAR ----------------------------------------------------------
        if(Input.GetKeyDown(KeyCode.R))
        {
            GameStateManager.Instance.ResetData();
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            GameStateManager.Instance.SaveData();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            GameStateManager.Instance.LoadData();
        }
        // -----------------------------------------------------------------
    }

    // Método para obtener los posibles culpables
    public List<string> GuiltyNames
    {
        get { return new List<string>(guiltyNames); }
        set { guiltyNames = new List<string>(value); }
    }

    // Métodos para obtener y para cambiar el culpable
    public string Guilty
    {
        get { return guilty; }
        set { guilty = value; }
    }

    // Métodos para obtener y para cambiar las pistas 
    public List<string> Clues
    {
        get { return new List<string>(clues); }
        set { clues = new List<string>(value); }
    }

    // QUITAR --------------------------------------------------------- 
    public int StoryPhaseAux
    {
        get { return storyPhaseAux; }
        set { storyPhaseAux = value; }
    }
    // -----------------------------------------------------------------

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
    public bool[] KnownSuspects
    {
        get { return (bool[])knownSuspects.Clone(); }
        set { knownSuspects = (bool[])value.Clone(); }
    }

    // Métodos para obtener y para cambiar los tutoriales conocidos
    public bool[] KnownTutorials
    {
        get { return (bool[])knownTutorials.Clone(); }
        set { knownTutorials = (bool[])value.Clone(); }
    }

    // Métodos para obtener y para cambiar los diálogos conocidos
    public bool[] KnownDialogues
    {
        get { return (bool[])knownDialogues.Clone(); }
        set { knownDialogues = (bool[])value.Clone(); }
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
            // Se usan listas para optimizar las comparaciones
            List<string> group1 = new List<string> { guiltyNames[0], guiltyNames[1], guiltyNames[6], guiltyNames[7] };
            List<string> group2 = new List<string> { guiltyNames[2], guiltyNames[3], guiltyNames[4], guiltyNames[5] };
            List<string> group3 = new List<string> { guiltyNames[0], guiltyNames[1], guiltyNames[4], guiltyNames[5] };
            List<string> group4 = new List<string> { guiltyNames[2], guiltyNames[3], guiltyNames[6], guiltyNames[7] };
            List<string> group5 = new List<string> { guiltyNames[0], guiltyNames[2], guiltyNames[4], guiltyNames[6] };
            List<string> group6 = new List<string> { guiltyNames[1], guiltyNames[3], guiltyNames[5], guiltyNames[7] };

            if (group1.Contains(guilty))
            {
                clues.Insert(0, "Tiene los ojos marrones");
            }
            else if (group2.Contains(guilty))
            {
                clues.Insert(0, "Tiene los ojos verdes");
            }

            if (group3.Contains(guilty))
            {
                clues.Insert(1, "Mechón de pelo negro");
            }
            else if (group4.Contains(guilty))
            {
                clues.Insert(1, "Mechón de pelo rubio");
            }

            if (group5.Contains(guilty))
            {
                clues.Insert(2, "Tiene una cicatriz");
            }
            else if (group6.Contains(guilty))
            {
                clues.Insert(2, "Tiene un pendiente");
            }
        }        
    }

    // QUITAR ---------------------------------------------------------
    private void FoundGamePhaseAux(GameData gameData)
    {
        if(gameData != null)
        {
            if(gameData.gameStoryPhaseAux != 0)
            {
                storyPhaseAux = gameData.gameStoryPhaseAux;
            }
            else
            {
                storyPhaseAux = 0;
            }
        }
        else
        {
            storyPhaseAux = 0;
        }
    }
    // -----------------------------------------------------------------

    // Método para cargar la información de la fase de historia en la que se encuentra el jugador o asignaer la primera si no existe
    private void FoundGamePhase(GameData gameData)
    {
        if(gameData != null && gameData.gameStoryPhase != null && gameData.gameStoryPhase.storySubphases != null 
            && gameData.gameStoryPhase.storySubphases.Count > 0)
        {
            storyPhase = gameData.gameStoryPhase;
        }
        else
        {
            storyPhase = StoryStateManager.CreateFirstPhase();
        }
    }

    // Método para cargar la información de apartados que conoce el jugador o asignar los valores por defecto en caso contrario
    private void FoundKnownInformation(GameData gameData)
    {
        if(gameData != null && gameData.gameKnownSuspects?.Length > 0)
        {
            knownSuspects = gameData.gameKnownSuspects;
        }
        else
        {
            knownSuspects = new bool[8];
        }

        if(gameData != null && gameData.gameKnownTutorials?.Length > 0)
        {
            knownTutorials = gameData.gameKnownTutorials;
        }
        else
        {
            knownTutorials = new bool[8];
        }

        if(gameData != null && gameData.gameKnownDialogues?.Length > 0)
        {
            knownDialogues = gameData.gameKnownDialogues;
        }
        else
        {
            knownDialogues = new bool[8];
            knownDialogues[0] = true;
        }
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
        if(puzzleData != null && puzzleData.gamePuzzleStates != null && puzzleData.gamePuzzleStates.Count > 0)
        {
            puzzleStateList = puzzleData.gamePuzzleStates;
            PuzzleState lastCompletedPuzzle = puzzleData.gamePuzzleStates.Where(p => p.gameIsPuzzleComplete).LastOrDefault();
            lastPuzzleComplete = lastCompletedPuzzle != null ? lastCompletedPuzzle.gamePuzzleName : "";
        }
        else
        {
            puzzleStateList = new List<PuzzleState>();
            lastPuzzleComplete = "";
        }
    }

    // Método que se llama cuando se activa el objeto y sirve para ejecutar OnSceneLoaded cuando se carga una nueva escena
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Método que se llama cuando se desactiva o se destruye el objeto y sirve para evitar llamadas innecesarias al OnSceneLoaded
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Método que se ejecuta cada vez que se carga una escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Se destruye el objeto donde está este script cuando se llega a una escena en concreto
        if (scene.name == sceneToDestroy)
        {
            Destroy(gameObject);
        }

        // Se activa el NPC correspondiente en función del final del juego
        if (scene.name == GameStateManager.Instance.MainScene)
        {
            GameData gameData = SaveManager.LoadGameData();
            if(gameData != null && gameData.gameStoryPhaseAux >= 200 && gameData.gameStoryPhaseAux < 400)
            {
                GameObject victim = GameObject.Find("Victim");
                victim.transform.GetChild(0).gameObject.SetActive(true);
            }
            else if(gameData != null && gameData.gameStoryPhaseAux >= 400)
            {
                GameObject father = GameObject.Find("Father");
                father.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
