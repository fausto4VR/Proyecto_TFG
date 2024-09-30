using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public bool isPlayerInRange;
    public bool didConversationStart;
    private int lineIndex;
    private MultipleChoiceDialogue multipleChoiceDialogue;
    private StoryPhaseDialogue storyPhaseDialogue;
    private InspectDialogue inspectDialogue;
    private PlayerLogicManager playerLogicManager;
    void Start()
    {
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

        if(playerLogicManager != null)
        {
            if(!didConversationStart && playerLogicManager.didConversationStart)
            {
                playerLogicManager.inspectLayout.SetActive(false);
                playerLogicManager.dialoguePanel.SetActive(true);
                StartDialogue();
            }

            if((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && didConversationStart) 
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

    private void StartDialogue()
    {
        didConversationStart = true;

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
            }
            else if(storyPhaseDialogue != null)
            {
                storyPhaseDialogue.dialoguePanel.SetActive(false);
                storyPhaseDialogue.didConversationStart = false;
                dialogueMark.SetActive(true);
            }
            else if(playerLogicManager != null)
            {
                playerLogicManager.dialoguePanel.SetActive(false);
                playerLogicManager.didConversationStart = false;
                playerLogicManager.isEmptyInspected = false;
                playerLogicManager.isObjectInspected = false;
            }

            else if(inspectDialogue != null)
            {
                inspectDialogue.dialoguePanel.SetActive(false);
                inspectDialogue.didConversationStart = false;
                player.GetComponent<PlayerLogicManager>().isObjectInspected = false;

                if(inspectDialogue.didObjectAdvanceStory)
                {
                    if(GetComponent<AvanceStory>() != null)
                    {
                        inspectDialogue.isStoryAdvanced = true;
                    }
                    
                    inspectDialogue.didObjectAdvanceStory = false;
                }

                if(isClueUnlockTrigger && GetComponent<InspectDialogue>().isClueDialogueFinish)
                {
                    isClueUnlock = true;
                    GetComponent<InspectDialogue>().isClueDialogueFinish = false;
                }
                else
                {
                    GetComponent<InspectDialogue>().isClueDialogueFinish = false;
                }
            }

            player.GetComponent<PlayerMovement>().isPlayerTalking = false;
            player.GetComponent<PlayerMovement>().isPlayerInspecting = false;
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
            characterNameText.text = player.GetComponent<PlayerLogicManager>().playerName;
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
            profileImage.sprite = player.GetComponent<PlayerLogicManager>().playerImage;
        }
        else
        {
            profileImage.sprite = characterImage;
        }        
    }

    private IEnumerator ShowLine() 
    {
        dialogueText.text = string.Empty;

        foreach(char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }
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
                player.GetComponent<PlayerLogicManager>().isInspectInRange = true;
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
                player.GetComponent<PlayerLogicManager>().isInspectInRange = false;
            }

            isPlayerInRange = false;
            didConversationStart = false;
        }
    }
}
