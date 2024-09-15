using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    private GameObject player;
    private string sceneName;
    
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
        SaveManager.SaveGameData(sceneName);

        Debug.Log("Datos guardados");
    }

    public void LoadData()
    {
        GameData gameData = SaveManager.LoadGameData();
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
