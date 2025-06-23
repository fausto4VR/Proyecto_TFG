using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{

    [Header("UI Objects Section")]
    [SerializeField] private GameObject mapMark;

    [Header("Variable Section")]
    [SerializeField] private ClueType clueToUnlockThirdOption;
    
    private Coroutine waitCoroutine;
    private Coroutine closeCoroutine;
    private Coroutine selectCoroutine;
    private Coroutine selectSoundCoroutine;
    private Coroutine markCoroutine;
    private bool isThirdOptionUnlock;
    
    // REVISAR AUDIO
    private AudioSource buttonsAudioSource;
    private AudioSource mapAudioSource;


    void Start()
    { 
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;       
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        buttonsAudioSource = audioSources[1];
        mapAudioSource = audioSources[4];
    }

    // Corrutina para esperar a que el jugador quiera abrir el panel del mapa
    private IEnumerator WaitUntilPlayerOpenMap()
    {
        if (closeCoroutine != null) StopCoroutine(closeCoroutine);
        if (selectCoroutine != null) StopCoroutine(selectCoroutine);
        if (selectSoundCoroutine != null) StopCoroutine(selectSoundCoroutine);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.M));
        yield return null;

        DisplayTheMapPanel(true);
    }

    // Corrutina para esperar a que el jugador quiera cerrar el panel del mapa
    private IEnumerator WaitUntilPlayerClosePanel()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.E));
        yield return null;
        
        DisplayTheMapPanel(false);
    }

    // Método para mostrar el panel del mapa donde se puede seleccionar el lugar al que quiere ir el jugador
    public void DisplayTheMapPanel(bool showPanel)
    {          
        mapAudioSource.Play();
            
        if(showPanel)
        {
            PlayerEvents.StartShowingInformation();

            ShowThirdOption();
            GameLogicManager.Instance.UIManager.MapPanel.SetActive(true);            
            GameLogicManager.Instance.UIManager.OutDetectionPanel.SetActive(true);

            closeCoroutine = StartCoroutine(WaitUntilPlayerClosePanel());
            selectCoroutine = StartCoroutine(WaitUntilPlayerSelectOption());
        }
        else
        {            
            ClosePanel();
            waitCoroutine = StartCoroutine(WaitUntilPlayerOpenMap());
        }
    }

    // Método para establecer si se puede mostrar la tercera opción del mapa
    private void ShowThirdOption()
    {
        int indexCurrentClue = (int) clueToUnlockThirdOption;

        if(GameLogicManager.Instance.KnownClues[indexCurrentClue])
        {
            isThirdOptionUnlock = true;
            GameLogicManager.Instance.UIManager.ThirdOptionKeyInMap.SetActive(true);
            GameLogicManager.Instance.UIManager.ThirdOptionPanelInMap.SetActive(true);
        }
    }

    // Corrutina para esperar a que el jugador elija una opción de destino en el mapa
    private IEnumerator WaitUntilPlayerSelectOption()
    {
        bool isOptionChosen = false;

        while (!isOptionChosen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                isOptionChosen = true;
                GoToScene(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                isOptionChosen = true;
                GoToScene(1);
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) && isThirdOptionUnlock)
            {
                isOptionChosen = true;
                GoToScene(2);
            }

            yield return null;
        }        
    }

    // Método para elegir la escena a la que se quiere ir
    public void GoToScene(int sceneIndex)
    {
        if (selectCoroutine != null) StopCoroutine(selectCoroutine);
        if (closeCoroutine != null) StopCoroutine(closeCoroutine);

        buttonsAudioSource.Play();

        selectSoundCoroutine = StartCoroutine(WaitForSoundAndSendScene(sceneIndex));
    }

    // Corrutina para esperar que suene el sonido del botón (que tarda menos que la transición) y se vaya a la escena seleccionada
    private IEnumerator WaitForSoundAndSendScene(int sceneIndex)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (sceneIndex == 0 && currentSceneName != "HomeScene")
        {
            GameStateManager.Instance.TransitionManager.StartOutTransition(TransitionType.Fade);

            // yield return new WaitForSeconds(buttonsAudioSource.clip.length);
            yield return new WaitForSeconds(GameStateManager.Instance.TransitionDuration);

            GameLogicManager.Instance.TemporalPlayerState =
                GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().DefaultStateInitialized;

            ClosePanel();

            GameStateManager.Instance.IsMapTravel = true;
            SceneManager.LoadScene("HomeScene");
        }
        else if (sceneIndex == 1 && currentSceneName != "MansionScene")
        {
            GameStateManager.Instance.TransitionManager.StartOutTransition(TransitionType.Fade);

            // yield return new WaitForSeconds(buttonsAudioSource.clip.length);
            yield return new WaitForSeconds(GameStateManager.Instance.TransitionDuration);

            GameLogicManager.Instance.TemporalPlayerState =
                GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().DefaultStateInitialized;

            ClosePanel();

            GameStateManager.Instance.IsMapTravel = true;
            SceneManager.LoadScene("MansionScene");
        }
        else if (sceneIndex == 2 && currentSceneName != "ParkScene" && isThirdOptionUnlock)
        {
            GameStateManager.Instance.TransitionManager.StartOutTransition(TransitionType.Fade);

            // yield return new WaitForSeconds(buttonsAudioSource.clip.length);
            yield return new WaitForSeconds(GameStateManager.Instance.TransitionDuration);

            GameLogicManager.Instance.TemporalPlayerState =
                GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().DefaultStateInitialized;

            ClosePanel();

            GameStateManager.Instance.IsMapTravel = true;
            SceneManager.LoadScene("ParkScene");
        }
        else
        {
            yield return new WaitForSeconds(buttonsAudioSource.clip.length);
            
            GameLogicManager.Instance.TemporalPlayerState =
                GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().DefaultStateInitialized;

            ClosePanel();

            yield return null;

            waitCoroutine = StartCoroutine(WaitUntilPlayerOpenMap());
        }   
    }

    // Método para cerrar el panel del mapa
    private void ClosePanel()
    {        
        GameLogicManager.Instance.UIManager.MapPanel.SetActive(false);        
            GameLogicManager.Instance.UIManager.OutDetectionPanel.SetActive(false);

        if (isThirdOptionUnlock)
        {
            GameLogicManager.Instance.UIManager.ThirdOptionKeyInMap.SetActive(false);
            GameLogicManager.Instance.UIManager.ThirdOptionPanelInMap.SetActive(false);
        }
        
        isThirdOptionUnlock = false;

        PlayerEvents.FinishShowingInformation();
    }

    // Corrutina para activar o desactivar la marca del mapa en función de si se está inspeccionando o no 
    private IEnumerator CheckPlayerStateForMark()
    {
        var previousPlayerState = GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerState.StateName;

        while (true)
        {
            var currentPlayerState = GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerState.StateName;

            if (currentPlayerState == PlayerStatePhase.Inspection)
            {
                if (mapMark.activeSelf)
                {
                    mapMark.SetActive(false);
                    previousPlayerState = PlayerStatePhase.Inspection;
                }
            }
            else if (currentPlayerState == PlayerStatePhase.Talking)
            {
                if (mapMark.activeSelf)
                {
                    mapMark.SetActive(false);
                    previousPlayerState = PlayerStatePhase.Talking;
                }
            }
            else if (currentPlayerState == PlayerStatePhase.Idle
                && (previousPlayerState == PlayerStatePhase.Inspection || previousPlayerState == PlayerStatePhase.Talking))
            {
                if (!mapMark.activeSelf)
                {
                    mapMark.SetActive(true);
                    previousPlayerState = PlayerStatePhase.Idle;
                }
            }

            yield return null;
        }
    }

    // Método que se llama cuando un objeto entra en el área de colisión del trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (GameLogicManager.Instance.CurrentStoryPhase.phaseName != StoryPhaseOption.Prologue)
            {
                if(mapMark != null) mapMark.SetActive(true);
                
                waitCoroutine = StartCoroutine(WaitUntilPlayerOpenMap());
                markCoroutine = StartCoroutine(CheckPlayerStateForMark());                
            }
        }
    }

    // Método que se llama cuando un objeto sale del área de colisión del trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (GameLogicManager.Instance.CurrentStoryPhase.phaseName != StoryPhaseOption.Prologue)
            {
                if(mapMark != null) mapMark.SetActive(false);
                isThirdOptionUnlock = false;            
                StopAllCoroutines();

                if (waitCoroutine != null) waitCoroutine = null;
                if (closeCoroutine != null) closeCoroutine = null;
                if (selectCoroutine != null) selectCoroutine = null;
                if (selectSoundCoroutine != null) selectSoundCoroutine = null;
                if (markCoroutine != null) markCoroutine = null;            
            }
        }
    }
}
