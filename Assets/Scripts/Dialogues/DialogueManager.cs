using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum ConversationPhase
{
    Started, Continued, Ended
}

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private GameObject player;
    [SerializeField] private Image profileImage;
    [SerializeField] private Sprite characterImage;
    [SerializeField]private float typingTime = 0.05f;    
    
    public bool isClueUnlockTrigger;
    public bool isClueUnlock;
    public GameObject conversationPanel;
    public string[] dialogueLines;
    public string[] characterNameLines;
    [SerializeField]private int charsToPlaySound;  
    [SerializeField] private GameObject audioSourcesManager; 

    public bool isPlayerInRange;
    public bool didConversationStart;
    private int lineIndex;
    private MultipleChoiceDialogue multipleChoiceDialogue;
    private StoryPhaseDialogue storyPhaseDialogue;
    private InspectDialogue inspectDialogue;
    private PlayerLogicManager playerLogicManager;
    private AudioSource typingDialogueAudioSource;

    public ConversationPhase conversationPhase;

    void Start()
    {
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        typingDialogueAudioSource = audioSources[0];

        multipleChoiceDialogue = GetComponent<MultipleChoiceDialogue>();
        storyPhaseDialogue = GetComponent<StoryPhaseDialogue>();
        inspectDialogue = GetComponent<InspectDialogue>();

        if(gameObject.name=="Player")
        {
            player = gameObject;
            playerLogicManager = GetComponent<PlayerLogicManager>();
        }
    }

    void Update()
    {   
        if(GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerState is TalkingState)
        {
            if(multipleChoiceDialogue != null)
            {
                if(isPlayerInRange == true && multipleChoiceDialogue.didDialogueStart == true && !didConversationStart
                    && multipleChoiceDialogue.didConversationStart)
                {
                    StartDialogue();
                }

                if(isPlayerInRange == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) 
                    && multipleChoiceDialogue.didDialogueStart == true && didConversationStart) 
                {
                    if(dialogueText.text == dialogueLines[lineIndex])
                    {
                        NextDialogueLine();
                    }
                    else
                    {
                        ShowLineDirectly();
                    }
                }
            }

            if(storyPhaseDialogue != null)
            {
                if(isPlayerInRange == true && !didConversationStart && storyPhaseDialogue.didConversationStart)
                {
                    StartDialogue();
                }

                if(isPlayerInRange == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && didConversationStart) 
                {
                    if(dialogueText.text == dialogueLines[lineIndex])
                    {
                        NextDialogueLine();
                    }
                    else
                    {
                        ShowLineDirectly();
                    }
                }
            }

            if(inspectDialogue != null)
            {
                if(isPlayerInRange == true && !didConversationStart && inspectDialogue.didConversationStart)
                {
                    inspectDialogue.dialoguePanel.SetActive(true);
                    StartDialogue();
                }

                if(isPlayerInRange == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && didConversationStart) 
                {
                    if(dialogueText.text == dialogueLines[lineIndex])
                    {
                        NextDialogueLine();
                    }
                    else
                    {
                        ShowLineDirectly();
                    }
                }
            }

            if(playerLogicManager != null && dialogueLines != null && dialogueLines.Length > 0)
            {
                if(conversationPhase == ConversationPhase.Started)
                {
                    playerLogicManager.inspectLayout.SetActive(false);
                    playerLogicManager.dialoguePanel.SetActive(true);
                    StartDialogue();
                }

                if((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && conversationPhase == ConversationPhase.Continued) 
                {
                    if(dialogueText.text == dialogueLines[lineIndex])
                    {
                        NextDialogueLine();
                    }
                    else
                    {
                        ShowLineDirectly();
                    }
                }
            }
        }
    }

    private void StartDialogue()
    {
        didConversationStart = true;
        conversationPhase = ConversationPhase.Continued;

        if(multipleChoiceDialogue != null)
        {
            multipleChoiceDialogue.choicePanel.SetActive(false);
        }

        if(multipleChoiceDialogue != null || storyPhaseDialogue != null)
        {
            dialogueMark.SetActive(false);
        }

        conversationPanel.SetActive(true);
        lineIndex = 0;
        characterNameText.text = characterNameLines[lineIndex];
        SelectProfileImage();
        SelectCharacterName();
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;

        if(lineIndex < dialogueLines.Length)
        {
            SelectCharacterName();
            SelectProfileImage();
            StartCoroutine(ShowLine());
        }
        else
        {
            didConversationStart = false;
            conversationPanel.SetActive(false);

            if(multipleChoiceDialogue != null)
            {
                multipleChoiceDialogue.dialoguePanel.SetActive(false);
                multipleChoiceDialogue.didDialogueStart = false;
                dialogueMark.SetActive(true);

                // PROVISIONAL
                player.GetComponent<PlayerMovement>().isPlayerTalking = false;
                player.GetComponent<PlayerMovement>().isPlayerInspecting = false;
                PlayerEvents.FinishTalkingWithoutClue();
            }
            else if(storyPhaseDialogue != null)
            {
                storyPhaseDialogue.dialoguePanel.SetActive(false);
                storyPhaseDialogue.didConversationStart = false;
                dialogueMark.SetActive(true);

                // PROVISIONAL
                player.GetComponent<PlayerMovement>().isPlayerTalking = false;
                player.GetComponent<PlayerMovement>().isPlayerInspecting = false;
                PlayerEvents.FinishTalkingWithoutClue();
            }
            else if(playerLogicManager != null)
            {
                playerLogicManager.dialoguePanel.SetActive(false);
                didConversationStart = false;
                playerLogicManager.IsInspectionComplete = false;

                // PROVISIONAL
                player.GetComponent<PlayerMovement>().isPlayerTalking = false;
                player.GetComponent<PlayerMovement>().isPlayerInspecting = false;
                PlayerEvents.FinishTalkingWithoutClue();
            }
            else if(inspectDialogue != null)
            {
                inspectDialogue.dialoguePanel.SetActive(false);
                inspectDialogue.didConversationStart = false;
                player.GetComponent<PlayerLogicManager>().IsInspectionComplete = false;

                if(inspectDialogue.didObjectAdvanceStory)
                {
                    if(GetComponent<AdvanceStoryManager>() != null)
                    {
                        GetComponent<AdvanceStoryManager>().AdvanceStoryState();
                    }
                    
                    inspectDialogue.didObjectAdvanceStory = false;
                }

                if(isClueUnlockTrigger && GetComponent<InspectDialogue>().isClueDialogueFinish)
                {
                    GetComponent<CluesDisplayManager>().ShowDiscoveredClue();
                    GetComponent<InspectDialogue>().isClueDialogueFinish = false;
                }
                else
                {
                    PlayerEvents.FinishTalkingWithoutClue();
                    GetComponent<InspectDialogue>().isClueDialogueFinish = false;
                }

                // PROVISIONAL
                player.GetComponent<PlayerMovement>().isPlayerInspecting = false;
            }
            else
            {
                // PROVISIONAL
                player.GetComponent<PlayerMovement>().isPlayerTalking = false;
                player.GetComponent<PlayerMovement>().isPlayerInspecting = false;
                PlayerEvents.FinishTalkingWithoutClue();
            }

            conversationPhase = ConversationPhase.Ended;
        }
    }

    private void ShowLineDirectly()
    {
        StopAllCoroutines();
        characterNameText.text = characterNameLines[lineIndex];
        SelectProfileImage();        
        SelectCharacterName();
        dialogueText.text = dialogueLines[lineIndex];
    }

    private void SelectCharacterName()
    {
        if(characterNameLines[lineIndex] == "Player")
        {
            characterNameText.text = player.GetComponent<PlayerLogicManager>().PlayerName;
        }
        else
        {
            characterNameText.text = characterNameLines[lineIndex];
        }        
    }

    private void SelectProfileImage()
    {
        if(characterNameLines[lineIndex] == "Player")
        {
            profileImage.sprite = player.GetComponent<PlayerLogicManager>().PlayerImage;
        }
        else
        {
            profileImage.sprite = characterImage;
        }        
    }

    private IEnumerator ShowLine() 
    {
        dialogueText.text = string.Empty;
        int charIndex = 0;

        foreach(char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            
            if(charIndex % charsToPlaySound == 0)
            {
                typingDialogueAudioSource.Play();
            }

            charIndex++;
            yield return new WaitForSeconds(typingTime);
        }
    }

    // REVISAR ---------------------------------------------------------
    // Método para que otros scripts puedan comenzar la conversación
    public void StartConversation()
    {
        conversationPhase = ConversationPhase.Started;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(multipleChoiceDialogue != null || storyPhaseDialogue != null)
            {
                dialogueMark.SetActive(true);
            }

            if(inspectDialogue != null)
            {
                player.GetComponent<PlayerLogicManager>().IsInspectObjectInRange = true;
            }

            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(multipleChoiceDialogue != null || storyPhaseDialogue != null)
            {
                dialogueMark.SetActive(false);
            }

            if(inspectDialogue != null)
            {
                player.GetComponent<PlayerLogicManager>().IsInspectObjectInRange = false;
            }

            isPlayerInRange = false;
            didConversationStart = false;
        }
    }
}
