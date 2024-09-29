using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultipleChoiceDialogue: MonoBehaviour
{

    [SerializeField, TextArea(4,5)] private string[] firstDialogueLines;
    [SerializeField] private string[] firstCharacterNameLines;
    [SerializeField, TextArea(4,5)] private string[] secondDialogueLines;
    [SerializeField] private string[] secondCharacterNameLines;
    [SerializeField, TextArea(4,5)] private string[] thirdDialogueLines;
    [SerializeField] private string[] thirdCharacterNameLines;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject thirdOptionKey;
    [SerializeField] private GameObject firstOptionButton;
    [SerializeField] private GameObject secondOptionButton;
    [SerializeField] private GameObject  thirdOptionButton;
    [SerializeField] private int clueToUnlockDialogue;
    [SerializeField] private int suspectIndex;    
    [SerializeField] private TMP_Text firstOptionPanel;
    [SerializeField] private TMP_Text secondOptionPanel;
    [SerializeField] private TMP_Text thirdOptionPanel;
    [SerializeField] private string firstOptionText;
    [SerializeField] private string secondOptionText;
    [SerializeField] private string thirdOptionText;

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

        if (GetComponent<DialogueManager>().isPlayerInRange)
        {
            firstOptionButton.GetComponent<Button>().onClick.RemoveAllListeners();
            firstOptionButton.GetComponent<Button>().onClick.AddListener(() => ChooseFirstOption());
            
            secondOptionButton.GetComponent<Button>().onClick.RemoveAllListeners();
            secondOptionButton.GetComponent<Button>().onClick.AddListener(() => ChooseSecondOption());
            
            thirdOptionButton.GetComponent<Button>().onClick.RemoveAllListeners();
            thirdOptionButton.GetComponent<Button>().onClick.AddListener(() => ChooseThirdOption());

            if (Input.GetKeyDown(KeyCode.E) && !player.GetComponent<PlayerMovement>().isPlayerDoingTutorial)
            {
                GameLogicManager.Instance.knownSuspects[suspectIndex] = true;

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
                ChooseFirstOption();
            }

            if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                ChooseSecondOption();
            }

            if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3) && isDialogueUnlock)
            {
                ChooseThirdOption();
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
        else if(clueToUnlockDialogue == 4)
        {
            isDialogueUnlock = true;
        }
    }

    public void ChooseFirstOption()
    {
        GetComponent<DialogueManager>().dialogueLines = firstDialogueLines;
        GetComponent<DialogueManager>().characterNameLines = firstCharacterNameLines;
        GoToConversation();
    }

    public void ChooseSecondOption()
    {
        GetComponent<DialogueManager>().dialogueLines = secondDialogueLines;
        GetComponent<DialogueManager>().characterNameLines = secondCharacterNameLines;
        GoToConversation();
    }

    public void ChooseThirdOption()
    {
        GetComponent<DialogueManager>().dialogueLines = thirdDialogueLines;
        GetComponent<DialogueManager>().characterNameLines = thirdCharacterNameLines;
        GoToConversation();
    }

    private void GoToConversation()
    {
        choicePanel.SetActive(false);
        GetComponent<DialogueManager>().conversationPanel.SetActive(true);
        didConversationStart = true;
    }

    private void StartDialogue()
    {
        player.GetComponent<PlayerMovement>().isPlayerTalking = true;
        choicePanel.SetActive(true);
        dialoguePanel.SetActive(true);

        firstOptionPanel.text = firstOptionText;
        secondOptionPanel.text = secondOptionText;
        thirdOptionPanel.text = thirdOptionText;

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
