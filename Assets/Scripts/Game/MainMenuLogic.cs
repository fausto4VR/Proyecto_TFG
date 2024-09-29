using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject continueButtonDisabled;
    [SerializeField] private GameObject resetGamePanel;
    [SerializeField] private GameObject settingsGamePanel;

    private bool isResetGamePanelShown;

    void Start()
    {
        UnlockContinueButton();
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
                if(gameData.gameStoryPhase != 0)
                {
                    isResetGamePanelShown = true;
                    resetGamePanel.SetActive(true);
                }
                else if(gameData.gameStoryPhase == 0)
                {
                    GameStateManager.Instance.isNewGame = true;
                    SceneManager.LoadScene("SampleScene");            
                }
            }
            else
            {
                GameStateManager.Instance.isNewGame = true;
                SceneManager.LoadScene("SampleScene");            
            }
        }  
    }

    public void ContinueGame()
    {
        if(!isResetGamePanelShown)
        {
            GameData gameData = SaveManager.LoadGameData();

            if(gameData != null)
            {
                GameStateManager.Instance.isLoadGame = true;
                SceneManager.LoadScene(gameData.gameScene);
            }
        } 
    }

    public void ExitGame()
    {
        if(!isResetGamePanelShown)
        {
            Application.Quit();
        }        
    }

    public void ResetGame()
    {
        GameStateManager.Instance.ResetData();
        isResetGamePanelShown = false;
        GameStateManager.Instance.isNewGame = true;
        SceneManager.LoadScene("SampleScene");
    }

    public void BackFromResetPanel()
    {
        resetGamePanel.SetActive(false);
        isResetGamePanelShown = false;
    }

    private void UnlockContinueButton()
    {
        GameData gameData = SaveManager.LoadGameData();

        if(gameData != null)
        {
            if(gameData.gameStoryPhase != 0)
            {
                continueButtonDisabled.SetActive(false);
                continueButton.SetActive(true);
            }
            else if(gameData.gameStoryPhase == 0)
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
        if(settingsGamePanel.activeInHierarchy == true)
        {
            settingsGamePanel.SetActive(false);
        }
        else if(settingsGamePanel.activeInHierarchy == false)
        {
            settingsGamePanel.SetActive(true);
        }
    }
}
