using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text chracterNameText;
    [SerializeField] private GameObject player;
    [SerializeField] private Image profileImage;
    [SerializeField] private Sprite characterImage;
    [SerializeField, TextArea(4,5)] private string[] dialogueLines;
    [SerializeField] private string[] chracterNameLines;
    [SerializeField]private float typingTime = 0.05f;

    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;

    void Update()
    {
        if(isPlayerInRange == true && Input.GetKeyDown(KeyCode.E) && !didDialogueStart) 
        {
            StartDialogue();
        }

        if(isPlayerInRange == true && Input.GetKeyDown(KeyCode.Space) && didDialogueStart) 
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
        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        dialogueMark.SetActive(false);
        lineIndex = 0;
        chracterNameText.text = chracterNameLines[lineIndex];
        SelectProfileImage();
        player.GetComponent<PlayerMovement>().isPlayerTalking = true;
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
            didDialogueStart = false;
            dialoguePanel.SetActive(false);
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
            didDialogueStart = false;
        }
    }
}
