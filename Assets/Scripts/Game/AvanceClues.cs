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
    [SerializeField] private GameObject audioSourcesManager;


    private bool isPlayerInRange;
    private bool isClueShown;
    private AudioSource cluesAudioSource;

    void Start()
    {
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        cluesAudioSource = audioSources[2];
    }

    void Update()
    {
        // QUITAR AUX
        if(GetComponent<DialogueManager>().isClueUnlock && storyPhase == GameLogicManager.Instance.StoryPhaseAux && isPlayerInRange)
        {
            player.GetComponent<PlayerMovement>().isPlayerTalking = true;
            cluePanel.SetActive(true);
            cluesAudioSource.Play();

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
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) 
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

        if(GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[0] || 
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[1] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[6] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[7])
        {
            GameLogicManager.Instance.Clues[0] = "Tiene los ojos marrones";
            clueTextString += GameLogicManager.Instance.Clues[0];
            clueImage.sprite = firstClueSprite1;

        }
        else if(GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[2] || 
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[3] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[4] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[5])
        {
            GameLogicManager.Instance.Clues[0] = "Tiene los ojos verdes";
            clueTextString += GameLogicManager.Instance.Clues[0];
            clueImage.sprite = firstClueSprite2;
        }

        clueText.text = clueTextString;
    }

    private void UnlockSecondClue()
    {
        string clueTextString = "Has desbloqueado la segunda pista: \n";

        if(GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[0] || 
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[1] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[4] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[5])
        {
            GameLogicManager.Instance.Clues[1] = "Mechón de pelo negro";
            clueTextString += GameLogicManager.Instance.Clues[1];
            clueImage.sprite = secondClueSprite1;

        }
        else if(GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[2] || 
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[3] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[6] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[7])
        {
            GameLogicManager.Instance.Clues[1] = "Mechón de pelo rubio";
            clueTextString += GameLogicManager.Instance.Clues[1];
            clueImage.sprite = secondClueSprite2;
        }

        clueText.text = clueTextString;
    }

    private void UnlockThirdClue()
    {
        string clueTextString = "Has desbloqueado la tercera pista: \n";

        if(GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[0] || 
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[2] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[4] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[6])
        {
            GameLogicManager.Instance.Clues[2] = "Tiene una cicatriz";
            clueTextString += GameLogicManager.Instance.Clues[2];
            clueImage.sprite = thirdClueSprite1;

        }
        else if(GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[1] || 
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[3] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[5] ||
            GameLogicManager.Instance.Guilty == GameLogicManager.Instance.GuiltyNames[7])
        {
            GameLogicManager.Instance.Clues[2]  = "Tiene un pendiente";
            clueTextString += GameLogicManager.Instance.Clues[2];
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
