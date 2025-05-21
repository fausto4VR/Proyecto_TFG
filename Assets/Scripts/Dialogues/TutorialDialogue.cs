using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

// Enum del posicionamiento del tutorial en pantalla
public enum TutorialPlacement
{
    Left, Right
}

public class TutorialDialogue : MonoBehaviour, IDialogueLogic
{
    [Header("UI Objects Section")]
    [SerializeField] private List<GameObject> visualSupports;
    
    [Header("Variable Section")]
    [SerializeField] private TutorialPlacement tutorialPlacement;
    [SerializeField] private List<int> tutorialSupportPhases;
    
    private Coroutine skipCoroutine;
    private Coroutine tutorialCoroutine;
    private string[] tutorialText;
    private int tutorialIndex;
    private int visualSupportIndex;

    // REVISAR AUDIO
    private AudioSource tutorialAudioSource;


    void Start()
    {        
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;        
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        tutorialAudioSource = audioSources[5];

        if (tutorialSupportPhases == null || tutorialSupportPhases.Count == 0) new List<int>();
        if (tutorialSupportPhases != null && visualSupports.Count() > 0) tutorialSupportPhases.Add(visualSupports.Count() - 1);
        tutorialIndex = 0;

        if (tutorialSupportPhases.Count() > 0 && visualSupports.Count() > 0 && !(tutorialSupportPhases.Count() == visualSupports.Count()))
        Debug.LogError($"No esta bien establecido el número de fases ({tutorialSupportPhases.Count()}) con respecto a las ayudas visuales ({visualSupports.Count()}) en el tutorial.");

        if(tutorialPlacement == TutorialPlacement.Left)
        {
            RectTransform rectTransform = GameLogicManager.Instance.UIManager.TutorialPanel.GetComponent<RectTransform>();
            if(rectTransform.anchoredPosition.x > 0)
            rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        }

        if(tutorialPlacement == TutorialPlacement.Right)
        {
            RectTransform rectTransform = GameLogicManager.Instance.UIManager.TutorialPanel.GetComponent<RectTransform>();
            if(rectTransform.anchoredPosition.x < 0)
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
            ManageTutorialText();
            ShowVisualSupport(true);
        }
    } 

    // Corrutina para esperar a que el jugador quiera saltarse el tutorial una vez empezado
    private IEnumerator WaitToSkipTutorial()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        yield return null;

        EndTutorial();
    }

    // Método para gestionar el texto que se muestra en el tutorial
    private void ManageTutorialText()
    {
        tutorialText = GameStateManager.Instance.gameConversations.tutorialTexts
            .FirstOrDefault(text => text.objectName == gameObject.name)?.text
            .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

        GameLogicManager.Instance.UIManager.TutorialText.text = tutorialText[0];
        
        tutorialIndex = 0;
        visualSupportIndex = 0;

        tutorialCoroutine = StartCoroutine(NextTutorialText(tutorialIndex+1));
    }

    // Corrutina para mostrar el siguiente texto en el tutorial
    private IEnumerator NextTutorialText(int currentTutorialIndex)
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
        yield return null;

        if (currentTutorialIndex >= tutorialText.Length)
        {
            EndTutorial();
            yield break;
        }

        GameLogicManager.Instance.UIManager.TutorialText.text = tutorialText[currentTutorialIndex];
        ShowVisualSupport(true);

        tutorialIndex = currentTutorialIndex;
        tutorialCoroutine = StartCoroutine(NextTutorialText(currentTutorialIndex + 1));
    }

    // Método para terminar de mostrar el tutorial
    private void EndTutorial()
    {
        Dictionary<string, bool> updatedTutorials = GameLogicManager.Instance.KnownTutorials;
        updatedTutorials[gameObject.name] = true;
        GameLogicManager.Instance.KnownTutorials = updatedTutorials;

        PlayerEvents.FinishShowingInformation();

        GameLogicManager.Instance.UIManager.TutorialPanel.SetActive(false);
        GameLogicManager.Instance.UIManager.TutorialText.text = "***";
        ShowVisualSupport(false);
        tutorialIndex = 0;
        visualSupportIndex = 0;
    }

    // Método para activar o desactivar la ayuda visual del tutorial
    private void ShowVisualSupport(bool activation)
    {
        if(activation)
        {
            if (visualSupports != null && visualSupports.Count > 0 && tutorialSupportPhases != null && tutorialSupportPhases.Count > 0)
            {
                foreach (int tutorialPhase in tutorialSupportPhases)
                {
                    if(tutorialIndex <= tutorialPhase)
                    {
                        if(visualSupportIndex > 0 && visualSupportIndex < visualSupports.Count()) 
                        visualSupports[visualSupportIndex-1].SetActive(false);

                        if(visualSupportIndex < visualSupports.Count())visualSupports[visualSupportIndex].SetActive(true);

                        visualSupportIndex++;
                        break;
                    }
                }
            }
        }
        else
        {
            visualSupports.ForEach(support => support.SetActive(false));
        }
    }

   // Método que se llama cuando se sale del rango de diálogo de un objeto
   public void ExitOfDialogueRange()
    {
        if (skipCoroutine != null)
        {
            StopCoroutine(skipCoroutine);
            skipCoroutine = null;
        }

        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
            tutorialCoroutine = null;
        }
    }
}
