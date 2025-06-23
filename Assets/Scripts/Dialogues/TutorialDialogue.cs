using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

// Enum del posicionamiento del tutorial en pantalla
public enum TutorialPlacement
{
    Left, Right
}

// Enum que representa si el siguiente texto que se tiene que mostrar del tutorial es anterior o posterior
public enum TutorialNextPhase
{
    Previous, Posterior
}

public class TutorialDialogue : MonoBehaviour, IDialogueLogic
{
    [Header("UI Objects Section")]
    [SerializeField] private GameObject defaultVisualSupport;
    [SerializeField] private List<GameObject> visualSupports;

    [Header("Variable Section")]
    [SerializeField] private TutorialPlacement tutorialPlacement;
    [SerializeField] private List<int> tutorialSupportShownPhases;

    private Coroutine skipCoroutine;
    private Coroutine tutorialCoroutine;
    private string[] tutorialText;
    private int tutorialIndex;
    private bool isOptionChosen;

    // REVISAR AUDIO
    private AudioSource tutorialAudioSource;


    void Start()
    {
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        tutorialAudioSource = audioSources[5];

        if (!(tutorialSupportShownPhases.Count() == visualSupports.Count()))
        Debug.LogError($"No está bien establecido el número de fases ({tutorialSupportShownPhases.Count()}) con respecto a las ayudas visuales ({visualSupports.Count()}) en el tutorial.");

        if (tutorialPlacement == TutorialPlacement.Left)
        {
            RectTransform rectTransform = GameLogicManager.Instance.UIManager.TutorialPanel.GetComponent<RectTransform>();
            if (rectTransform.anchoredPosition.x > 0)
                rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        }

        if (tutorialPlacement == TutorialPlacement.Right)
        {
            RectTransform rectTransform = GameLogicManager.Instance.UIManager.TutorialPanel.GetComponent<RectTransform>();
            if (rectTransform.anchoredPosition.x < 0)
                rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

        }
    }

    // Método que se llama cuando se entra en el rango de diálogo de un objeto
    public void WaitForDialogueInput()
    {
        if (skipCoroutine != null) StopCoroutine(skipCoroutine);

        if (!GameLogicManager.Instance.KnownTutorials.TryGetValue(gameObject.name, out bool isKnown) || !isKnown)
        {
            skipCoroutine = StartCoroutine(WaitToSkipTutorial());

            tutorialAudioSource.Play();

            PlayerEvents.StartShowingInformation();

            GameLogicManager.Instance.UIManager.TutorialPanel.SetActive(true);
            GameLogicManager.Instance.UIManager.OutDetectionPanel.SetActive(true);
            ManageFirstTutorialText();
        }
    }

    // Método para iniciar la conversación al hacer clic con el cursor sobre el personaje
    public void TryStartDialogueOnClick()
    {
        // Se deja vacío porque este tipo de diálogo no se activa por clic
    }

