using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PuzzleUIManager : MonoBehaviour
{
    [Header("Help and Skip Section")]
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private GameObject skipPanel;
    [SerializeField] private GameObject skipButton;

    [Header("Supports Section")]
    [SerializeField] private GameObject supportSectionPanel;
    [SerializeField] private GameObject firstSupportButton;
    [SerializeField] private GameObject secondSupportButton;
    [SerializeField] private GameObject thirdSupportButton;
    [SerializeField] private GameObject firstSupportPanelButton;
    [SerializeField] private GameObject secondSupportPanelButton;
    [SerializeField] private GameObject thirdSupportPanelButton;
    [SerializeField] private GameObject supportUnlockButton;
    [SerializeField] private GameObject secondSupportWarningText;
    [SerializeField] private GameObject thirdSupportWarningText;   
    [SerializeField] private GameObject supportText; 

    [Header("Solution Section")]
    [SerializeField] private GameObject successsPanel;
    [SerializeField] private GameObject failurePanel;
    [SerializeField] private TMP_Text pointsSuccessText;
    [SerializeField] private TMP_Text pointsFailureText;
    
    [Header("Detection Section")]
    [SerializeField] private GameObject outStatementDetectionButton;
    [SerializeField] private GameObject outPanelDetectionButton; 

    [Header("Sound Section")]   
    [SerializeField] private GameObject audioSourcesManager;    
    [SerializeField] private AudioSource puzzleMusic;
    
    [Header("Variable Section")]
    private float transparentSupportButtonColor = 0.63f;
    
    private Color unlockSupportButtonColor;
    private Image firstSupportImage;
    private Image secondSupportImage;
    private Image thirdSupportImage;
    private string firstSupportText;    
    private string secondSupportText;    
    private string thirdSupportText;
    private string lastSupportButtonActivated;
    private bool isPanelShown;

    // REVISAR AUDIO
    private AudioSource buttonsAudioSource;
    private AudioSource unlockSupportAudioSource;
    private AudioSource successSolutionAudioSource;
    private AudioSource failureSolutionAudioSource;
    private AudioSource accessDeniedudioSource;

    // QUITAR    
    public ResultType isCorrectResult;
    public bool isCheckTrigger;
    public bool isNecesaryResetInputs;

    void Start()
    {
        unlockSupportButtonColor = new Color(0.525f, 1f, 0.518f, 1f);

        firstSupportImage = firstSupportPanelButton.GetComponent<Image>();
        secondSupportImage = secondSupportPanelButton.GetComponent<Image>();
        thirdSupportImage = thirdSupportPanelButton.GetComponent<Image>();

        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        buttonsAudioSource = audioSources[1];
        unlockSupportAudioSource = audioSources[2];
        successSolutionAudioSource = audioSources[3];
        failureSolutionAudioSource = audioSources[4];
        accessDeniedudioSource = audioSources[5];

        if(GetComponent<PuzzleLogicManager>().puzzleSupports[0])
        {
            firstSupportButton.GetComponent<Image>().color = unlockSupportButtonColor;
            firstSupportPanelButton.GetComponent<Image>().color = unlockSupportButtonColor;
        }

        if(GetComponent<PuzzleLogicManager>().puzzleSupports[1])
        {
            secondSupportButton.GetComponent<Image>().color = unlockSupportButtonColor;
            secondSupportPanelButton.GetComponent<Image>().color = unlockSupportButtonColor;
        }

        if(GetComponent<PuzzleLogicManager>().puzzleSupports[2])
        {
            thirdSupportButton.GetComponent<Image>().color = unlockSupportButtonColor;
            thirdSupportPanelButton.GetComponent<Image>().color = unlockSupportButtonColor;
        }

        // Para esperar que esté inicializada la lista de puzzleSupports al entrar en la escena
        StartCoroutine(ExecuteAfterDelay());
    }

    // Corrutina para esperar que la lista de pistas esté inicializada y activar el botón de omitir el puzle si fuese necesario
    private IEnumerator ExecuteAfterDelay()
    {
        while (GetComponent<PuzzleLogicManager>().puzzleSupports == null || 
           GetComponent<PuzzleLogicManager>().puzzleSupports.Length == 0)
        {
            yield return null; // Espera un frame
        }

        if(GetComponent<PuzzleLogicManager>().puzzleSupports[0] && GetComponent<PuzzleLogicManager>().puzzleSupports[1] 
            && GetComponent<PuzzleLogicManager>().puzzleSupports[2])
        {
            skipButton.SetActive(true);
        }
    }

    // Método para cambiar el texto de la primera pista
    public void SetFirstSupportText(string firstSupportTextInput)
    {
        firstSupportText = firstSupportTextInput;
    }

    // Método para cambiar el texto de la segunda pista
    public void SetSecondSupportText(string secondSupportTextInput)
    {
        secondSupportText = secondSupportTextInput;
    }

    // Método para cambiar el texto de la tercera pista
    public void SetThirdSupportText(string thirdSupportTextInput)
    {
        thirdSupportText = thirdSupportTextInput;
    }

    // Método para mostrar el panel de éxito despúes de acertar la solución de un puzle
    public void ShowSuccessPanel()
    {
        puzzleMusic.Stop();
        successSolutionAudioSource.Play();
        successsPanel.SetActive(true);
        int points = GetComponent<PuzzleLogicManager>().CalculatePoints(true);
        pointsSuccessText.text = points + "/50";
        GetComponent<PuzzleLogicManager>().DetectToClosePanel(true);
    }

    // Método para mostrar el panel de fallo despúes de fallar la solución de un puzle
    public void ShowFailurePanel()
    {
        puzzleMusic.Stop();
        failureSolutionAudioSource.Play();
        failurePanel.SetActive(true);
        int points = GetComponent<PuzzleLogicManager>().CalculatePoints(false);
        pointsFailureText.text = points + "/50";
        GetComponent<PuzzleLogicManager>().DetectToClosePanel(false);
    }

    // Método para desactivar el panel de fallo y volver a mostrar el puzle 
    public void ReturnToPuzzleAfterFail()
    {
        failureSolutionAudioSource.Stop();
        puzzleMusic.Play();
        failurePanel.SetActive(false);

        IPuzzleLogic puzzleLogic = GetComponent<IPuzzleLogic>();

        if (puzzleLogic != null)
        {
            puzzleLogic.ResetSolutionInputs();
        }
        else
        {
            Debug.LogError("El componente de lógica del puzzle no está presente en este GameObject.");
        }
    }

    // Método para mostar el panel de ayuda
    public void DisplayHelpPanel()
    {
        buttonsAudioSource.Play();

        if(helpPanel.activeInHierarchy && isPanelShown)
        {
            helpPanel.SetActive(false);
            isPanelShown = false;
            outPanelDetectionButton.SetActive(false); 
        }
        else if(!helpPanel.activeInHierarchy && !isPanelShown)
        {
            helpPanel.SetActive(true);
            outPanelDetectionButton.SetActive(true);
            isPanelShown = true;
        }
    }

    // Método para mostar el panel de saltar puzle
    public void DisplaySkipPanel()
    {
        buttonsAudioSource.Play();

        if(skipPanel.activeInHierarchy && isPanelShown)
        {
            skipPanel.SetActive(false);
            isPanelShown = false;
            outPanelDetectionButton.SetActive(false); 
        }
        else if(!skipPanel.activeInHierarchy && !isPanelShown)
        {
            outPanelDetectionButton.SetActive(true);
            skipPanel.SetActive(true);
            isPanelShown = true;
        }
    }

    // Método para mostar el panel de pistas
    public void DisplaySupportPanel(Button button)
    {
        buttonsAudioSource.Play();
        string supportButtonTag = button.gameObject.tag;

        if(supportSectionPanel.activeInHierarchy && isPanelShown)
        {
            // Sección para cerrar el panel de pistas
            if(lastSupportButtonActivated == supportButtonTag)
            {
                CloseSupportPanel();
            }
            // Sección para cambiar entre pistas
            else
            {
                lastSupportButtonActivated = supportButtonTag;
                secondSupportWarningText.SetActive(false);
                thirdSupportWarningText.SetActive(false);
                ShowSupport(supportButtonTag);
            }
        }
        // Sección para abrir el panel de pistas con la que corresponda
        else if (!supportSectionPanel.activeInHierarchy && !isPanelShown)
        {
            supportSectionPanel.SetActive(true);
            outPanelDetectionButton.SetActive(true);            
            isPanelShown = true;
            lastSupportButtonActivated = supportButtonTag;

            ShowSupport(supportButtonTag);
        }
    }

    // Método para cerrar él panel de pistas
    private void CloseSupportPanel()
    {
        supportSectionPanel.SetActive(false);
        supportUnlockButton.SetActive(false);
        supportText.SetActive(false);
        secondSupportWarningText.SetActive(false);
        thirdSupportWarningText.SetActive(false);
        outPanelDetectionButton.SetActive(false);
        isPanelShown = false;
    }

    // Método para mostrar la pista correspondiente
    private void ShowSupport(string supportButtonTag)
    {
        if(supportButtonTag == "Support1")
        {
            firstSupportImage.color = new Color(firstSupportImage.color.r, firstSupportImage.color.g, firstSupportImage.color.b, 1f);
            secondSupportImage.color = new Color(secondSupportImage.color.r, secondSupportImage.color.g, secondSupportImage.color.b, transparentSupportButtonColor);
            thirdSupportImage.color = new Color(thirdSupportImage.color.r, thirdSupportImage.color.g, thirdSupportImage.color.b, transparentSupportButtonColor);

            if(!GetComponent<PuzzleLogicManager>().puzzleSupports[0])
            {
                supportText.SetActive(false);
                supportUnlockButton.SetActive(true);               
            }
            else if(GetComponent<PuzzleLogicManager>().puzzleSupports[0])
            {
                supportUnlockButton.SetActive(false);  
                supportText.SetActive(true);
                supportText.GetComponent<TMP_Text>().text = firstSupportText;
            }            
        }

        else if(supportButtonTag == "Support2")
        {
            firstSupportImage.color = new Color(firstSupportImage.color.r, firstSupportImage.color.g, firstSupportImage.color.b, transparentSupportButtonColor);
            secondSupportImage.color = new Color(secondSupportImage.color.r, secondSupportImage.color.g, secondSupportImage.color.b, 1f);
            thirdSupportImage.color = new Color(thirdSupportImage.color.r, thirdSupportImage.color.g, thirdSupportImage.color.b, transparentSupportButtonColor);

            if(!GetComponent<PuzzleLogicManager>().puzzleSupports[1])
            {
                supportText.SetActive(false);
                supportUnlockButton.SetActive(true);               
            }
            else if(GetComponent<PuzzleLogicManager>().puzzleSupports[1])
            {
                supportUnlockButton.SetActive(false);
                supportText.SetActive(true);
                supportText.GetComponent<TMP_Text>().text = secondSupportText;
            }
        }

        else if(supportButtonTag == "Support3")
        {
            firstSupportImage.color = new Color(firstSupportImage.color.r, firstSupportImage.color.g, firstSupportImage.color.b, transparentSupportButtonColor);
            secondSupportImage.color = new Color(secondSupportImage.color.r, secondSupportImage.color.g, secondSupportImage.color.b, transparentSupportButtonColor);
            thirdSupportImage.color = new Color(thirdSupportImage.color.r, thirdSupportImage.color.g, thirdSupportImage.color.b, 1f);

            if(!GetComponent<PuzzleLogicManager>().puzzleSupports[2])
            {
                supportText.SetActive(false);
                supportUnlockButton.SetActive(true);               
            }
            else if(GetComponent<PuzzleLogicManager>().puzzleSupports[2])
            {
                supportUnlockButton.SetActive(false);
                supportText.SetActive(true);
                supportText.GetComponent<TMP_Text>().text = thirdSupportText;
            }
        }
    }

    // Método para desbloquear las pistas
    public void UnlockSupport()
    {
        if(lastSupportButtonActivated == "Support1")
        {
            unlockSupportAudioSource.Play();
            firstSupportButton.GetComponent<Image>().color = unlockSupportButtonColor;
            firstSupportPanelButton.GetComponent<Image>().color = unlockSupportButtonColor;
            supportText.GetComponent<TMP_Text>().text = firstSupportText;
            GetComponent<PuzzleLogicManager>().puzzleSupports[0] = true;
            supportUnlockButton.SetActive(false);
            supportText.SetActive(true);
        }
        else if(lastSupportButtonActivated == "Support2")
        {
            if(GetComponent<PuzzleLogicManager>().puzzleSupports[0])
            {
                unlockSupportAudioSource.Play();
                secondSupportButton.GetComponent<Image>().color = unlockSupportButtonColor;
                secondSupportPanelButton.GetComponent<Image>().color = unlockSupportButtonColor;
                supportText.GetComponent<TMP_Text>().text = secondSupportText;
                GetComponent<PuzzleLogicManager>().puzzleSupports[1] = true;
                supportUnlockButton.SetActive(false);
                supportText.SetActive(true);
            }
            else
            {
                accessDeniedudioSource.Play();
                secondSupportWarningText.SetActive(true);
            }
        }
        else if(lastSupportButtonActivated == "Support3")
        {
            if(GetComponent<PuzzleLogicManager>().puzzleSupports[0] && GetComponent<PuzzleLogicManager>().puzzleSupports[1])
            {
                unlockSupportAudioSource.Play();
                thirdSupportButton.GetComponent<Image>().color = unlockSupportButtonColor;
                thirdSupportPanelButton.GetComponent<Image>().color = unlockSupportButtonColor;
                supportText.GetComponent<TMP_Text>().text = thirdSupportText;
                GetComponent<PuzzleLogicManager>().puzzleSupports[2] = true;
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

    // Método para cerrar un panel pulsando fuera de él
    public void OutStatementDisplay()
    {
        outStatementDetectionButton.SetActive(false);
    }

    // Método para cerrar un panel pulsando fuera de él
    public void OutPanelDisplay()
    {
        if(supportSectionPanel.activeInHierarchy == true && isPanelShown)
        {
            CloseSupportPanel();
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

    // Método para activar la lógica que omite el puzle
    public void SkipPuzzle()
    {
        buttonsAudioSource.Play();
        StartCoroutine(WaitForButtonSound("skip"));
    }

    // Método para activar la lógica que vuelve a la escena
    public void ReturnToGameScene()
    {
        buttonsAudioSource.Play();
        StartCoroutine(WaitForButtonSound("return"));
    }
    
    // Corrutina para esperar que suene el efecto de sonido de un botón completamente
    private IEnumerator WaitForButtonSound(string option)
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);

        if (option == "skip")
        {
            GetComponent<PuzzleLogicManager>().SkipPuzzleLogic();
        }
        else if (option == "return") 
        {
            GetComponent<PuzzleLogicManager>().ReturnToGameScene();
        }
    }

    // Método para comprobar el resultado al pulsar el botón de Listo
    public void ReadyToCheckSolution()
    {
        IPuzzleLogic puzzleLogic = GetComponent<IPuzzleLogic>();

        if (puzzleLogic != null)
        {
            puzzleLogic.CheckResult();
        }
        else
        {
            Debug.LogError("El componente de lógica del puzzle no está presente en este GameObject.");
        }
    }
}
