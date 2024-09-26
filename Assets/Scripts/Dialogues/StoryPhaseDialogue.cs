using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class StoryPhaseDialogue : MonoBehaviour
{
    [SerializeField, TextArea(4,5)] private string[] defaultDialogueLines;
    [SerializeField] private string[] defaultCharacterNameLines;
    [SerializeField, TextArea(4,5)] private string[] firstDialogueLines;
    [SerializeField] private string[] firstCharacterNameLines;
    [SerializeField, TextArea(4,5)] private string[] secondDialogueLines;
    [SerializeField] private string[] secondCharacterNameLines;
    [SerializeField, TextArea(4,5)] private string[] thirdDialogueLines;
    [SerializeField] private string[] thirdCharacterNameLines;
    [SerializeField, TextArea(4,5)] private string[] fourthDialogueLines;
    [SerializeField] private string[] fourthCharacterNameLines;
    [SerializeField, TextArea(4,5)] private string[] fifthDialogueLines;
    [SerializeField] private string[] fifthCharacterNameLines;
    [SerializeField, TextArea(4,5)] private string[] sixthDialogueLines;
    [SerializeField] private string[] sixthCharacterNameLines;
    [SerializeField] private int[] storyPhasesToUnlockDialogue;
    [SerializeField] private GameObject player;
    [SerializeField] private bool isTriggerDialogue;    
    [SerializeField] private int knownDialogueIndex = 0;
    
    public GameObject dialoguePanel;
    public bool didConversationStart;

    private List<string[]> dialogueLinesList;
    private List<string[]> characterNamesList;
    private int storyPhaseIndex;
    private bool didDialogueTrigger;

    void Start()
    {
        dialogueLinesList = new List<string[]>();
        characterNamesList = new List<string[]>();

        dialogueLinesList.Add(firstDialogueLines);
        dialogueLinesList.Add(secondDialogueLines);
        dialogueLinesList.Add(thirdDialogueLines);
        dialogueLinesList.Add(fourthDialogueLines);
        dialogueLinesList.Add(fifthDialogueLines);
        dialogueLinesList.Add(sixthDialogueLines);

        characterNamesList.Add(firstCharacterNameLines);
        characterNamesList.Add(secondCharacterNameLines);
        characterNamesList.Add(thirdCharacterNameLines);
        characterNamesList.Add(fourthCharacterNameLines);
        characterNamesList.Add(fifthCharacterNameLines);
        characterNamesList.Add(sixthCharacterNameLines);

        // Para esperar que esté inicializada la lista de knownDialogues
        StartCoroutine(ExecuteAfterDelay());
    }

    void Update()
    {
        if (GetComponent<DialogueManager>().isPlayerInRange && !player.GetComponent<PlayerMovement>().isPlayerDoingTutorial)
        {
            if (Input.GetKeyDown(KeyCode.E) || (isTriggerDialogue && !didDialogueTrigger))
            {
                dialoguePanel.SetActive(true);
                didConversationStart = true;

                if(!didDialogueTrigger)
                {
                    didDialogueTrigger = true;
                    GameLogicManager.Instance.knownDialogues[knownDialogueIndex] = true;
                }

                storyPhaseIndex = GameLogicManager.Instance.storyPhase;
                player.GetComponent<PlayerMovement>().isPlayerTalking = true;

                int dialogueIndex = -1;
                for (int i = 0; i < storyPhasesToUnlockDialogue.Length; i++)
                {
                    if (storyPhasesToUnlockDialogue[i] <= storyPhaseIndex)
                    {
                        dialogueIndex = i;
                    }
                }

                if(dialogueIndex == -1)
                {
                    GetComponent<DialogueManager>().dialogueLines = defaultDialogueLines;
                    GetComponent<DialogueManager>().characterNameLines = defaultCharacterNameLines;
                }
                else
                {
                    GetComponent<DialogueManager>().dialogueLines = dialogueLinesList[dialogueIndex];
                    GetComponent<DialogueManager>().characterNameLines = characterNamesList[dialogueIndex];
                }                
            }
        }
    }

    private IEnumerator ExecuteAfterDelay()
    {
        yield return null;

        if (isTriggerDialogue)
        {
            didDialogueTrigger = GameLogicManager.Instance.knownDialogues[knownDialogueIndex];
        }
    }
}
