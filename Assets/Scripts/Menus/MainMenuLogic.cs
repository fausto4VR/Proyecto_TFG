using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    [Header("UI Objects Section")]
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject continueButtonDisabled;
    [SerializeField] private GameObject resetGamePanel;
    [SerializeField] private GameObject settingsGamePanel;    
    [SerializeField] private GameObject outSettingPanelDetectionButton;
    [SerializeField] private GameObject outResetPanelDetectionButton;

    [Header("Cursors Textures Section")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D interactCursor;
    
    [Header("Variable Section")]
    [SerializeField] private Vector2 hotspot = Vector2.zero;
    
    [Header("Audio Section")]
    [SerializeField] private GameObject audioSourcesManager;

    private bool isPanelShown;

    // REVISAR AUDIO
    private AudioSource buttonsAudioSource;

    
    void Start()
    {
        UnlockContinueButton();
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        buttonsAudioSource = audioSources[0];
    }

    /*
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            GameStateManager.Instance.ResetData();
        }
    }
    */  

    // Método para obtener la textura necesaria del cursor por defecto
    public Texture2D DefaultCursor
    {
        get { return defaultCursor; }
    }

    // Método para obtener la textura necesaria del cursor cuando puede interactuar con un elemento
    public Texture2D InteractCursor
    {
        get { return interactCursor; }
    }

    // Método que efectua el cambio de cursor
    public void SetCursor(Texture2D cursorTexture)
    {
        if (cursorTexture != null)
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    // Método para gestionar si se muestra el botón de continuar habilitado o no
    private void UnlockContinueButton()
    {
        GameData gameData = SaveManager.LoadGameData();

        if(gameData != null && gameData.isGameStarted)
        {
            continueButtonDisabled.SetActive(false);
            continueButton.SetActive(true);
            GameStateManager.Instance.IsGameStarted = true;
        }
        else
        {
            continueButton.SetActive(false);
            continueButtonDisabled.SetActive(true);
        }
    }

    // Método para gestionar el empezar una nueva partida
    public void NewGame()
    {
        if(!isPanelShown)
        {
            GameData gameData = SaveManager.LoadGameData();

            if(gameData != null && gameData.isGameStarted)
            {
                buttonsAudioSource.Play();
                outResetPanelDetectionButton.SetActive(true);
                resetGamePanel.SetActive(true);
                isPanelShown = true;
            }
            else
            {
                buttonsAudioSource.Play();
                StartCoroutine(WaitForSoundAndLoadNewScene()); 
            }
        }  
    }

    // Corrutina para esperar que suene el audio del botón y se empiece una nueva partida
    private IEnumerator WaitForSoundAndLoadNewScene()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);

        SetCursor(DefaultCursor);

        GameStateManager.Instance.IsNewGame = true;
        isPanelShown = false;

        SceneManager.LoadScene(GameStateManager.Instance.MainScene);    
    }

    // Método para gestionar el reiniciar una partida
    public void ResetGame()
    {   
        buttonsAudioSource.Play();
        StartCoroutine(WaitForSoundAndLoadResetScene());
    }

    // Corrutina para esperar que suene el audio del botón y se reinicie una partida
    private IEnumerator WaitForSoundAndLoadResetScene()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        
        GameStateManager.Instance.ResetData();

        SetCursor(DefaultCursor);
        outResetPanelDetectionButton.SetActive(false);

        GameStateManager.Instance.IsNewGame = true;
        isPanelShown = false;

        SceneManager.LoadScene(GameStateManager.Instance.MainScene);
    }

    // Método para gestionar el volver del panel de reiniciar una partida
    public void BackFromResetPanel()
    {
        buttonsAudioSource.Play();
        outResetPanelDetectionButton.SetActive(false);
        resetGamePanel.SetActive(false);
        isPanelShown = false;
    }

   // Método para gestionar el continuar una partida
    public void ContinueGame()
    {
        if(!isPanelShown)
        {
            buttonsAudioSource.Play();
            StartCoroutine(WaitForSoundAndLoadContinueScene());
        } 
    }

    // Corrutina para esperar que suene el audio del botón y se continue con la partida
    private IEnumerator WaitForSoundAndLoadContinueScene()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);

        GameData gameData = SaveManager.LoadGameData();

        if(gameData != null)
        {
            SetCursor(DefaultCursor);
            
            GameStateManager.Instance.IsLoadGame = true;

            SceneManager.LoadScene(gameData.gameScene);
        }
        else
        {
            Debug.LogError("Ha habido un error al intentar continua con la partida.");
        }
    }

   // Método para gestionar el salir del juego
    public void ExitGame()
    {
        if(!isPanelShown)
        {
            buttonsAudioSource.Play();
            StartCoroutine(WaitForSoundAndLoadQuitGame());
        }        
    }

    // Corrutina para esperar que suene el audio del botón y se salga del juego
    private IEnumerator WaitForSoundAndLoadQuitGame()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);

        Application.Quit();
    }

    // Método para mostrar el panel de configuración
    public void DisplaySettingsPanel(bool isButtonSetting)
    {
        buttonsAudioSource.Play();

        if(settingsGamePanel.activeInHierarchy == true)
        {
            outSettingPanelDetectionButton.SetActive(false);
            settingsGamePanel.SetActive(false);
            if (isButtonSetting) SetCursor(InteractCursor);
        } 
        else 
        {
            outSettingPanelDetectionButton.SetActive(true);
            settingsGamePanel.SetActive(true);
        }
    }
}
