using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLogicManager : MonoBehaviour
{
    [SerializeField] private GameObject inspectProgressBar;
    [SerializeField] private float holdInspectKeyTime = 2.0f;
    [SerializeField, TextArea(4,5)] private string[] defaultInspectDialogueLines;
    [SerializeField] private string[] defaultInspectCharacterNameLines;
    
    public GameObject inspectLayout;
    public GameObject dialoguePanel;
    public Sprite playerImage;
    public string playerName = "Player";
    public bool isObjectInspected = false;
    public bool isEmptyInspected = false;
    public bool isInspectInRange;
    public bool didConversationStart;
    public bool isTutorialInProgress;

    private float holdInspectKeyDuration = 0f;

    void Update()
    {
        if(!isTutorialInProgress && !GetComponent<PlayerMovement>().isPlayerTalking)
        {
            InspectCheck();
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Guilty: " + GameLogicManager.Instance.guilty);
            Debug.Log("First Clue: " + GameLogicManager.Instance.firstClue);
            Debug.Log("Second Clue: " + GameLogicManager.Instance.secondClue);
            Debug.Log("Third Clue: " + GameLogicManager.Instance.thirdClue);
            Debug.Log("Story Phase: " + GameLogicManager.Instance.storyPhase);
            Debug.Log("Last Puzzle Complete: " + GameLogicManager.Instance.lastPuzzleComplete);
            string suspectsContent = string.Join(", ", GameLogicManager.Instance.knownSuspects);
            Debug.Log("Known Suspects: " + suspectsContent);
            string tutorialsContent = string.Join(", ", GameLogicManager.Instance.knownTutorials);
            Debug.Log("Known Tutorials: " + tutorialsContent);
            string dialoguesContent = string.Join(", ", GameLogicManager.Instance.knownDialogues);
            Debug.Log("Known Dialogues: " + dialoguesContent);
            string supportsContent = string.Join(", ", GameStateManager.Instance.lastPuzzleSupports);
            Debug.Log("Last Puzzle Supports: " + supportsContent);
            Debug.Log("Last Puzzle Points: " + GameStateManager.Instance.lastPuzzlePoints);            
            Debug.Log("Final Malo: " + GameLogicManager.Instance.isBadEnding);       
            Debug.Log("Oportunidades para el Final: " + GameLogicManager.Instance.endOpportunities);
        }
    }

    private void InspectCheck()
    {
        if (Input.GetKey(KeyCode.Q) && !isObjectInspected && !isEmptyInspected)
        {
            GetComponent<PlayerMovement>().isPlayerInspecting = true;
            inspectLayout.SetActive(true);

            holdInspectKeyDuration += Time.deltaTime;
            float progress = holdInspectKeyDuration / holdInspectKeyTime;
            inspectProgressBar.GetComponent<Image>().fillAmount = progress;

            if(holdInspectKeyDuration >= holdInspectKeyTime)
            {
                inspectProgressBar.GetComponent<Image>().fillAmount = 1;

                if(isInspectInRange)
                {
                    isObjectInspected = true;
                }
                else
                {
                    isEmptyInspected = true;
                    showDefaultMessage();
                }
            }
        }
        else if(isEmptyInspected || isObjectInspected)
        {
            ResetProgress();
        }
        else if((!isEmptyInspected || !isObjectInspected) && Input.GetKeyUp(KeyCode.Q))
        {
            GetComponent<PlayerMovement>().isPlayerInspecting = false;
            ResetProgress();
        }
    }

    public void showDefaultMessage()
    {
        didConversationStart = true;
        GetComponent<DialogueManager>().dialogueLines = defaultInspectDialogueLines;
        GetComponent<DialogueManager>().characterNameLines = defaultInspectCharacterNameLines;
    }

    private void ResetProgress()
    {
        inspectLayout.SetActive(false);
        holdInspectKeyDuration = 0f;
        inspectProgressBar.GetComponent<Image>().fillAmount = 0f;
    }
}
