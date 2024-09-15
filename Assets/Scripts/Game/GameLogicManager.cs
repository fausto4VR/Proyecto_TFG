using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogicManager : MonoBehaviour
{
    public static GameLogicManager Instance;
    private int timer = 3000;

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

    void Update()
    {
        timer -= 1;
        if(timer == 0)
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}
