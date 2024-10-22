using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PuzzleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private GameObject skipPanel;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject supportSectionPanel;
    [SerializeField] private GameObject firstSupportButton;
    [SerializeField] private GameObject firstSupportPanelButton;
    [SerializeField] private GameObject secondSupportButton;
    [SerializeField] private GameObject secondSupportPanelButton;
    [SerializeField] private GameObject thirdSupportButton;
    [SerializeField] private GameObject thirdSupportPanelButton;
    [SerializeField] private GameObject supportUnlockButton;
    [SerializeField] private GameObject secondSupportWarningText;
    [SerializeField] private GameObject thirdSupportWarningText;   
    [SerializeField] private GameObject supportText;   
    [SerializeField] private GameObject successsPanel;
    [SerializeField] private GameObject failurePanel;
    [SerializeField] private TMP_Text pointsSuccessText;
    [SerializeField] private TMP_Text pointsFailureText;    
    [SerializeField] private GameObject audioSourcesManager;    
    [SerializeField] private AudioSource puzzleMusic;
    [SerializeField] private GameObject outPanelDetectionButton;  

    public string firstSupportText;    
    public string secondSupportText;    
    public string thirdSupportText;
    public bool isPanelShown;
    public bool isCheckTrigger;
    public int isCorrectResult;
    public bool isSuccessPanelShown;    
    public bool isFailurePanelShown;
    public bool isReturnToPuzzleAfterFail;    
    public bool isNecesaryResetInputs;
    public bool isPuzzleSkipped;

    private int supportIndex;
    private AudioSource buttonsAudioSource;
    private AudioSource unlockSupportAudioSource;
    private AudioSource successSolutionAudioSource;
    private AudioSource failureSolutionAudioSource;
    private AudioSource accessDeniedudioSource;
    private bool isFirstSupportShown;
    private bool isSecondSupportShown;
    private bool isThirdSupportShown;

    void Start()
    {
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        buttonsAudioSource = audioSources[1];
        unlockSupportAudioSource = audioSources[2];
        successSolutionAudioSource = audioSources[3];
        failureSolutionAudioSource = audioSources[4];
        accessDeniedudioSource = audioSources[5];

        Color customColor = new Color(0.525f, 1f, 0.518f, 1f);

        if(GetComponent<PuzzleLogicManager>().puzzleSupports[0])
        {
            firstSupportButton.GetComponent<Image>().color = customColor;
            firstSupportPanelButton.GetComponent<Image>().color = customColor;
        }

        if(GetComponent<PuzzleLogicManager>().puzzleSupports[1])
        {
            secondSupportButton.GetComponent<Image>().color = customColor;
            secondSupportPanelButton.GetComponent<Image>().color = customColor;
        }

        if(GetComponent<PuzzleLogicManager>().puzzleSupports[2])
        {
            thirdSupportButton.GetComponent<Image>().color = customColor;
            thirdSupportPanelButton.GetComponent<Image>().color = customColor;
        }

        // Para esperar que esté inicializada la lista de puzzleSupports
        StartCoroutine(ExecuteAfterDelay());
    }

    void Update()
    {
        if(isCorrectResult == 1)
        {
            ShowSuccess();
        }
        else if(isCorrectResult == 2)
        {
            ShowFailure();
        }

        if(isReturnToPuzzleAfterFail)
        {
            failureSolutionAudioSource.Stop();
            puzzleMusic.Play();
            failurePanel.SetActive(false);
            isFailurePanelShown = false;
            isReturnToPuzzleAfterFail = false;
            isNecesaryResetInputs = true;
        }
    }

    private void ShowSuccess()
    {
        puzzleMusic.Stop();
        successSolutionAudioSource.Play();
        successsPanel.SetActive(true);
        isSuccessPanelShown = true;
        int pointsToShow = GetComponent<PuzzleLogicManager>().CalculatePoints(true);
        pointsSuccessText.text = pointsToShow + "/50";

        isCorrectResult = 0;
    }

    private void ShowFailure()
    {
        puzzleMusic.Stop();
        failureSolutionAudioSource.Play();
        failurePanel.SetActive(true);
        isFailurePanelShown = true;
        int pointsToShow = GetComponent<PuzzleLogicManager>().CalculatePoints(false);
        pointsFailureText.text = pointsToShow + "/50";

        isCorrectResult = 0;
    }

    public void UnlockSupport()
    {
        Color customColor = new Color(0.525f, 1f, 0.518f, 1f);

        if(supportIndex == 0)
        {
            GetComponent<PuzzleLogicManager>().CalculatePoints(false);
            unlockSupportAudioSource.Play();
            firstSupportButton.GetComponent<Image>().color = customColor;
            firstSupportPanelButton.GetComponent<Image>().color = customColor;
            supportText.GetComponent<TMP_Text>().text = firstSupportText;
            GetComponent<PuzzleLogicManager>().puzzleSupports[supportIndex] = true;
            supportUnlockButton.SetActive(false);
            supportText.SetActive(true);
        }

        if(supportIndex == 1)
        {
            if(GetComponent<PuzzleLogicManager>().puzzleSupports[0])
            {
                GetComponent<PuzzleLogicManager>().CalculatePoints(false);
                unlockSupportAudioSource.Play();
                secondSupportButton.GetComponent<Image>().color = customColor;
                secondSupportPanelButton.GetComponent<Image>().color = customColor;
                supportText.GetComponent<TMP_Text>().text = secondSupportText;
                GetComponent<PuzzleLogicManager>().puzzleSupports[supportIndex] = true;
                supportUnlockButton.SetActive(false);
                supportText.SetActive(true);
            }
            else
            {
                accessDeniedudioSource.Play();
                secondSupportWarningText.SetActive(true);
            }
        }

        if(supportIndex == 2)
        {
            if(GetComponent<PuzzleLogicManager>().puzzleSupports[0] && GetComponent<PuzzleLogicManager>().puzzleSupports[1])
            {
                GetComponent<PuzzleLogicManager>().CalculatePoints(false);
                unlockSupportAudioSource.Play();
                thirdSupportButton.GetComponent<Image>().color = customColor;
                thirdSupportPanelButton.GetComponent<Image>().color = customColor;
                supportText.GetComponent<TMP_Text>().text = thirdSupportText;
                GetComponent<PuzzleLogicManager>().puzzleSupports[supportIndex] = true;
                supportUnlockButton.SetActive(false);
                supportText.SetActive(true);
                skipButton.SetActive(true);
            }
            else
            {
                accessDeniedudioSource.Play();
                thirdSupportWarningText.SetActive(true);
            }
        }
    }

    public void DisplayHelpPanel()
    {
        buttonsAudioSource.Play();

        if(helpPanel.activeInHierarchy == true && isPanelShown)
        {
            helpPanel.SetActive(false);
            isPanelShown = false;
            outPanelDetectionButton.SetActive(false); 
        }
        else if(helpPanel.activeInHierarchy == false && !isPanelShown)
        {
            helpPanel.SetActive(true);
            outPanelDetectionButton.SetActive(true);
            isPanelShown = true;
        }
    }

    public void DisplaySkipPanel()
    {
        buttonsAudioSource.Play();

        if(skipPanel.activeInHierarchy == true && isPanelShown)
        {
            skipPanel.SetActive(false);
            isPanelShown = false;
            outPanelDetectionButton.SetActive(false); 
        }
        else if(skipPanel.activeInHierarchy == false && !isPanelShown)
        {
            outPanelDetectionButton.SetActive(true);
            skipPanel.SetActive(true);
            isPanelShown = true;
        }
    }

    public void SkipPuzzle()
    {
        buttonsAudioSource.Play();
        StartCoroutine(WaitForSoundAndSkip());
    }

    public void DisplayFirstSupportPanel()
    {
        buttonsAudioSource.Play();

        if(supportSectionPanel.activeInHierarchy == true && isPanelShown)
        {
            supportSectionPanel.SetActive(false);
            firstSupportPanelButton.SetActive(false);
            supportUnlockButton.SetActive(false);
            supportText.SetActive(false);
            isPanelShown = false; 
            outPanelDetectionButton.SetActive(false); 
        }
        else if(supportSectionPanel.activeInHierarchy == false && !isPanelShown)
        {
            supportSectionPanel.SetActive(true);
            firstSupportPanelButton.SetActive(true);
            outPanelDetectionButton.SetActive(true);
            isFirstSupportShown = true;

            if(!GetComponent<PuzzleLogicManager>().puzzleSupports[0])
            {
                supportIndex = 0;
                supportUnlockButton.SetActive(true);               
            }
            else if(GetComponent<PuzzleLogicManager>().puzzleSupports[0])
            {
                supportText.SetActive(true);
                supportText.GetComponent<TMP_Text>().text = firstSupportText;
            }

            isPanelShown = true;
        }
    }

    public void DisplaySecondSupportPanel()
    {
        buttonsAudioSource.Play();

        if(supportSectionPanel.activeInHierarchy == true && isPanelShown)
        {
            supportSectionPanel.SetActive(false);
            secondSupportPanelButton.SetActive(false);
            supportUnlockButton.SetActive(false);
            supportText.SetActive(false);
            secondSupportWarningText.SetActive(false);
            isPanelShown = false; 
            outPanelDetectionButton.SetActive(false); 
        }
        else if(supportSectionPanel.activeInHierarchy == false && !isPanelShown)
        {
            supportSectionPanel.SetActive(true);
            secondSupportPanelButton.SetActive(true);
            outPanelDetectionButton.SetActive(true);
            isSecondSupportShown = true;

            if(!GetComponent<PuzzleLogicManager>().puzzleSupports[1])
            {
                supportIndex = 1;
                supportUnlockButton.SetActive(true);               
            }
            else if(GetComponent<PuzzleLogicManager>().puzzleSupports[1])
            {
                supportText.SetActive(true);
                supportText.GetComponent<TMP_Text>().text = secondSupportText;
            }

            isPanelShown = true;
        }
    }

    public void DisplayThirdSupportPanel()
    {
        buttonsAudioSource.Play();

        if(supportSectionPanel.activeInHierarchy == true && isPanelShown)
        {
            supportSectionPanel.SetActive(false);
            thirdSupportPanelButton.SetActive(false);
            supportUnlockButton.SetActive(false);
            supportText.SetActive(false);
            thirdSupportWarningText.SetActive(false);
            isPanelShown = false;
            outPanelDetectionButton.SetActive(false); 
        }
        else if(supportSectionPanel.activeInHierarchy == false && !isPanelShown)
        {
            supportSectionPanel.SetActive(true);
            thirdSupportPanelButton.SetActive(true);
            outPanelDetectionButton.SetActive(true);
            isThirdSupportShown = true;

            if(!GetComponent<PuzzleLogicManager>().puzzleSupports[2])
            {
                supportIndex = 2;
                supportUnlockButton.SetActive(true);               
            }
            else if(GetComponent<PuzzleLogicManager>().puzzleSupports[2])
            {
                supportText.SetActive(true);
                supportText.GetComponent<TMP_Text>().text = thirdSupportText;
            }

            isPanelShown = true;
        }
    }

    public void ReturnToGameScene()
    {
        buttonsAudioSource.Play();
        StartCoroutine(WaitForSoundAndReturn());
    }

    public void ReadyToCheckSolution()
    {
        isCheckTrigger = true;
    }

    public void OutPanelDisplay()
    {
        if(supportSectionPanel.activeInHierarchy == true && isPanelShown)
        {
            if(isFirstSupportShown)
            {
                isFirstSupportShown = false;
                DisplayFirstSupportPanel();
            }
            else if(isSecondSupportShown)
            {
                isSecondSupportShown = false;
                DisplaySecondSupportPanel();
            }
            else if(isThirdSupportShown)
            {
                isThirdSupportShown = false;
                DisplayThirdSupportPanel();
            }
        }

        if(skipPanel.activeInHierarchy == true && isPanelShown)
        {
            DisplaySkipPanel();
        }

        if(helpPanel.activeInHierarchy == true && isPanelShown)
        {
            DisplayHelpPanel();
        }

        outPanelDetectionButton.SetActive(false);
    }

    private IEnumerator ExecuteAfterDelay()
    {
        yield return null;

        if(GetComponent<PuzzleLogicManager>().puzzleSupports[0] && GetComponent<PuzzleLogicManager>().puzzleSupports[1] 
            && GetComponent<PuzzleLogicManager>().puzzleSupports[2])
        {
            skipButton.SetActive(true);
        }
    }

    private IEnumerator WaitForSoundAndReturn()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        GetComponent<PuzzleLogicManager>().ReturnToGameScene();
    }

    private IEnumerator WaitForSoundAndSkip()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        isPuzzleSkipped = true;
    }
}
