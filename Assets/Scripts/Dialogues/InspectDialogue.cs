using UnityEngine;

public class InspectDialogue : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject virtualCamara;
    [SerializeField, TextArea(4,5)] private string[] dialogueLines;
    [SerializeField] private string[] characterNameLines;
    [SerializeField, TextArea(4,5)] private string[] dialogueLinesToRecentPhase;
    [SerializeField] private string[] characterNameLinesToRecentPhase;
    [SerializeField, TextArea(4,5)] private string[] dialogueLinesToPastPhase;
    [SerializeField] private string[] characterNameLinesToPastPhase;
    [SerializeField] private int storyPhaseToUnlockDialogue;

    [SubphaseSelector]
    [SerializeField] private string selectedSubphase;

    public bool didConversationStart;
    public GameObject dialoguePanel;
    public bool isPuzzleTriggerObject;
    public string puzzleAssociated;
    public bool isStoryAdvanced;
    public bool didObjectAdvanceStory;
    public bool isPuzzleReturn = false;
    public bool isClueDialogueFinish = false;
    public string puzzleScene;

    void Update()
    {
        if (GetComponent<DialogueManager>().isPlayerInRange)
        {
            if(player.GetComponent<PlayerLogicManager>().isObjectInspected == true)
            {
                // QUITAR AUX
                if(storyPhaseToUnlockDialogue == GameLogicManager.Instance.StoryPhaseAux)
                {
                    didConversationStart = true;
                    didObjectAdvanceStory = true;
                    GetComponent<DialogueManager>().dialogueLines = dialogueLines;
                    GetComponent<DialogueManager>().characterNameLines = characterNameLines;
                }
                // QUITAR AUX
                else if(storyPhaseToUnlockDialogue == (GameLogicManager.Instance.StoryPhaseAux - 1))
                {
                    didConversationStart = true;
                    GetComponent<DialogueManager>().dialogueLines = dialogueLinesToRecentPhase;
                    GetComponent<DialogueManager>().characterNameLines = characterNameLinesToRecentPhase;
                }
                // QUITAR AUX
                else if(storyPhaseToUnlockDialogue < (GameLogicManager.Instance.StoryPhaseAux + 1))
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
                if(GetComponent<DialogueManager>().isClueUnlockTrigger)
                {
                    isClueDialogueFinish = true;
                }
                player.GetComponent<PlayerMovement>().isPlayerTalking = true;
                GetComponent<DialogueManager>().dialogueLines = dialogueLinesToRecentPhase;
                GetComponent<DialogueManager>().characterNameLines = characterNameLinesToRecentPhase;
            }
        }

        if(GameStateManager.Instance.isPuzzleIncomplete)
        {
            GameStateManager.Instance.isPuzzleIncomplete = false;
            GameLogicManager.Instance.LoadTemporarilyPosition();
        }        
    }
}
