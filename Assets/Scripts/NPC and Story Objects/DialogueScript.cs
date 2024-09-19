using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text chracterNameText;
    [SerializeField] private GameObject player;
    [SerializeField] private Image profileImage;
    [SerializeField] private Sprite characterImage;
    [SerializeField]private float typingTime = 0.05f;

    public GameObject conversationPanel;
    public string[] dialogueLines;
    public string[] chracterNameLines;

    public bool isPlayerInRange;
    public bool didConversationStart;
    private int lineIndex;

    void Update()
    {
        if(isPlayerInRange == true && GetComponent<MultipleChoiceScript>().didDialogueStart == true && !didConversationStart
            && GetComponent<MultipleChoiceScript>().didConversationStart)
        {
            StartDialogue();
        }

        if(isPlayerInRange == true && Input.GetKeyDown(KeyCode.Space) && GetComponent<MultipleChoiceScript>().didDialogueStart == true
            && didConversationStart) 
        {
            if(dialogueText.text == dialogueLines[lineIndex])
            {
                NextDialogueLine();
            }
            else
            {
                StopAllCoroutines();
                chracterNameText.text = chracterNameLines[lineIndex];
                SelectProfileImage();
                dialogueText.text = dialogueLines[lineIndex];
            }
        }
    }

    private void StartDialogue()
    {
        didConversationStart = true;
        GetComponent<MultipleChoiceScript>().choicePanel.SetActive(false);
        conversationPanel.SetActive(true);
        dialogueMark.SetActive(false);
        lineIndex = 0;
        chracterNameText.text = chracterNameLines[lineIndex];
        SelectProfileImage();
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;

        if(lineIndex < dialogueLines.Length)
        {
            chracterNameText.text = chracterNameLines[lineIndex];
            SelectProfileImage();
            StartCoroutine(ShowLine());
        }
        else
        {
            didConversationStart = false;
            conversationPanel.SetActive(false);
            GetComponent<MultipleChoiceScript>().choicePanel.SetActive(true);
            GetComponent<MultipleChoiceScript>().dialoguePanel.SetActive(false);
            GetComponent<MultipleChoiceScript>().didDialogueStart = false;
            dialogueMark.SetActive(true);
            player.GetComponent<PlayerMovement>().isPlayerTalking = false;
        }
    }

    private void SelectProfileImage()
    {
        if(player.GetComponent<PlayerLogicManager>().playerName == chracterNameLines[lineIndex])
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
