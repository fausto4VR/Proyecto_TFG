using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject continueButtonDisabled;
    [SerializeField] private GameObject resetGamePanel;
    [SerializeField] private GameObject settingsGamePanel;
    [SerializeField] private GameObject audioSourcesManager;

    private bool isResetGamePanelShown;
    private AudioSource buttonsAudioSource;

    void Start()
    {
        UnlockContinueButton();
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        buttonsAudioSource = audioSources[0];
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            GameStateManager.Instance.ResetData();
        }
    }

    public void NewGame()
    {
        if(!isResetGamePanelShown)
        {
            GameData gameData = SaveManager.LoadGameData();

            if(gameData != null)
            {
                // if(gameData.gameStoryPhaseAux != 0)
                if(gameData.gameStoryPhase != null)
                {                    
                    buttonsAudioSource.Play();
                    isResetGamePanelShown = true;
                    resetGamePanel.SetActive(true);
                }
                //else if(gameData.gameStoryPhaseAux == 0)
                else if(gameData.gameStoryPhase == null)
                {
                    buttonsAudioSource.Play();
                    StartCoroutine(WaitForSoundAndLoadNewScene());        
                }
            }
            else
            {
                GameStateManager.Instance.isNewGame = true;
                SceneManager.LoadScene(GameStateManager.Instance.MainScene);            
            }
        }  
    }

    public void ContinueGame()
    {
        if(!isResetGamePanelShown)
        {
            buttonsAudioSource.Play();
            StartCoroutine(WaitForSoundAndLoadContinueScene());
        } 
    }

    public void ExitGame()
    {
        if(!isResetGamePanelShown)
        {
            buttonsAudioSource.Play();
            StartCoroutine(WaitForSoundAndLoadQuitGame());
        }        
    }

    public void ResetGame()
    {   
        buttonsAudioSource.Play();
        StartCoroutine(WaitForSoundAndLoadResetScene());
    }

    public void BackFromResetPanel()
    {
        buttonsAudioSource.Play();
        resetGamePanel.SetActive(false);
        isResetGamePanelShown = false;
    }

    private void UnlockContinueButton()
    {
        GameData gameData = SaveManager.LoadGameData();

        if(gameData != null)
        {
            // if(gameData.gameStoryPhaseAux != 0)
            if(gameData.gameStoryPhase != null)
            {
                continueButtonDisabled.SetActive(false);
                continueButton.SetActive(true);
            }
            // else if(gameData.gameStoryPhaseAux == 0)
            else if(gameData.gameStoryPhase == null)
            {
                continueButton.SetActive(false);
                continueButtonDisabled.SetActive(true);
            }
        }
        else
        {
            continueButton.SetActive(false);
            continueButtonDisabled.SetActive(true);
        }
    }

    public void DisplaySettingsPanel()
    {
        buttonsAudioSource.Play();

        if(settingsGamePanel.activeInHierarchy == true)
        {
            settingsGamePanel.SetActive(false);
        }
        else if(settingsGamePanel.activeInHierarchy == false)
        {
            settingsGamePanel.SetActive(true);
        }
    }

    private IEnumerator WaitForSoundAndLoadContinueScene()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        GameData gameData = SaveManager.LoadGameData();

        if(gameData != null)
        {
            GameStateManager.Instance.isLoadGame = true;
            SceneManager.LoadScene(gameData.gameScene);
        }
    }

    private IEnumerator WaitForSoundAndLoadNewScene()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        GameStateManager.Instance.isNewGame = true;
        SceneManager.LoadScene(GameStateManager.Instance.MainScene);    
    }

    private IEnumerator WaitForSoundAndLoadResetScene()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        
        GameStateManager.Instance.ResetData();
        isResetGamePanelShown = false;
        GameStateManager.Instance.isNewGame = true;
        SceneManager.LoadScene(GameStateManager.Instance.MainScene);
    }

    private IEnumerator WaitForSoundAndLoadQuitGame()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        Application.Quit();
    }
}
