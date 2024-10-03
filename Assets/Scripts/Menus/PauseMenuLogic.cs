using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuLogic : MonoBehaviour
{
    [SerializeField] private GameObject briefcaseIconButton;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private TMP_Text objectiveBodyText;
    [SerializeField, TextArea(4,5)] private string[] objectiveTextPhases;
    [SerializeField] private GameObject afterSavePanel;
    [SerializeField] private TMP_Text afterSaveText;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private GameObject cluesPanel;
    [SerializeField] private Image firstClueImage;
    [SerializeField] private Image secondClueImage;
    [SerializeField] private Image thirdClueImage;
    [SerializeField] private Sprite firstClueSprite1;    
    [SerializeField] private Sprite firstClueSprite2;
    [SerializeField] private Sprite secondClueSprite1;
    [SerializeField] private Sprite secondClueSprite2;
    [SerializeField] private Sprite thirdClueSprite1;
    [SerializeField] private Sprite thirdClueSprite2;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private TMP_Text firstClueText;
    [SerializeField] private TMP_Text secondClueText;
    [SerializeField] private TMP_Text thirdClueText;
    [SerializeField] private GameObject suspectsPanel;
    [SerializeField] private Image suspectImage1;
    [SerializeField] private Sprite suspectSprite1;
    [SerializeField] private TMP_Text suspectText1;
    [SerializeField] private Image suspectImage2;
    [SerializeField] private Sprite suspectSprite2;
    [SerializeField] private TMP_Text suspectText2;
    [SerializeField] private Image suspectImage3;
    [SerializeField] private Sprite suspectSprite3;
    [SerializeField] private TMP_Text suspectText3;
    [SerializeField] private Image suspectImage4;
    [SerializeField] private Sprite suspectSprite4;
    [SerializeField] private TMP_Text suspectText4;
    [SerializeField] private Image suspectImage5;
    [SerializeField] private Sprite suspectSprite5;
    [SerializeField] private TMP_Text suspectText5;
    [SerializeField] private Image suspectImage6;
    [SerializeField] private Sprite suspectSprite6;
    [SerializeField] private TMP_Text suspectText6;
    [SerializeField] private Image suspectImage7;
    [SerializeField] private Sprite suspectSprite7;
    [SerializeField] private TMP_Text suspectText7;
    [SerializeField] private Image suspectImage8;
    [SerializeField] private Sprite suspectSprite8;
    [SerializeField] private TMP_Text suspectText8;
    [SerializeField] private GameObject audioSourcesManager;

    private bool isPanelShown;
    private int lastStoryPhase;
    private bool isAfterSavePanelShown;
    private bool isExitPanelShown;
    private bool isCluesPanelShown;
    private bool isSuspectsPanelShown;
    private List<Image> suspectImages = new List<Image>();
    private List<Sprite> suspectSprites = new List<Sprite>();
    private List<TMP_Text> suspectTexts = new List<TMP_Text>();
    private AudioSource pauseAudioSource;
    private AudioSource buttonsAudioSource;

    void Start() 
    {        
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        pauseAudioSource = audioSources[6];
        buttonsAudioSource = audioSources[1];

        lastStoryPhase = GameLogicManager.Instance.storyPhase;

        suspectImages.Add(suspectImage1);
        suspectImages.Add(suspectImage2);
        suspectImages.Add(suspectImage3);
        suspectImages.Add(suspectImage4);
        suspectImages.Add(suspectImage5);
        suspectImages.Add(suspectImage6);
        suspectImages.Add(suspectImage7);
        suspectImages.Add(suspectImage8);

        suspectSprites.Add(suspectSprite1);
        suspectSprites.Add(suspectSprite2);
        suspectSprites.Add(suspectSprite3);
        suspectSprites.Add(suspectSprite4);
        suspectSprites.Add(suspectSprite5);
        suspectSprites.Add(suspectSprite6);
        suspectSprites.Add(suspectSprite7);
        suspectSprites.Add(suspectSprite8);

        suspectTexts.Add(suspectText1);
        suspectTexts.Add(suspectText2);
        suspectTexts.Add(suspectText3);
        suspectTexts.Add(suspectText4);
        suspectTexts.Add(suspectText5);
        suspectTexts.Add(suspectText6);
        suspectTexts.Add(suspectText7);
        suspectTexts.Add(suspectText8);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            DisplayMenuPanel();
        }

        if(lastStoryPhase + 1 == GameLogicManager.Instance.storyPhase)
        {
            if(lastStoryPhase < objectiveTextPhases.Length)
            {
                objectiveBodyText.text = objectiveTextPhases[lastStoryPhase];
                lastStoryPhase++;
            }
        }

        if(isPanelShown && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)))
        {
            DisplayCluesBoard();
        }

        if(isPanelShown && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)))
        {
            DisplaySuspectsBoard();
        }

        if(isPanelShown && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)))
        {
            SaveGame();
        }

        if(isPanelShown && (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)))
        {
            GoToMainMenuPanel();
        }

        if(isAfterSavePanelShown && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            buttonsAudioSource.Play();
            afterSavePanel.SetActive(false);
            isAfterSavePanelShown = false;
        }

        if(isExitPanelShown && Input.GetKeyDown(KeyCode.Y))
        {
            GoToMainMenu();
        }

        if(isExitPanelShown && Input.GetKeyDown(KeyCode.N))
        {
            GoBackFromExitPanel();
        }

        if(isCluesPanelShown && Input.GetKeyDown(KeyCode.Space))
        {
            GoBackFromCluesPanel();
        }

        if(isSuspectsPanelShown && Input.GetKeyDown(KeyCode.Space))
        {
            GoBackFromSuspectsPanel();
        }
    }

    public void DisplayMenuPanel()
    {
        if(pauseMenuPanel.activeInHierarchy == true && isPanelShown)
        {
            pauseAudioSource.Play();

            pauseMenuPanel.SetActive(false);
            briefcaseIconButton.SetActive(true);
            isPanelShown = false;
            GetComponent<PlayerMovement>().isPlayerInPause = false;

            afterSavePanel.SetActive(false);
            isAfterSavePanelShown = false;

            exitPanel.SetActive(false);
            isExitPanelShown = false;
            
            cluesPanel.SetActive(false);
            isCluesPanelShown = false;

            suspectsPanel.SetActive(false);
            isSuspectsPanelShown = false;
        }
        else if(pauseMenuPanel.activeInHierarchy == false && !isPanelShown)
        {
            if(!GetComponent<PlayerMovement>().isPlayerTalking && !GetComponent<PlayerMovement>().isPlayerInspecting 
                && !GetComponent<PlayerMovement>().isPlayerDoingTutorial)
            {
                pauseAudioSource.Play();
                
                pauseMenuPanel.SetActive(true);
                briefcaseIconButton.SetActive(false);
                isPanelShown = true;                
                GetComponent<PlayerMovement>().isPlayerInPause = true;
            }
        }
    }

    public void SaveGame()
    {
        if(isPanelShown && !isAfterSavePanelShown && !isExitPanelShown && !isCluesPanelShown && !isSuspectsPanelShown)
        {
            buttonsAudioSource.Play();
            isAfterSavePanelShown = true;
            GameStateManager.Instance.SaveData();
            afterSavePanel.SetActive(true);
            GameData gameData = SaveManager.LoadGameData();

            if(gameData != null)
            {
                if(GameLogicManager.Instance.storyPhase == gameData.gameStoryPhase)
                {
                    afterSaveText.text = "La partida se ha guardado correctamente.";
                }
                else
                {
                    afterSaveText.text = "Ha habido un error al guardar la partida. Inténtalo otra vez.";
                }
            }
            else
            {
                afterSaveText.text = "Ha habido un error al guardar la partida. Inténtalo otra vez.";
            }
        }
        else if(isAfterSavePanelShown)
        {
            buttonsAudioSource.Play();
            afterSavePanel.SetActive(false);
            isAfterSavePanelShown = false;
        }
    }

    public void GoToMainMenuPanel()
    {
        if(isPanelShown && !isAfterSavePanelShown && !isExitPanelShown && !isCluesPanelShown && !isSuspectsPanelShown)
        {
            buttonsAudioSource.Play();
            isExitPanelShown = true;            
            exitPanel.SetActive(true);
        }
        else if(isExitPanelShown)
        {
            GoBackFromExitPanel();
        }
    }

    public void GoToMainMenu()
    {        
        buttonsAudioSource.Play();        
        StartCoroutine(WaitForSoundAndGoMenu()); 
    }

    public void GoBackFromExitPanel()
    {        
        buttonsAudioSource.Play();
        isExitPanelShown = false;            
        exitPanel.SetActive(false);
    }

    public void DisplayCluesBoard()
    {
        if(isPanelShown && !isAfterSavePanelShown && !isExitPanelShown && !isCluesPanelShown && !isSuspectsPanelShown)
        {
            
            buttonsAudioSource.Play();
            isCluesPanelShown = true;            
            cluesPanel.SetActive(true);
            UnlockFirstClue();
            UnlockSecondClue();
            UnlockThirdClue();
        }
        else if(isCluesPanelShown)
        {
            GoBackFromCluesPanel();
        }
    }

    public void GoBackFromCluesPanel()
    {        
        buttonsAudioSource.Play();
        isCluesPanelShown = false;            
        cluesPanel.SetActive(false);
    }

    private void UnlockFirstClue()
    {
        if(!string.IsNullOrEmpty(GameLogicManager.Instance.firstClue))
        {
            if(GameLogicManager.Instance.firstClue == "Tiene los ojos marrones")
            {
                firstClueImage.sprite = firstClueSprite1;
                firstClueText.text = "Tiene los ojos marrones";
            }
            else if(GameLogicManager.Instance.firstClue == "Tiene los ojos verdes")
            {
                firstClueImage.sprite = firstClueSprite2;
                firstClueText.text = "Tiene los ojos verdes";
            }
            else
            {
                firstClueImage.sprite = defaultSprite;
                firstClueText.text = "Desconocido";
            }
        }
        else
        {
            firstClueImage.sprite = defaultSprite;
            firstClueText.text = "Desconocido";
        }
    }

    private void UnlockSecondClue()
    {
        if(!string.IsNullOrEmpty(GameLogicManager.Instance.secondClue))
        {
            if(GameLogicManager.Instance.secondClue == "Mechón de pelo negro")
            {
                secondClueImage.sprite = secondClueSprite1;
                secondClueText.text = "Mechón de pelo negro";
            }
            else if(GameLogicManager.Instance.secondClue == "Mechón de pelo rubio")
            {
                secondClueImage.sprite = secondClueSprite2;
                secondClueText.text = "Mechón de pelo rubio";
            }
            else
            {
                secondClueImage.sprite = defaultSprite;
                secondClueText.text = "Desconocido";
            }
        }
        else
        {
            secondClueImage.sprite = defaultSprite;
            secondClueText.text = "Desconocido";
        }
    }

    private void UnlockThirdClue()
    {
        if(!string.IsNullOrEmpty(GameLogicManager.Instance.thirdClue))
        {
            if(GameLogicManager.Instance.thirdClue == "Tiene una cicatriz")
            {
                thirdClueImage.sprite = thirdClueSprite1;
                thirdClueText.text = "Tiene una cicatriz";
            }
            else if(GameLogicManager.Instance.thirdClue == "Tiene un pendiente")
            {
                thirdClueImage.sprite = thirdClueSprite2;
                thirdClueText.text = "Tiene un pendiente";
            }
            else
            {
                thirdClueImage.sprite = defaultSprite;
                thirdClueText.text = "Desconocido";
            }
        }
        else
        {
            thirdClueImage.sprite = defaultSprite;
            thirdClueText.text = "Desconocido";
        }
    }

    public void DisplaySuspectsBoard()
    {
        if(isPanelShown && !isAfterSavePanelShown && !isExitPanelShown && !isCluesPanelShown && !isSuspectsPanelShown)
        {            
            buttonsAudioSource.Play();
            isSuspectsPanelShown = true;            
            suspectsPanel.SetActive(true);
            UnlockSuspect();
        }
        else if(isSuspectsPanelShown)
        {
            GoBackFromSuspectsPanel();
        }
    }

    public void GoBackFromSuspectsPanel()
    {        
        buttonsAudioSource.Play();
        isSuspectsPanelShown = false;            
        suspectsPanel.SetActive(false);
    }

    public void UnlockSuspect()
    {
        for(int i=0; i<GameLogicManager.Instance.guiltyNames.Count; i++)
        {
            if(GameLogicManager.Instance.knownSuspects[i])
            {
                suspectImages[i].sprite = suspectSprites[i];
                suspectTexts[i].text = GameLogicManager.Instance.guiltyNames[i];
            }
            else
            {
                suspectImages[i].sprite = defaultSprite;
                suspectTexts[i].text = "Desconocido";
            }
        }
    }
    private IEnumerator WaitForSoundAndGoMenu()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        
        pauseMenuPanel.SetActive(false);
        briefcaseIconButton.SetActive(true);
        isPanelShown = false;
        GetComponent<PlayerMovement>().isPlayerInPause = false;

        afterSavePanel.SetActive(false);
        isAfterSavePanelShown = false;

        exitPanel.SetActive(false);
        isExitPanelShown = false;
            
        cluesPanel.SetActive(false);
        isCluesPanelShown = false;

        suspectsPanel.SetActive(false);
        isSuspectsPanelShown = false;
              
        SceneManager.LoadScene("MenuScene");
    }
}
