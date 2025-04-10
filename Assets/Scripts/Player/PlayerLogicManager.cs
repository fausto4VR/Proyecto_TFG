using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerLogicManager : MonoBehaviour
{
    [Header("UI Objects Section")]
    [SerializeField] private GameObject inspectProgressBar;

    [Header("Sound Section")]
    [SerializeField] private GameObject audioSourcesManager;
    [SerializeField] private AudioSource worldMusic;

    [Header("Player Data Section")]
    [SerializeField] private string playerName = "Player";
    [SerializeField] private Sprite playerImage;

    [Header("Variable Section")]
    [SerializeField] private float holdInspectKeyTime = 2.0f;
    [SerializeField] private PlayerStatePhase firstPlayerPhase = PlayerStatePhase.Idle;

    // QUITAR ---------------------------------------------------------
    [Header("QUITAR")]    
    public GameObject inspectLayout;
    public GameObject dialoguePanel;
    // ----------------------------------------------------------------

    private PlayerState playerState;
    private PlayerState defaultPlayerState;
    private float holdInspectKeyDuration;
    private bool isInspectionComplete;
    private bool isInspectObjectInRange;

    // REVISAR AUDIO
    private AudioSource inspectAudioSource;


    void Start()
    {
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        inspectAudioSource = audioSources[3];

        defaultPlayerState = GetStateFromPhase(firstPlayerPhase);

        // Se asigna el estado que tenía antes cuando se vuelve de un puzle
        if(!(GameStateManager.Instance.isNewGame || GameStateManager.Instance.isLoadGame))
        {
            playerState = GameLogicManager.Instance.TemporalPlayerState;
            if (playerState == null) InitializeFirstState();
            playerState.OnEnter();
        }  
    }

    // Métdodo para convertir una opción del enum PlayerStatePhase en un tipo de estado del jugador
    private PlayerState GetStateFromPhase(PlayerStatePhase phase)
    {
        switch (phase)
        {
            case PlayerStatePhase.Idle:
                return new IdleState();
            case PlayerStatePhase.Inspection:
                return new InspectionState();
            case PlayerStatePhase.Talking:
                return new TalkingState();
            case PlayerStatePhase.ShowingInformation:
                return new ShowingInformationState();
            default:
                return new IdleState();
        }
    } 

    void Update()
    {
        playerState = playerState.HandleInput();

        if(Input.GetKey(KeyCode.Q))
        {
            InspectCheck();
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            ResetProgress();
            PlayerEvents.AbortInspection();
        }

        // Se obtiene información relevante para el desarrollador pulsando V
        if(Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Guilty: " + GameLogicManager.Instance.Guilty);
            Debug.Log("First Clue: " + GameLogicManager.Instance.Clues[0]);
            Debug.Log("Second Clue: " + GameLogicManager.Instance.Clues[1]);
            Debug.Log("Third Clue: " + GameLogicManager.Instance.Clues[2]);
            Debug.Log("Story Phase Aux: " + GameLogicManager.Instance.StoryPhaseAux); // QUITAR
            Debug.Log("Story Phase: " + GameLogicManager.Instance.CurrentStoryPhase.phaseName);
            Debug.Log("Story Subphase: " + GameLogicManager.Instance.CurrentStoryPhase.currentSubphase.subphaseName);
            string LastPuzzle = GameLogicManager.Instance.LastPuzzleComplete;
            Debug.Log("Last Puzzle Complete: " + LastPuzzle == null || LastPuzzle == "" ? "None" : LastPuzzle);
            string suspectsContent = string.Join(", ", GameLogicManager.Instance.KnownSuspects);
            Debug.Log("Known Suspects: " + suspectsContent);
            string tutorialsContent = string.Join(", ", GameLogicManager.Instance.KnownTutorials);
            Debug.Log("Known Tutorials: " + tutorialsContent);
            string dialoguesContent = string.Join(", ", GameLogicManager.Instance.KnownDialogues);
            Debug.Log("Known Dialogues: " + dialoguesContent);            
            Debug.Log("Is Bad Ending: " + GameLogicManager.Instance.IsBadEnding);       
            Debug.Log("End Opportunities: " + GameLogicManager.Instance.EndOpportunities);
            Debug.Log("Player State: " + playerState.StateName);
            Debug.Log("Player Position: (" + GameLogicManager.Instance.Player.transform.position.x + ", " 
                + GameLogicManager.Instance.Player.transform.position.y + ", " 
                + GameLogicManager.Instance.Player.transform.position.z + ")");
            Debug.Log("Player Position: (" + GameLogicManager.Instance.VirtualCamera.transform.position.x + ", " 
                + GameLogicManager.Instance.VirtualCamera.transform.position.y + ", " 
                + GameLogicManager.Instance.VirtualCamera.transform.position.z + ")");
        }
    }

    // Método para comenzar el proceso de inspección de un objeto
    private void InspectCheck()
    {
        if(playerState is IdleState)
        {
            PlayerEvents.StartInspection();
            
            inspectLayout.SetActive(true);

            worldMusic.Stop();
            inspectAudioSource.Play();
        }

        if(playerState is InspectionState)
        {
            holdInspectKeyDuration += Time.deltaTime;
            float progress = holdInspectKeyDuration / holdInspectKeyTime;
            inspectProgressBar.GetComponent<Image>().fillAmount = progress;

            if(holdInspectKeyDuration >= holdInspectKeyTime)
            {
                inspectProgressBar.GetComponent<Image>().fillAmount = 1;

                PlayerEvents.FinishInspection();

                if(isInspectObjectInRange)
                {                                  
                    CompleteInspection();
                }
                else
                { 
                    ShowDefaultMessage();
                }
            }
        }
    }

    // Método que gestiona la lógica cuando se completa la inspección de un objeto
    private void CompleteInspection()
    {   
        isInspectionComplete = true;
        ResetProgress();
    }

    // Método para mostrar un mensaje por defecto cuando se inspecciona un objeto que no tiene nada relevante
    public void ShowDefaultMessage()
    {
        GetComponent<DialogueManager>().dialogueLines = GameStateManager.Instance.gameConversations.defaultInspectConversation
            .Select(dialogue => dialogue.line).ToArray();
        GetComponent<DialogueManager>().characterNameLines = GameStateManager.Instance.gameConversations.defaultInspectConversation
            .Select(dialogue => dialogue.speaker).ToArray();

        ResetProgress();
        GetComponent<DialogueManager>().StartConversation();
    }

    // Método para reiniciar el progreso mientras transcurre la inspección del objeto
    private void ResetProgress()
    {
        if (inspectLayout.activeInHierarchy)
        {
            inspectAudioSource.Stop();
            worldMusic.Play();

            inspectLayout.SetActive(false);
            holdInspectKeyDuration = 0f;
            inspectProgressBar.GetComponent<Image>().fillAmount = 0f;
        }
    }

    // Métodos para obtener y cambiar si se ha completado la inspección por parte del jugador
    public bool IsInspectionComplete
    {
        get { return isInspectionComplete; }
        set { isInspectionComplete = value; }
    }

    // Método para cambiar si existe algún objeto en inpseccionable a rango
    public bool IsInspectObjectInRange
    {
        set { isInspectObjectInRange = value; }
    }

    // Método para obtener el nombre que identifica al personaje del jugador
    public string PlayerName
    {
        get { return playerName; }
    }

    // Método para obtener la imagen del protagonista
    public Sprite PlayerImage
    {
        get { return playerImage; }
    }
    
    // Métodos para obtener y cambiar el estado actual del jugador
    public PlayerState PlayerState
    {
        get { return playerState != null ? playerState : DefaultStateInitialized; }
        set { playerState = value; }
    }

    // Método para obtener el primer estado del jugador por defecto
    public PlayerState DefaultStateInitialized
    {
        get 
        {
            defaultPlayerState.OnEnter();
            return defaultPlayerState; 
        }
    }

    // Método para inicializar el estado del jugador
    public void InitializeFirstState()
    {
        playerState = defaultPlayerState;
        playerState.OnEnter();
    }
}
