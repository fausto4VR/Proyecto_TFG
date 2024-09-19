using UnityEngine;

public class MultipleChoiceDialogueScript : MonoBehaviour
{

    [SerializeField, TextArea(4,5)] private string[] firstDialogueLines;
    [SerializeField] private string[] firstCharacterNameLines;
    [SerializeField, TextArea(4,5)] private string[] secondDialogueLines;
    [SerializeField] private string[] secondCharacterNameLines;
    [SerializeField, TextArea(4,5)] private string[] thirdDialogueLines;
    [SerializeField] private string[] thirdCharacterNameLines;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject thirdOptionKey;
    [SerializeField] private GameObject thirdOptionButton;
    [SerializeField] private int clueToUnlockDialogue;

    public GameObject dialoguePanel;
    public GameObject choicePanel;
    public bool didDialogueStart;
    public bool didConversationStart;
    
    private bool isDialogueUnlock;

    void Update()
    {
        CheckDialogueUnlock();

        if(isDialogueUnlock)
        {
            thirdOptionKey.SetActive(true);
            thirdOptionButton.SetActive(true);
        }

        if (GetComponent<DialogueScript>().isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!didDialogueStart)
                {
                    StartDialogue();
                }
                else if (didDialogueStart && !didConversationStart)
                {
                    EndDialogue();
                }
            }
        }

        if(didDialogueStart)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                GetComponent<DialogueScript>().dialogueLines = firstDialogueLines;
                GetComponent<DialogueScript>().characterNameLines = firstCharacterNameLines;
                GoToConversation();
            }

            if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                GetComponent<DialogueScript>().dialogueLines = secondDialogueLines;
                GetComponent<DialogueScript>().characterNameLines = secondCharacterNameLines;
                GoToConversation();
            }

            if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3) && isDialogueUnlock)
            {
                GetComponent<DialogueScript>().dialogueLines = thirdDialogueLines;
                GetComponent<DialogueScript>().characterNameLines = thirdCharacterNameLines;
                GoToConversation();
            }
        }
    }

    private void CheckDialogueUnlock()
    {
        if(clueToUnlockDialogue == 1 && !string.IsNullOrEmpty(GameLogicManager.Instance.firstClue))
        {
            isDialogueUnlock = true;
        }
        else if(clueToUnlockDialogue == 2 && !string.IsNullOrEmpty(GameLogicManager.Instance.secondClue))
        {
            isDialogueUnlock = true;
        }
        else if(clueToUnlockDialogue == 3 && !string.IsNullOrEmpty(GameLogicManager.Instance.thirdClue))
        {
            isDialogueUnlock = true;
        }
    }

    private void GoToConversation()
    {
        choicePanel.SetActive(false);
        GetComponent<DialogueScript>().conversationPanel.SetActive(true);
        didConversationStart = true;
    }

    private void StartDialogue()
    {
        player.GetComponent<PlayerMovement>().isPlayerTalking = true;
        choicePanel.SetActive(true);
        dialoguePanel.SetActive(true);
        didDialogueStart = true;
        didConversationStart = false;
    }

    private void EndDialogue()
    {
        player.GetComponent<PlayerMovement>().isPlayerTalking = false;
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        didDialogueStart = false;
    }
}
