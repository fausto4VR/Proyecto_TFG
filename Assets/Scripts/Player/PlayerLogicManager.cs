using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class PlayerLogicManager : MonoBehaviour
{
    [Header("UI Objects Section")]
    [SerializeField] private GameObject inspectLayout;
    [SerializeField] private GameObject inspectProgressBar;

    [Header("Player Data Section")]
    [SerializeField] private string playerName = "Player";
    [SerializeField] private Color playerNameColor = new Color(0.6588f, 0.0f, 0.0509f); // Esto es #A8000D
    [SerializeField] private Sprite playerImage;

    [Header("Variable Section")]
    [SerializeField] private float holdInspectKeyTime = 2.0f;
    [SerializeField] private PlayerStatePhase firstPlayerPhase = PlayerStatePhase.Idle;

    private PlayerState playerState;
    private PlayerState defaultPlayerState;
    private Coroutine skipCoroutine;
    private Coroutine finishCoroutine;
    private float holdInspectKeyDuration;
    private bool isInspectionComplete;
    private bool isInspectObjectInRange;

    // REVISAR AUDIO
    private AudioSource inspectAudioSource;
    private AudioSource worldMusic;


    void Start()
    {
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        inspectAudioSource = audioSources[3];
        worldMusic = GameLogicManager.Instance.UIManager.WorldMusic;

        defaultPlayerState = GetStateFromPhase(firstPlayerPhase);

        // Se asigna el estado que tenía antes cuando se vuelve de un puzle o se cambia de escenario con el mapa
        if (!(GameStateManager.Instance.IsNewGame || GameStateManager.Instance.IsLoadGame))
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

        if (Input.GetKey(KeyCode.Q))
        {
            InspectCheck();
            GameLogicManager.Instance.UIManager.OutDetectionPanel.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Q) && playerState is InspectionState)
        {
            ResetProgress();
            PlayerEvents.AbortInspection();
            GameLogicManager.Instance.UIManager.OutDetectionPanel.SetActive(false);
        }
    }

    // Método para comenzar el proceso de inspección de un objeto
    private void InspectCheck()
    {
        if(playerState is IdleState)
        {
            PlayerEvents.StartInspection();
            
            inspectLayout.SetActive(true);

            worldMusic.Pause();
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
                    skipCoroutine = StartCoroutine(WaitToSkipDialogue()); 
                    finishCoroutine = StartCoroutine(WaitToFinishDialogue()); 
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
        string[] dialogueLines = GameStateManager.Instance.gameConversations.defaultInspectConversation
            .Select(dialogue => dialogue.line).ToArray();
        string[] characterNameLines = GameStateManager.Instance.gameConversations.defaultInspectConversation
            .Select(dialogue => dialogue.speaker).ToArray();

        ResetProgress();
        GetComponent<DialogueManager>().StartConversation(ConversationType.PlayerLogicDialogue, dialogueLines, characterNameLines);   
    }

    // Corrutina para esperar a que el jugador quiera saltarse el diálogo una vez empezado
    private IEnumerator WaitToSkipDialogue()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        yield return null;

        if (GetComponent<DialogueManager>().CurrentConversationPhase != ConversationPhase.Ended)
        StopDefaultMessage();
    }

    // Método para detener el mensaje por defecto cuando se inspecciona un objeto que no tiene nada relevante
    public void StopDefaultMessage()
    {
        GetComponent<DialogueManager>().EndConversation(ConversationType.PlayerLogicDialogue);        
        FinishInspection();
    }

    // Corrutina para esperar a que termine el mensaje por defecto cuando se inspecciona un objeto que no tiene nada relevante
    private IEnumerator WaitToFinishDialogue()
    {
        yield return new WaitUntil(() => GetComponent<DialogueManager>().CurrentConversationPhase == ConversationPhase.Ended);
        yield return null;

        FinishInspection();
    }

    // Método para gestionar el final de la conversación del mensaje por defecto cuando se inspecciona un objeto que no tiene nada relevante
    private void FinishInspection()
    {
        StopAllCoroutines();
        if (skipCoroutine != null) skipCoroutine = null;
        if (finishCoroutine != null) finishCoroutine = null;

        GetComponent<DialogueManager>().CurrentConversationPhase = ConversationPhase.None;
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

    // Método para obtener el color del nombre del personaje del jugador
    public Color PlayerNameColor
    {
        get { return playerNameColor; }
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