    // Corrutina para esperar a que el jugador quiera saltarse el tutorial una vez empezado
    private IEnumerator WaitToSkipTutorial()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E) && tutorialIndex == tutorialText.Length - 1);
        yield return null;

        EndTutorial();
    }

    // Método para gestionar el primer texto que se muestra en el tutorial
    private void ManageFirstTutorialText()
    {
        tutorialIndex = 0;

        tutorialText = GameStateManager.Instance.gameConversations.tutorialTexts
            .FirstOrDefault(text => text.objectName == gameObject.name)?.text
            .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

        GameLogicManager.Instance.UIManager.TutorialText.text = tutorialText[0];
        ShowVisualSupport(true);
        UpdateShowButtons();

        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
            tutorialCoroutine = null;
        }

        tutorialCoroutine = StartCoroutine(NextTutorialText());
    }

    // Corrutina para decidir cual es el siguiente texto en el tutorial
    private IEnumerator NextTutorialText()
    {        
        yield return null;
        
        isOptionChosen = false;
        ManageButtonLogic();

        while (!isOptionChosen)
        {
            if (tutorialIndex > 0 && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)))
            {
                ShowNextTutorialText(TutorialNextPhase.Previous);
                isOptionChosen = true;
            }
            else if (tutorialIndex < tutorialText.Length - 1 && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)))
            {
                ShowNextTutorialText(TutorialNextPhase.Posterior);
                isOptionChosen = true;
            }

            yield return null;
        }
    }

    // Método para gestionar la lógica de los botones y que al pulsarlos hagan lo mismo que las flechas de dirección
    private void ManageButtonLogic()
    {
        Button leftArrowTutorialButton = GameLogicManager.Instance.UIManager.LeftArrowTutorialButton.GetComponent<Button>();
        Button rightArrowTutorialButton = GameLogicManager.Instance.UIManager.RightArrowTutorialButton.GetComponent<Button>();

        leftArrowTutorialButton.onClick.RemoveAllListeners();
        rightArrowTutorialButton.onClick.RemoveAllListeners();
        
        leftArrowTutorialButton.onClick.AddListener(() =>
        {
            if (!isOptionChosen)
            {
                if (tutorialIndex > 0)
                {
                    ShowNextTutorialText(TutorialNextPhase.Previous);
                    isOptionChosen = true;
                }
                else
                {
                    Debug.LogError("El botón de la flecha izquierda se ha presionado en la primera fase.");
                }
            }
        });

        rightArrowTutorialButton.onClick.AddListener(() =>
        {
            if (!isOptionChosen)
            {
                if (tutorialIndex < tutorialText.Length - 1)
                {
                    ShowNextTutorialText(TutorialNextPhase.Posterior);
                    isOptionChosen = true;
                }
                else
                {
                    Debug.LogError("El botón de la flecha derecha se ha presionado en la última fase.");
                }
            }
        });
    }

    // Método para mostrar el siguiente texto en el tutorial
    public void ShowNextTutorialText(TutorialNextPhase nextPhase)
    {
        if (nextPhase == TutorialNextPhase.Previous)
        {
            if (tutorialIndex > 0) tutorialIndex--;
            else Debug.LogError("El índice del tutorial es menor que cero.");
        }
        else if (nextPhase == TutorialNextPhase.Posterior)
        {
            if (tutorialIndex < tutorialText.Length - 1) tutorialIndex++;
            else Debug.LogError("El índice del tutorial es mayor que el tamaño de fases del tutorial.");
        }

        GameLogicManager.Instance.UIManager.TutorialText.text = tutorialText[tutorialIndex];
        ShowVisualSupport(true);
        UpdateShowButtons();     

        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
            tutorialCoroutine = null;
        }

        tutorialCoroutine = StartCoroutine(NextTutorialText());
    }

    // Método para actualizar si se muestran o no los botones
    private void UpdateShowButtons()
    {         
        if (tutorialIndex == 0) GameLogicManager.Instance.UIManager.LeftArrowTutorialButton.SetActive(false);
        else GameLogicManager.Instance.UIManager.LeftArrowTutorialButton.SetActive(true);

        if (tutorialIndex == tutorialText.Length - 1) GameLogicManager.Instance.UIManager.RightArrowTutorialButton.SetActive(false);
        else GameLogicManager.Instance.UIManager.RightArrowTutorialButton.SetActive(true);
    }

    // Método para terminar de mostrar el tutorial
    private void EndTutorial()
    {
        Dictionary<string, bool> updatedTutorials = GameLogicManager.Instance.KnownTutorials;
        updatedTutorials[gameObject.name] = true;
        GameLogicManager.Instance.KnownTutorials = updatedTutorials;

        PlayerEvents.FinishShowingInformation();

        GameLogicManager.Instance.UIManager.TutorialPanel.SetActive(false);
        GameLogicManager.Instance.UIManager.OutDetectionPanel.SetActive(false);
        GameLogicManager.Instance.UIManager.TutorialText.text = "***";
        ShowVisualSupport(false);
        tutorialIndex = 0;

        ExitOfDialogueRange();
    }

    // Método para activar o desactivar la ayuda visual del tutorial
    private void ShowVisualSupport(bool activation)
    {
        defaultVisualSupport.SetActive(false);
        visualSupports?.ForEach(support => support?.SetActive(false));

        if (activation)
        {
            if (tutorialSupportShownPhases == null || tutorialSupportShownPhases.Count == 0 ||
                visualSupports == null || visualSupports.Count == 0)
            {
                defaultVisualSupport.SetActive(true);
                return;
            }

            int index = tutorialSupportShownPhases.IndexOf(tutorialIndex);

            if (index != -1 && index < visualSupports.Count) visualSupports[index].SetActive(true);
            else defaultVisualSupport.SetActive(true);
        }
    }

    // Método que se llama cuando se sale del rango de diálogo de un objeto
    public void ExitOfDialogueRange()
    {
        isOptionChosen = false;
        StopAllCoroutines();

        if (skipCoroutine != null) skipCoroutine = null;
        if (tutorialCoroutine != null) tutorialCoroutine = null;
    }
}
