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

    private float holdInspectKeyDuration = 0f;

    void Update()
    {
        InspectCheck();
 
        if(Input.GetKeyDown(KeyCode.G))
        {
            GameStateManager.Instance.SaveData();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            GameStateManager.Instance.LoadData();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            GameStateManager.Instance.ResetData();
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
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            //
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
