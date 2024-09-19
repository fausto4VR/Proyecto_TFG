using System.Collections.Generic;
using UnityEngine;

public class StoryPhaseDialogueScript : MonoBehaviour
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
    
    public GameObject dialoguePanel;
    public bool didConversationStart;

    private List<string[]> dialogueLinesList;
    private List<string[]> characterNamesList;
    private int storyPhaseIndex;

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
    }

    void Update()
    {
        if (GetComponent<DialogueScript>().isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                dialoguePanel.SetActive(true);
                didConversationStart = true;
                storyPhaseIndex = GameLogicManager.Instance.storyPhase;

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
                    GetComponent<DialogueScript>().dialogueLines = defaultDialogueLines;
                    GetComponent<DialogueScript>().characterNameLines = defaultCharacterNameLines;
                }
                else
                {
                    GetComponent<DialogueScript>().dialogueLines = dialogueLinesList[dialogueIndex];
                    GetComponent<DialogueScript>().characterNameLines = characterNamesList[dialogueIndex];
                }                
            }
        }
    }
}
