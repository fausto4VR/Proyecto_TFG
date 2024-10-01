using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TheEndManager : MonoBehaviour
{
    [SerializeField] private GameObject theEndPanel;
    [SerializeField] private GameObject theEndMark;
    [SerializeField] private  GameObject guiltyDropdown;
    [SerializeField] private  GameObject player;    
    [SerializeField] private  TMP_Text opportunitiesText;
    [SerializeField] private  GameObject badEndingScreen;
    [SerializeField] private  GameObject goodEndingScreen;
    [SerializeField] private  GameObject anotherTryScreen;
    [SerializeField] private Sprite suspectSprite1;
    [SerializeField] private Sprite suspectSprite2;
    [SerializeField] private Sprite suspectSprite3;
    [SerializeField] private Sprite suspectSprite4;
    [SerializeField] private Sprite suspectSprite5;
    [SerializeField] private Sprite suspectSprite6;
    [SerializeField] private Sprite suspectSprite7;
    [SerializeField] private Sprite suspectSprite8;
    [SerializeField] private  TMP_Text guiltyAnotherTryText;
    [SerializeField] private  TMP_Text guiltyFailText;
    [SerializeField] private  TMP_Text guiltySuccessText;
    [SerializeField] private GameObject fatherNPC;
    [SerializeField] private GameObject victimNPC;
    [SerializeField] private Image failImageAnotherTryEnding;
    [SerializeField] private Image failImageBadEnding;
    [SerializeField] private Image correctImageBadEnding;
    [SerializeField] private Image correctImageGoodEnding;
    [SerializeField] private GameObject failSectionBadEnding;

    private bool isPlayerInRange;
    private bool isTheEndPanelShown;
    private bool isFinalScreenShown;
    private bool isSuspectKnown;    
    private List<Sprite> suspectSprites = new List<Sprite>();

    void Start()
    {
        suspectSprites.Add(suspectSprite1);
        suspectSprites.Add(suspectSprite2);
        suspectSprites.Add(suspectSprite3);
        suspectSprites.Add(suspectSprite4);
        suspectSprites.Add(suspectSprite5);
        suspectSprites.Add(suspectSprite6);
        suspectSprites.Add(suspectSprite7);
        suspectSprites.Add(suspectSprite8);
    }

    void Update()
    {
        if(!isFinalScreenShown && isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            DisplayTheEndPanel();
        }

        if(isTheEndPanelShown && Input.GetKeyDown(KeyCode.Y))
        {
            SendGuilty();
        }

        if(isTheEndPanelShown && Input.GetKeyDown(KeyCode.N))
        {
            GoBackFromTheEndPanel();
        }

        if(isFinalScreenShown && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            GoBackFromTheFinalScreen();
        }  

        //QUITAR
        if(Input.GetKeyDown(KeyCode.R))
        {
            GameStateManager.Instance.ResetData();
        }    
    }

    public void DisplayTheEndPanel()
    {
        if(theEndPanel.activeInHierarchy == true)
        {
            GoBackFromTheEndPanel();
        }
        else if(theEndPanel.activeInHierarchy == false)
        {
            if(GameLogicManager.Instance.endOpportunities == 2)
            {
                opportunitiesText.text = "Te quedan 2 intentos.";
            }
            else if(GameLogicManager.Instance.endOpportunities == 1)
            {
                opportunitiesText.text = "Te queda 1 intento.";
            }
            else if(GameLogicManager.Instance.endOpportunities == 0)
            {
                opportunitiesText.text = "No te quedan más intentos.";
            }

            UploadDropdown();
            player.GetComponent<PlayerMovement>().isPlayerInspecting = true;
            isTheEndPanelShown = true;
            theEndPanel.SetActive(true);
        }
    }

    public void SendGuilty()
    {
        if(isSuspectKnown)
        {
            if(GameLogicManager.Instance.endOpportunities == 0)
            {
                ShowEnding();
            }
            else
            {
                int selectedIndex = guiltyDropdown.GetComponent<TMP_Dropdown>().value;
                string selectedGuilty = guiltyDropdown.GetComponent<TMP_Dropdown>().options[selectedIndex].text;

                if(GameLogicManager.Instance.guilty == selectedGuilty)
                {
                    GoToGoodEnding();
                }
                else
                {
                    if(GameLogicManager.Instance.endOpportunities == 2)
                    {
                        GoToAnotherTry(selectedGuilty);
                    }
                    else if(GameLogicManager.Instance.endOpportunities == 1)
                    {
                        GoToBadEnding(selectedGuilty);
                    }
                }
            }
        }
    }

    public void GoBackFromTheFinalScreen()
    {
        isFinalScreenShown = false;
        player.GetComponent<PlayerMovement>().isPlayerInspecting = false;
        GameStateManager.Instance.SaveData();

        if(anotherTryScreen.activeInHierarchy == true)
        {
            anotherTryScreen.SetActive(false);
        }
        else if(badEndingScreen.activeInHierarchy == true)
        {
            badEndingScreen.SetActive(false);
        }
        else if(goodEndingScreen.activeInHierarchy == true)
        {
            goodEndingScreen.SetActive(false);
        }
    }

    public void GoBackFromTheEndPanel()
    {
        player.GetComponent<PlayerMovement>().isPlayerInspecting = false;
        isTheEndPanelShown = false;
        theEndPanel.SetActive(false);
    }

    private void ShowEnding()
    {
        if(GameLogicManager.Instance.isBadEnding)
        {
            isTheEndPanelShown = false;
            theEndPanel.SetActive(false);
            badEndingScreen.SetActive(true);
            isFinalScreenShown = true;
            guiltyFailText.text = "El culpable era " + GameLogicManager.Instance.guilty + " y no lo has elegido.";
            failSectionBadEnding.SetActive(false);

            for(int i = 0; i < GameLogicManager.Instance.guiltyNames.Count; i++)
            {
                if(GameLogicManager.Instance.guiltyNames[i] == GameLogicManager.Instance.guilty)
                {
                    correctImageBadEnding.sprite = suspectSprites[i];
                }
            }
        }
        else
        {
            isTheEndPanelShown = false;
            theEndPanel.SetActive(false);
            goodEndingScreen.SetActive(true);
            isFinalScreenShown = true;
            guiltySuccessText.text = "El culpable era " + GameLogicManager.Instance.guilty + " y lo has elegido.";

            for(int i = 0; i < GameLogicManager.Instance.guiltyNames.Count; i++)
        {
            if(GameLogicManager.Instance.guiltyNames[i] == GameLogicManager.Instance.guilty)
            {
                correctImageGoodEnding.sprite = suspectSprites[i];
            }
        }
        }
    }

    private void GoToBadEnding(string selectedGuilty)
    {
        isTheEndPanelShown = false;
        theEndPanel.SetActive(false);
        badEndingScreen.SetActive(true);
        guiltyFailText.text = "El culpable era " + GameLogicManager.Instance.guilty + " y has elegido a " + selectedGuilty + ".";
        
        for(int i = 0; i < GameLogicManager.Instance.guiltyNames.Count; i++)
        {
            if(GameLogicManager.Instance.guiltyNames[i] == selectedGuilty)
            {
                failImageBadEnding.sprite = suspectSprites[i];
            }

            if(GameLogicManager.Instance.guiltyNames[i] == GameLogicManager.Instance.guilty)
            {
                correctImageBadEnding.sprite = suspectSprites[i];
            }
        }

        GameLogicManager.Instance.endOpportunities = 0;
        GameLogicManager.Instance.isBadEnding = true;
        ShowFinalNPC();
        isFinalScreenShown = true;
    }

    private void GoToGoodEnding()
    {
        isTheEndPanelShown = false;
        theEndPanel.SetActive(false);
        goodEndingScreen.SetActive(true);
        guiltySuccessText.text = "El culpable era " + GameLogicManager.Instance.guilty + " y lo has elegido.";
        
        for(int i = 0; i < GameLogicManager.Instance.guiltyNames.Count; i++)
        {
            if(GameLogicManager.Instance.guiltyNames[i] == GameLogicManager.Instance.guilty)
            {
                correctImageGoodEnding.sprite = suspectSprites[i];
            }
        }

        GameLogicManager.Instance.endOpportunities = 0;
        GameLogicManager.Instance.isBadEnding = false;
        ShowFinalNPC();
        isFinalScreenShown = true;
    }

    private void GoToAnotherTry(string selectedGuilty)
    {
        isTheEndPanelShown = false;
        theEndPanel.SetActive(false);
        anotherTryScreen.SetActive(true);
        GameLogicManager.Instance.endOpportunities = 1;
        isFinalScreenShown = true;
        guiltyAnotherTryText.text = "El culpable no era " + selectedGuilty + " y lo has elegido.";

        for(int i = 0; i < GameLogicManager.Instance.guiltyNames.Count; i++)
        {
            if(GameLogicManager.Instance.guiltyNames[i] == selectedGuilty)
            {
                failImageAnotherTryEnding.sprite = suspectSprites[i];
            }
        }
    }
    
    private void UploadDropdown()
    {
        TMP_Dropdown guiltyDropdownOptions = guiltyDropdown.GetComponent<TMP_Dropdown>();
        guiltyDropdownOptions.ClearOptions();        
        List<string> options = new List<string>();

        for(int i = 0; i < GameLogicManager.Instance.guiltyNames.Count; i++)
        {
            if(GameLogicManager.Instance.knownSuspects[i])
            {
                options.Add(GameLogicManager.Instance.guiltyNames[i]);
            }
        }

        if(options.Count > 0)
        {
            guiltyDropdownOptions.AddOptions(options);
            isSuspectKnown = true;
        }
        else
        {
            options.Add("No conoces a ningún sospechoso");
            guiltyDropdownOptions.AddOptions(options);
            isSuspectKnown = false;
        }
    }

    private void ShowFinalNPC()
    {
        if(GameLogicManager.Instance.isBadEnding)
        {
            fatherNPC.SetActive(true);

            int storyPhaseToUnlockFinalDialogue = 400;
            for(int i = 0; i < GameLogicManager.Instance.guiltyNames.Count; i++)
            {
                if(GameLogicManager.Instance.guiltyNames[i] == GameLogicManager.Instance.guilty)
                {
                    GameLogicManager.Instance.storyPhase = storyPhaseToUnlockFinalDialogue;
                }

                storyPhaseToUnlockFinalDialogue += 5;
            }
        }
        else
        {
            victimNPC.SetActive(true);

            int storyPhaseToUnlockFinalDialogue = 200;
            for(int i = 0; i < GameLogicManager.Instance.guiltyNames.Count; i++)
            {
                if(GameLogicManager.Instance.guiltyNames[i] == GameLogicManager.Instance.guilty)
                {
                    GameLogicManager.Instance.storyPhase = storyPhaseToUnlockFinalDialogue;
                }
                
                storyPhaseToUnlockFinalDialogue += 5;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            theEndMark.SetActive(true);
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(theEndMark != null)
            {
                theEndMark.SetActive(false);                
            }
            isPlayerInRange = false;
        }
    }
}
