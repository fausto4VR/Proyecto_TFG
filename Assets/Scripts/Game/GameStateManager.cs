using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    private GameObject player;
    private string sceneName;
    private string guilty;
    private string firstClue = "";
    private string secondClue = "";
    private string thirdClue = "";
    
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
        SaveManager.SaveGameData(sceneName, guilty, firstClue, secondClue, thirdClue);

        Debug.Log("Datos guardados");
    }

    public void LoadData()
    {
        GameData gameData = SaveManager.LoadGameData();
        GameLogicManager.Instance.guilty = gameData.gameGuilty;
        GameLogicManager.Instance.firstClue = gameData.gameFirstClue;
        GameLogicManager.Instance.secondClue = gameData.gameSecondClue;
        GameLogicManager.Instance.thirdClue = gameData.gameThirdClue;
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
    }

    public void ResetData()
    {
        SaveManager.ResetPlayerData();
        SaveManager.ResetGameData();
        Debug.Log("Datos reseteados");
    }
}
