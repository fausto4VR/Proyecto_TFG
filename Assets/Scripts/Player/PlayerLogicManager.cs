using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLogicManager : MonoBehaviour
{
    [SerializeField] private GameObject inspectProgressBar;
    [SerializeField] private float holdInspectKeyTime = 2.0f;
    [SerializeField, TextArea(4,5)] private string[] defaultInspectDialogueLines;
    [SerializeField] private string[] defaultInspectCharacterNameLines;
    [SerializeField] private GameObject audioSourcesManager;
    [SerializeField] private AudioSource worldMusic;
    
    public GameObject inspectLayout;
    public GameObject dialoguePanel;
    public Sprite playerImage;
    public string playerName = "Player";
    public bool isObjectInspected = false;
    public bool isEmptyInspected = false;
    public bool isInspectInRange;
    public bool didConversationStart;
    public bool isTutorialInProgress;

    private float holdInspectKeyDuration = 0f;
    private AudioSource inspectAudioSource;

    void Start()
    {
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        inspectAudioSource = audioSources[3];
    }

    void Update()
    {
        if(!isTutorialInProgress && !GetComponent<PlayerMovement>().isPlayerTalking 
            && !GetComponent<PlayerMovement>().isPlayerDoingTutorial && !GetComponent<PlayerMovement>().isPlayerInPause)
        {
            InspectCheck();
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Guilty: " + GameLogicManager.Instance.Guilty);
            Debug.Log("First Clue: " + GameLogicManager.Instance.Clues[0]);
            Debug.Log("Second Clue: " + GameLogicManager.Instance.Clues[1]);
            Debug.Log("Third Clue: " + GameLogicManager.Instance.Clues[2]);
            Debug.Log("Story Phase Aux: " + GameLogicManager.Instance.StoryPhaseAux); // QUITAR
            Debug.Log("Story Phase: " + GameLogicManager.Instance.CurrentStoryPhase.phaseName);
            Debug.Log("Story Subphase: " + GameLogicManager.Instance.CurrentStoryPhase.currentSubphase.subphaseName);
            Debug.Log("Last Puzzle Complete: " + GameLogicManager.Instance.LastPuzzleComplete);
            string suspectsContent = string.Join(", ", GameLogicManager.Instance.KnownSuspects);
            Debug.Log("Known Suspects: " + suspectsContent);
            string tutorialsContent = string.Join(", ", GameLogicManager.Instance.KnownTutorials);
            Debug.Log("Known Tutorials: " + tutorialsContent);
            string dialoguesContent = string.Join(", ", GameLogicManager.Instance.KnownDialogues);
            Debug.Log("Known Dialogues: " + dialoguesContent);            
            Debug.Log("Is Bad Ending: " + GameLogicManager.Instance.IsBadEnding);       
            Debug.Log("End Opportunities: " + GameLogicManager.Instance.EndOpportunities);   
            string guiltyNamesContent = string.Join(", ", GameLogicManager.Instance.GuiltyNames);
            Debug.Log("Guilty Names: " + guiltyNamesContent);
        }
    }

    private void InspectCheck()
    {
        if(Input.GetKeyDown(KeyCode.Q) && !GetComponent<PlayerMovement>().isPlayerInspecting)
        {
            worldMusic.Stop();
            inspectAudioSource.Play();
        }

        if (Input.GetKey(KeyCode.Q) && !isObjectInspected && !isEmptyInspected)
        {
            GetComponent<PlayerMovement>().isPlayerInspecting = true;
            inspectLayout.SetActive(true);

            holdInspectKeyDuration += Time.deltaTime;
            float progress = holdInspectKeyDuration / holdInspectKeyTime;
            inspectProgressBar.GetComponent<Image>().fillAmount = progress;

            if(holdInspectKeyDuration >= holdInspectKeyTime)
            {
                if (!worldMusic.isPlaying)
                {
                    worldMusic.Play();
                }
                inspectProgressBar.GetComponent<Image>().fillAmount = 1;

                if(isInspectInRange)
                {
                    isObjectInspected = true;
                }
                else
                {
                    isEmptyInspected = true;
                    showDefaultMessage();
                }
            }
        }
        else if(isEmptyInspected || isObjectInspected)
        {
            ResetProgress();
        }
        else if((!isEmptyInspected || !isObjectInspected) && Input.GetKeyUp(KeyCode.Q))
        {
            GetComponent<PlayerMovement>().isPlayerInspecting = false;
            ResetProgress();
        }
    }

    public void showDefaultMessage()
    {
        didConversationStart = true;
        GetComponent<DialogueManager>().dialogueLines = defaultInspectDialogueLines;
        GetComponent<DialogueManager>().characterNameLines = defaultInspectCharacterNameLines;
    }

    private void ResetProgress()
    {
        inspectAudioSource.Stop();
        if (!worldMusic.isPlaying)
        {
            worldMusic.Play();
        }
        inspectLayout.SetActive(false);
        holdInspectKeyDuration = 0f;
        inspectProgressBar.GetComponent<Image>().fillAmount = 0f;
    }
}
