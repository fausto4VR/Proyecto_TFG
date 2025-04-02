using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CluesDisplayManager : MonoBehaviour
{
    [Header("Panel Section")]
    [SerializeField] private GameObject cluePanel;
    [SerializeField] private TMP_Text clueText;
    [SerializeField] private Image clueImage;

    [Header("Sprites Section")]
    [SerializeField] private Sprite firstClueSprite1;    
    [SerializeField] private Sprite firstClueSprite2;
    [SerializeField] private Sprite secondClueSprite1;
    [SerializeField] private Sprite secondClueSprite2;
    [SerializeField] private Sprite thirdClueSprite1;
    [SerializeField] private Sprite thirdClueSprite2;

    [Header("Sound Section")] 
    [SerializeField] private GameObject audioSourcesManager;
    
    [Header("Variable Section")]
    [SerializeField] private int currentClueIndex;

    [SubphaseSelector]
    [SerializeField] private string selectedSubphase;

    // QUITAR ----------------------------------------------------------
    [Header("QUITAR")]
    [SerializeField] private GameObject player;
    // -----------------------------------------------------------------
    
    // REVISAR AUDIO
    private AudioSource cluesAudioSource;

    void Start()
    {
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        cluesAudioSource = audioSources[2];
    }

    // Método para mostrar la pista descubierta al jugador
    public void ShowDiscoveredClue()
    {
        if(GameLogicManager.Instance.CurrentStoryPhase.CheckCurrentPhase(selectedSubphase) == SubphaseTemporaryOrder.IsCurrent)
        {
            // QUITAR ----------------------------------------------------------
            player.GetComponent<PlayerMovement>().isPlayerTalking = true;
            // -----------------------------------------------------------------

            cluePanel.SetActive(true);
            cluesAudioSource.Play();

            if(currentClueIndex == 0)
            {
                ShowFirstClue();
            }
            else if(currentClueIndex == 1)
            {
                ShowSecondClue();
            }
            else if(currentClueIndex == 2)
            {
                ShowThirdClue();
            }

            StartCoroutine(WaitForInputToClosePanel());
        }
    }

    // Método para mostrar la primera pista
    private void ShowFirstClue()
    {
        string clueTextString = "Has desbloqueado la primera pista: \n";
        clueTextString += GameLogicManager.Instance.Clues[0];

        if(clueTextString.Contains("Tiene los ojos marrones"))
        {
            clueImage.sprite = firstClueSprite1;

        }
        else if(clueTextString.Contains("Tiene los ojos verdes"))
        {
            clueImage.sprite = firstClueSprite2;
        }

        clueText.text = clueTextString;
    }

    // Método para mostrar la segunda pista
    private void ShowSecondClue()
    {
        string clueTextString = "Has desbloqueado la segunda pista: \n";
        clueTextString += GameLogicManager.Instance.Clues[1];

        if(clueTextString.Contains("Mechón de pelo negro"))
        {
            clueImage.sprite = secondClueSprite1;

        }
        else if(clueTextString.Contains("Mechón de pelo rubio"))
        {
            clueImage.sprite = secondClueSprite2;
        }

        clueText.text = clueTextString;
    }

    // Método para mostrar la tercera pista
    private void ShowThirdClue()
    {
        string clueTextString = "Has desbloqueado la tercera pista: \n";
        clueTextString += GameLogicManager.Instance.Clues[2];

        if(clueTextString.Contains("Tiene una cicatriz"))
        {
            clueImage.sprite = thirdClueSprite1;

        }
        else if(clueTextString.Contains("Tiene un pendiente"))
        {
            clueImage.sprite = thirdClueSprite2;
        }

        clueText.text = clueTextString;
    }

    // Corrutina para cerrar el panel que muestra la pista
    private IEnumerator WaitForInputToClosePanel()
    {        
        yield return new WaitForSeconds(cluesAudioSource.clip.length);
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0));
        cluePanel.SetActive(false);

        // QUITAR ----------------------------------------------------------
        player.GetComponent<PlayerMovement>().isPlayerTalking = false;
        // -----------------------------------------------------------------
    }
}
