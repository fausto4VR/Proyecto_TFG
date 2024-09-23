using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectDialogue : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField, TextArea(4,5)] private string[] dialogueLines;
    [SerializeField] private string[] characterNameLines;
    [SerializeField, TextArea(4,5)] private string[] dialogueLinesToRecentPhase;
    [SerializeField] private string[] characterNameLinesToRecentPhase;
    [SerializeField, TextArea(4,5)] private string[] dialogueLinesToPastPhase;
    [SerializeField] private string[] characterNameLinesToPastPhase;
    [SerializeField] private int storyPhaseToUnlockDialogue;

    public bool didConversationStart;
    public GameObject dialoguePanel;
    public bool isPuzzleTriggerObject;
    public string puzzleAssociated;
    public bool isStoryAdvanced;
    public bool didObjectAdvanceStory;
    public bool isPuzzleReturn = false;

    void Update()
    {
        if (GetComponent<DialogueManager>().isPlayerInRange)
        {
            if(player.GetComponent<PlayerLogicManager>().isObjectInspected == true)
            {
                if(storyPhaseToUnlockDialogue == GameLogicManager.Instance.storyPhase)
                {
                    didConversationStart = true;
                    didObjectAdvanceStory = true;
                    GetComponent<DialogueManager>().dialogueLines = dialogueLines;
                    GetComponent<DialogueManager>().characterNameLines = characterNameLines;
                }
                else if(storyPhaseToUnlockDialogue == (GameLogicManager.Instance.storyPhase - 1))
                {
                    didConversationStart = true;
                    GetComponent<DialogueManager>().dialogueLines = dialogueLinesToRecentPhase;
                    GetComponent<DialogueManager>().characterNameLines = characterNameLinesToRecentPhase;
                }
                else if(storyPhaseToUnlockDialogue < (GameLogicManager.Instance.storyPhase + 1))
                {
                    didConversationStart = true;
                    GetComponent<DialogueManager>().dialogueLines = dialogueLinesToPastPhase;
                    GetComponent<DialogueManager>().characterNameLines = characterNameLinesToPastPhase;
                }
                else
                {
                    player.GetComponent<PlayerLogicManager>().showDefaultMessage();
                }
            }
            else if(isPuzzleReturn)
            {
                didConversationStart = true;
                isPuzzleReturn = false;
                player.GetComponent<PlayerMovement>().isPlayerTalking = true;
                GetComponent<DialogueManager>().dialogueLines = dialogueLinesToRecentPhase;
                GetComponent<DialogueManager>().characterNameLines = characterNameLinesToRecentPhase;
            }
        }        
    }
}
