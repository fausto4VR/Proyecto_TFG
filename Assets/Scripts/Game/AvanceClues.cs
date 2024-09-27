using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvanceClues : MonoBehaviour
{
    [SerializeField] private int storyPhase;
    [SerializeField] private int indexOfClue;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cluePanel;
    [SerializeField] private TMP_Text clueText;
    [SerializeField] private Image clueImage;
    [SerializeField] private Sprite firstClueSprite1;    
    [SerializeField] private Sprite firstClueSprite2;
    [SerializeField] private Sprite secondClueSprite1;
    [SerializeField] private Sprite secondClueSprite2;
    [SerializeField] private Sprite thirdClueSprite1;
    [SerializeField] private Sprite thirdClueSprite2;


    private bool isPlayerInRange;
    private bool isClueShown;

    void Start()
    {
        
    }

    void Update()
    {
        if(GetComponent<DialogueManager>().isClueUnlock && storyPhase == GameLogicManager.Instance.storyPhase && isPlayerInRange)
        {
            player.GetComponent<PlayerMovement>().isPlayerTalking = true;
            cluePanel.SetActive(true);

            if(indexOfClue == 1)
            {
                UnlockFirstClue();
            }
            else if(indexOfClue == 2)
            {
                UnlockSecondClue();
            }
            else if(indexOfClue == 3)
            {
                UnlockThirdClue();
            }

            GetComponent<DialogueManager>().isClueUnlock = false;
            StartCoroutine(DelayClueShown());
        }

        if(isClueShown)
        {
            if(Input.GetKeyDown(KeyCode.Space)) 
            {
                cluePanel.SetActive(false);
                isClueShown = false;
                player.GetComponent<PlayerMovement>().isPlayerTalking = false;
            }
        }        
    }

    private void UnlockFirstClue()
    {
        string clueTextString = "Has desbloqueado la primera pista: \n";

        if(GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[0] || 
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[1] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[6] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[7])
        {
            GameLogicManager.Instance.firstClue = "Tiene los ojos marrones";
            clueTextString += GameLogicManager.Instance.firstClue;
            clueImage.sprite = firstClueSprite1;

        }
        else if(GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[2] || 
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[3] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[4] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[5])
        {
            GameLogicManager.Instance.firstClue = "Tiene los ojos verde";
            clueTextString += GameLogicManager.Instance.firstClue;
            clueImage.sprite = firstClueSprite2;
        }

        clueText.text = clueTextString;
    }

    private void UnlockSecondClue()
    {
        string clueTextString = "Has desbloqueado la segunda pista: \n";

        if(GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[0] || 
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[1] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[4] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[5])
        {
            GameLogicManager.Instance.secondClue = "Mechón de pelo negro";
            clueTextString += GameLogicManager.Instance.secondClue;
            clueImage.sprite = secondClueSprite1;

        }
        else if(GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[2] || 
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[3] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[7] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[8])
        {
            GameLogicManager.Instance.secondClue = "Mechón de pelo rubio";
            clueTextString += GameLogicManager.Instance.secondClue;
            clueImage.sprite = secondClueSprite2;
        }

        clueText.text = clueTextString;
    }

    private void UnlockThirdClue()
    {
        string clueTextString = "Has desbloqueado la tercera pista: \n";

        if(GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[0] || 
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[2] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[4] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[6])
        {
            GameLogicManager.Instance.thirdClue = "Tiene una cicatriz";
            clueTextString += GameLogicManager.Instance.thirdClue;
            clueImage.sprite = thirdClueSprite1;

        }
        else if(GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[1] || 
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[3] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[5] ||
            GameLogicManager.Instance.guilty == GameLogicManager.Instance.guiltyNames[7])
        {
            GameLogicManager.Instance.thirdClue = "Tiene un pendiente";
            clueTextString += GameLogicManager.Instance.thirdClue;
            clueImage.sprite = thirdClueSprite2;
        }

        clueText.text = clueTextString;
    }

    private IEnumerator DelayClueShown()
    {
        yield return new WaitForSeconds(0.2f);
        isClueShown = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
