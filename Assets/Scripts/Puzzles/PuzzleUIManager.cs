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

    void Start()
    {
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
            failurePanel.SetActive(false);
            isFailurePanelShown = false;
            isReturnToPuzzleAfterFail = false;
            isNecesaryResetInputs = true;
        }
    }

    private void ShowSuccess()
    {
        successsPanel.SetActive(true);
        isSuccessPanelShown = true;
        int pointsToShow = GetComponent<PuzzleLogicManager>().CalculatePoints(true);
        pointsSuccessText.text = pointsToShow + "/50";

        isCorrectResult = 0;
    }

    private void ShowFailure()
    {
        failurePanel.SetActive(true);
        isFailurePanelShown = true;
        int pointsToShow = GetComponent<PuzzleLogicManager>().CalculatePoints(false);
        pointsFailureText.text = pointsToShow + "/50";

        isCorrectResult = 0;
    }

    public void UnlockSupport()
    {
        Color customColor = new Color(0.525f, 1f, 0.518f, 1f);
        GetComponent<PuzzleLogicManager>().CalculatePoints(false);

        if(supportIndex == 0)
        {
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
                secondSupportButton.GetComponent<Image>().color = customColor;
                secondSupportPanelButton.GetComponent<Image>().color = customColor;
                supportText.GetComponent<TMP_Text>().text = secondSupportText;
                GetComponent<PuzzleLogicManager>().puzzleSupports[supportIndex] = true;
                supportUnlockButton.SetActive(false);
                supportText.SetActive(true);
            }
            else
            {
                secondSupportWarningText.SetActive(true);
            }
        }

        if(supportIndex == 2)
        {
            if(GetComponent<PuzzleLogicManager>().puzzleSupports[0] && GetComponent<PuzzleLogicManager>().puzzleSupports[1])
            {
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
                thirdSupportWarningText.SetActive(true);
            }
        }
    }

    public void DisplayHelpPanel()
    {
        if(helpPanel.activeInHierarchy == true && isPanelShown)
        {
            helpPanel.SetActive(false);
            isPanelShown = false;
        }
        else if(helpPanel.activeInHierarchy == false && !isPanelShown)
        {
            helpPanel.SetActive(true);
            isPanelShown = true;
        }
    }

    public void DisplaySkipPanel()
    {
        if(skipPanel.activeInHierarchy == true && isPanelShown)
        {
            skipPanel.SetActive(false);
            isPanelShown = false;
        }
        else if(skipPanel.activeInHierarchy == false && !isPanelShown)
        {
            skipPanel.SetActive(true);
            isPanelShown = true;
        }
    }

    public void SkipPuzzle()
    {
        isPuzzleSkipped = true;
    }

    public void DisplayFirstSupportPanel()
    {
        if(supportSectionPanel.activeInHierarchy == true && isPanelShown)
        {
            supportSectionPanel.SetActive(false);
            firstSupportPanelButton.SetActive(false);
            supportUnlockButton.SetActive(false);
            supportText.SetActive(false);
            isPanelShown = false; 
        }
        else if(supportSectionPanel.activeInHierarchy == false && !isPanelShown)
        {
            supportSectionPanel.SetActive(true);
            firstSupportPanelButton.SetActive(true);

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
        if(supportSectionPanel.activeInHierarchy == true && isPanelShown)
        {
            supportSectionPanel.SetActive(false);
            secondSupportPanelButton.SetActive(false);
            supportUnlockButton.SetActive(false);
            supportText.SetActive(false);
            secondSupportWarningText.SetActive(false);
            isPanelShown = false; 
        }
        else if(supportSectionPanel.activeInHierarchy == false && !isPanelShown)
        {
            supportSectionPanel.SetActive(true);
            secondSupportPanelButton.SetActive(true);

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
        if(supportSectionPanel.activeInHierarchy == true && isPanelShown)
        {
            supportSectionPanel.SetActive(false);
            thirdSupportPanelButton.SetActive(false);
            supportUnlockButton.SetActive(false);
            supportText.SetActive(false);
            thirdSupportWarningText.SetActive(false);
            isPanelShown = false; 
        }
        else if(supportSectionPanel.activeInHierarchy == false && !isPanelShown)
        {
            supportSectionPanel.SetActive(true);
            thirdSupportPanelButton.SetActive(true);

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
        GetComponent<PuzzleLogicManager>().ReturnToGameScene();
    }

    public void ReadyToCheckSolution()
    {
        isCheckTrigger = true;
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
}
