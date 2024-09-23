using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public bool isPuzzleRecentlyCompleted;
    public float[] actualPlayerPosition;
    public float[] actualCameraPosition;

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
        SaveManager.SaveGameData(sceneName, guilty, firstClue, secondClue, thirdClue, storyPhase, lastPuzzleComplete, 
            knownSuspects, knownTutorials, knownDialogues);

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
        SceneManager.LoadScene(gameData.gameScene);

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
        Debug.Log("Datos reseteados");
    }
}
