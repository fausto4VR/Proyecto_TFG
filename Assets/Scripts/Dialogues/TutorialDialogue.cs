using System.Linq;
using TMPro;
using UnityEngine;

public class TutorialDialogue : MonoBehaviour
{
    [SerializeField, TextArea(4,5)] private string[] turorialDialogueLines;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject talkingTutorial;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private int tutorialIndexOrder;
    [SerializeField] private Option panelSide;
    [SerializeField] private GameObject audioSourcesManager;

    private enum Option {Left, Right}
    private bool isPlayerInRange;
    private bool isTutorialDone;
    private int turorialDialogueLineIndex;
    private bool isVisualSupportShown;
    private bool isTutorialInProgress;
    private AudioSource tutorialAudioSource;


    void Start()
    {        
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        tutorialAudioSource = audioSources[5];

        turorialDialogueLineIndex = 0;

        if(panelSide == Option.Left)
        {
            RectTransform rectTransform = tutorialPanel.GetComponent<RectTransform>();
            if(rectTransform.anchoredPosition.x > 0)
            {
                rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
            }
        }

        if(panelSide == Option.Right)
        {
            RectTransform rectTransform = tutorialPanel.GetComponent<RectTransform>();
            if(rectTransform.anchoredPosition.x < 0)
            {
                rectTransform.anchoredPosition = new Vector2(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
            }
        }
    }

    void Update()
    {
        if(isPlayerInRange && !isTutorialDone && !GameLogicManager.Instance.KnownTutorials[tutorialIndexOrder])
        {
            if(!isTutorialInProgress)
            {
                isTutorialInProgress = true;
                tutorialAudioSource.Play();
            }

            player.GetComponent<PlayerMovement>().isPlayerDoingTutorial = true;
            player.GetComponent<PlayerLogicManager>().isTutorialInProgress = true;
            tutorialPanel.SetActive(true);

            if(turorialDialogueLines.Count() == 1)
            {
                tutorialText.text = turorialDialogueLines[0];

                if(!isVisualSupportShown)
                {
                    ShowVisualSupport(true);
                    isVisualSupportShown = true;
                }

                if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    FinishTutorial();
                }
            }
            else if(turorialDialogueLines.Count() > 1 && turorialDialogueLineIndex < turorialDialogueLines.Count())
            {
                tutorialText.text = turorialDialogueLines[turorialDialogueLineIndex];

                if(!isVisualSupportShown)
                {
                    ShowVisualSupport(true);
                    isVisualSupportShown = true;
                }

                if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    turorialDialogueLineIndex++;
                    isVisualSupportShown = false; 
                }               
            }
            else if(turorialDialogueLines.Count() > 1 && turorialDialogueLineIndex == turorialDialogueLines.Count())
            {
                FinishTutorial();            
            }
        }
    }

    private void ShowVisualSupport(bool activation)
    {
        if(tutorialIndexOrder == 0)
        {
            talkingTutorial.SetActive(activation);
        }
    }

    private void FinishTutorial()
    {
        isTutorialDone = true;
        isTutorialInProgress = false;
        player.GetComponent<PlayerMovement>().isPlayerDoingTutorial = false;
        player.GetComponent<PlayerLogicManager>().isTutorialInProgress = false;
        ShowVisualSupport(false);
        tutorialPanel.SetActive(false);
        GameLogicManager.Instance.KnownTutorials[tutorialIndexOrder] = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
