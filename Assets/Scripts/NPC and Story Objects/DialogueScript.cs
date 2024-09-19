using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private GameObject player;
    [SerializeField] private Image profileImage;
    [SerializeField] private Sprite characterImage;
    [SerializeField]private float typingTime = 0.05f;

    public GameObject conversationPanel;
    public string[] dialogueLines;
    public string[] characterNameLines;

    public bool isPlayerInRange;
    public bool didConversationStart;
    private int lineIndex;
    private MultipleChoiceDialogueScript multipleChoiceDialogueScript;
    private StoryPhaseDialogueScript storyPhaseDialogueScript;

    void Start()
    {
        multipleChoiceDialogueScript = GetComponent<MultipleChoiceDialogueScript>();
        storyPhaseDialogueScript = GetComponent<StoryPhaseDialogueScript>();
    }

    void Update()
    {
        if(multipleChoiceDialogueScript != null)
        {
            if(isPlayerInRange == true && multipleChoiceDialogueScript.didDialogueStart == true && !didConversationStart
                && multipleChoiceDialogueScript.didConversationStart)
            {
                StartDialogue();
            }

            if(isPlayerInRange == true && Input.GetKeyDown(KeyCode.Space) && multipleChoiceDialogueScript.didDialogueStart == true
                && didConversationStart) 
            {
                if(dialogueText.text == dialogueLines[lineIndex])
                {
                    NextDialogueLine();
                }
                else
                {
                    StopAllCoroutines();
                    characterNameText.text = characterNameLines[lineIndex];
                    SelectProfileImage();
                    dialogueText.text = dialogueLines[lineIndex];
                }
            }
        }

        if(storyPhaseDialogueScript != null)
        {
            if(isPlayerInRange == true && !didConversationStart && storyPhaseDialogueScript.didConversationStart)
            {
                StartDialogue();
            }

            if(isPlayerInRange == true && Input.GetKeyDown(KeyCode.Space) && didConversationStart) 
            {
                if(dialogueText.text == dialogueLines[lineIndex])
                {
                    NextDialogueLine();
                }
                else
                {
                    StopAllCoroutines();
                    characterNameText.text = characterNameLines[lineIndex];
                    SelectProfileImage();
                    dialogueText.text = dialogueLines[lineIndex];
                }
            }
        }
    }

    private void StartDialogue()
    {
        didConversationStart = true;

        if(multipleChoiceDialogueScript != null)
        {
            multipleChoiceDialogueScript.choicePanel.SetActive(false);
        }

        conversationPanel.SetActive(true);
        dialogueMark.SetActive(false);
        lineIndex = 0;
        characterNameText.text = characterNameLines[lineIndex];
        SelectProfileImage();
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;

        if(lineIndex < dialogueLines.Length)
        {
            characterNameText.text = characterNameLines[lineIndex];
            SelectProfileImage();
            StartCoroutine(ShowLine());
        }
        else
        {
            didConversationStart = false;
            conversationPanel.SetActive(false);

            if(multipleChoiceDialogueScript != null)
            {
                multipleChoiceDialogueScript.dialoguePanel.SetActive(false);
                multipleChoiceDialogueScript.didDialogueStart = false;
            }
            if(storyPhaseDialogueScript != null)
            {
                storyPhaseDialogueScript.dialoguePanel.SetActive(false);
                storyPhaseDialogueScript.didConversationStart = false;
            }

            dialogueMark.SetActive(true);
            player.GetComponent<PlayerMovement>().isPlayerTalking = false;
        }
    }

    private void SelectProfileImage()
    {
        if(player.GetComponent<PlayerLogicManager>().playerName == characterNameLines[lineIndex])
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
            dialogueMark.SetActive(true);
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            dialogueMark.SetActive(false);
            isPlayerInRange = false;
            didConversationStart = false;
        }
    }
}
