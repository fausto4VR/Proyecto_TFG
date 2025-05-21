using System.Collections;
using UnityEngine;

// Enum de las pistas que existen en el juego
public enum ClueType
{
    FirstClue, SecondClue, ThirdClue, None
}

public class CluesDisplayManager : MonoBehaviour
{    
    [Header("Variable Section")]
    [SerializeField] private ClueType currentClue;
    [SerializeField] private float waitingTimeToClose = 0.5f;

    private string selectedSubphase;
    
    // REVISAR AUDIO
    private AudioSource cluesAudioSource;

    void Start()
    {
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        cluesAudioSource = audioSources[2];
        selectedSubphase = GetComponent<AdvanceStoryManager>().SelectedSubphase;
    }

    // Método para mostrar la pista descubierta al jugador
    public void ShowDiscoveredClue()
    {
        if(GameLogicManager.Instance.CurrentStoryPhase.ComparePhase(selectedSubphase) == SubphaseTemporaryOrder.IsRecentBefore)
        {
            GameLogicManager.Instance.UIManager.CluePanel.SetActive(true);
            cluesAudioSource.Play();

            bool[] clues = GameLogicManager.Instance.KnownClues;

            if(currentClue == ClueType.FirstClue)
            {
                ShowFirstClue();
                clues[0] = true;
            }
            else if(currentClue == ClueType.SecondClue)
            {
                ShowSecondClue();
                clues[1] = true;
            }
            else if(currentClue == ClueType.ThirdClue)
            {
                ShowThirdClue();
                clues[2] = true;
            }

            GameLogicManager.Instance.KnownClues = clues;

            StartCoroutine(WaitForInputToClosePanel());
        }
        else
        {
            PlayerEvents.FinishShowingInformation();
        }
    }

    // Método para mostrar la primera pista
    private void ShowFirstClue()
    {
        string clueTextString = "Has desbloqueado la primera pista: \n";
        clueTextString += GameLogicManager.Instance.Clues[0];

        if (GameLogicManager.Instance.FirstClueGroup1.Contains(GameLogicManager.Instance.Guilty))
        {
            GameLogicManager.Instance.UIManager.ClueImage.sprite = GameLogicManager.Instance.UIManager.FirstClueSprite1;

        }
        else if (GameLogicManager.Instance.FirstClueGroup2.Contains(GameLogicManager.Instance.Guilty))
        {
            GameLogicManager.Instance.UIManager.ClueImage.sprite = GameLogicManager.Instance.UIManager.FirstClueSprite2;

        }

        GameLogicManager.Instance.UIManager.ClueText.text = clueTextString;
    }

    // Método para mostrar la segunda pista
    private void ShowSecondClue()
    {
        string clueTextString = "Has desbloqueado la segunda pista: \n";
        clueTextString += GameLogicManager.Instance.Clues[1];

        if (GameLogicManager.Instance.SecondClueGroup1.Contains(GameLogicManager.Instance.Guilty))
        {
            GameLogicManager.Instance.UIManager.ClueImage.sprite = GameLogicManager.Instance.UIManager.SecondClueSprite1;
        }
        else if (GameLogicManager.Instance.SecondClueGroup2.Contains(GameLogicManager.Instance.Guilty))
        {
            GameLogicManager.Instance.UIManager.ClueImage.sprite = GameLogicManager.Instance.UIManager.SecondClueSprite2;
        }

        GameLogicManager.Instance.UIManager.ClueText.text = clueTextString;
    }

    // Método para mostrar la tercera pista
    private void ShowThirdClue()
    {
        string clueTextString = "Has desbloqueado la tercera pista: \n";
        clueTextString += GameLogicManager.Instance.Clues[2];

        if (GameLogicManager.Instance.ThirdClueGroup1.Contains(GameLogicManager.Instance.Guilty))
        {
            GameLogicManager.Instance.UIManager.ClueImage.sprite = GameLogicManager.Instance.UIManager.ThirdClueSprite1;

        }
        else if (GameLogicManager.Instance.ThirdClueGroup2.Contains(GameLogicManager.Instance.Guilty))
        {
            GameLogicManager.Instance.UIManager.ClueImage.sprite = GameLogicManager.Instance.UIManager.ThirdClueSprite2;
        }

        GameLogicManager.Instance.UIManager.ClueText.text = clueTextString;
    }

    // Corrutina para cerrar el panel que muestra la pista
    private IEnumerator WaitForInputToClosePanel()
    {        
        yield return new WaitForSeconds(waitingTimeToClose);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.E));
        PlayerEvents.FinishShowingInformation();
        GameLogicManager.Instance.UIManager.CluePanel.SetActive(false);
    }
}
