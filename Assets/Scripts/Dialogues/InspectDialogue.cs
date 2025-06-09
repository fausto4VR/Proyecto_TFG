using UnityEngine;
using System.Collections;
using System.Linq;

public class InspectDialogue : MonoBehaviour, IDialogueLogic
{   
    [Header("Variable Section")]
    [SerializeField] private string puzzleAssociated;
    [SerializeField] private bool isAdvanceStoryTrigger;
    [SerializeField] private bool isPuzzleTriggerObject;
    [SerializeField] private bool isClueUnlockTrigger;
    
    [SubphaseSelector]
    [SerializeField] private string selectedSubphase;

    private Coroutine waitCoroutine;
    private Coroutine skipCoroutine;
    private bool isAdvanceStory;    
    private bool isNecesaryShowClue;
    private bool isDefaultMessage;

    // REVISAR AUDIO
    private AudioSource inspectionSuccess;


    void Start()
    {
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        inspectionSuccess = audioSources[9];
    }

    // Método que se llama cuando se entra en el rango de diálogo de un objeto
    public void WaitForDialogueInput()
    {
        waitCoroutine = StartCoroutine(WaitUntilInspectionComplete());
    }

    // Método para iniciar la conversación al hacer clic con el cursor sobre el personaje
    public void TryStartDialogueOnClick()
    {
        // Se deja vacío porque este tipo de diálogo no se activa por clic
    }
    
    // Corrutina para esperar que se complete la inspección del objeto 
    private IEnumerator WaitUntilInspectionComplete()
    {
        PlayerLogicManager playerLogic = GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>();
        yield return new WaitUntil(() => playerLogic.IsInspectionComplete);

        SelectDialogue();
        skipCoroutine = StartCoroutine(WaitToSkipDialogue());
        WaitForDialogueInput();
    }

    // Método para seleccionar el diálogo correspondiente en función de cuando se inspeccionó el objeto
    private void SelectDialogue()
    {
        if (GameLogicManager.Instance.CurrentStoryPhase.ComparePhase(selectedSubphase) == SubphaseTemporaryOrder.IsCurrent)
        {
            string[] dialogueLines = GameStateManager.Instance.gameConversations.inspectConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.currentDialogue
                .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

            string[] characterNameLines = GameStateManager.Instance.gameConversations.inspectConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.currentDialogue
                .Select(dialogue => dialogue.speaker).ToArray() ?? new string[0];

            if (isAdvanceStoryTrigger) isAdvanceStory = true;
            if (isClueUnlockTrigger) isNecesaryShowClue = true;
            inspectionSuccess.Play();
            GetComponent<DialogueManager>().StartConversation(ConversationType.InspectDialogue, dialogueLines, characterNameLines);
            isDefaultMessage = false;
        }
        else if (GameLogicManager.Instance.CurrentStoryPhase.ComparePhase(selectedSubphase) == SubphaseTemporaryOrder.IsRecentBefore)
        {
            string[] dialogueLines = GameStateManager.Instance.gameConversations.inspectConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.afterRecentDialogue
                .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

            string[] characterNameLines = GameStateManager.Instance.gameConversations.inspectConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.afterRecentDialogue
                .Select(dialogue => dialogue.speaker).ToArray() ?? new string[0];

            GetComponent<DialogueManager>().StartConversation(ConversationType.InspectDialogue, dialogueLines, characterNameLines);
            isDefaultMessage = false;
        }
        else if (GameLogicManager.Instance.CurrentStoryPhase.ComparePhase(selectedSubphase) == SubphaseTemporaryOrder.IsDistantBefore)
        {
            string[] dialogueLines = GameStateManager.Instance.gameConversations.inspectConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.afterDistantDialogue
                .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

            string[] characterNameLines = GameStateManager.Instance.gameConversations.inspectConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.afterDistantDialogue
                .Select(dialogue => dialogue.speaker).ToArray() ?? new string[0];

            GetComponent<DialogueManager>().StartConversation(ConversationType.InspectDialogue, dialogueLines, characterNameLines);
            isDefaultMessage = false;
        }
        else
        {
            GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().ShowDefaultMessage();
            isDefaultMessage = true;
        }

        GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().IsInspectionComplete = false;
    }

    // Corrutina para esperar a que el jugador quiera saltarse el diálogo una vez empezado
    private IEnumerator WaitToSkipDialogue()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        yield return null;

        if (GetComponent<DialogueManager>().CurrentConversationPhase != ConversationPhase.Ended)
        {
            inspectionSuccess.Stop();

            if (isDefaultMessage) GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().StopDefaultMessage();
            else GetComponent<DialogueManager>().EndConversation(ConversationType.InspectDialogue);
        }

    }

    // Método que se llama cuando se sale del rango de diálogo de un objeto
    public void ExitOfDialogueRange()
    {
        StopAllCoroutines();
        
        if (waitCoroutine != null) waitCoroutine = null;
        if (skipCoroutine != null) skipCoroutine = null;
    }

    // Método para mostrar el diálogo al jugador después de completar un puzle
    public void ShowAfterPuzzleDialogue()
    {
        if (isClueUnlockTrigger) isNecesaryShowClue = true;

        string[] dialogueLines = GameStateManager.Instance.gameConversations.inspectConversations
            .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.afterRecentDialogue
            .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

        string[] characterNameLines = GameStateManager.Instance.gameConversations.inspectConversations
            .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.afterRecentDialogue
            .Select(dialogue => dialogue.speaker).ToArray() ?? new string[0];

        skipCoroutine = StartCoroutine(WaitToSkipDialogue());
        GetComponent<DialogueManager>().StartConversation(ConversationType.InspectDialogue, dialogueLines, characterNameLines);
        isDefaultMessage = false;
    }

    // Métodos para obtener y para cambiar si es necesario avanzar la historia
    public bool IsAdvanceStory
    {
        get { return isAdvanceStory; }
        set { isAdvanceStory = value; }
    }

    // Métodos para obtener y para cambiar si es un objeto que active un puzle
    public bool IsPuzzleTriggerObject
    {
        get { return isPuzzleTriggerObject; }
        set { isPuzzleTriggerObject = value; }
    }

    // Métodos para obtener y para cambiar si es necesario mostrar la pista correspondiente
    public bool IsNecesaryShowClue
    {
        get { return isNecesaryShowClue; }
        set { isNecesaryShowClue = value; }
    }

    // Método para obtener el puzzle asociado al objeto
    public string PuzzleAssociated
    {
        get { return puzzleAssociated; }
    }

    // Método para obtener la fase seleccionada
    public string SelectedSubphase
    {
        get { return selectedSubphase; }
    }

    // Método para obtener el nombre de la escena al puzzle asociado al objeto
    public string PuzzleScene
    {
        get { return puzzleAssociated + "Scene"; }
    }

    // Método para actualizar el editor cada vez que se modifica un valor en el Inspector
    private void OnValidate()
    {
        if (this == null || gameObject == null) return;

        if (isPuzzleTriggerObject || isClueUnlockTrigger)
        {
            isAdvanceStoryTrigger = true;
        }

        if (isAdvanceStoryTrigger && GetComponent<AdvanceStoryManager>() == null)
        {
            Debug.LogError($"[{gameObject.name}] Falta el componente AdvanceStoryManager necesario para avanzar la historia.", this);
        }

        if (isClueUnlockTrigger && GetComponent<CluesDisplayManager>() == null)
        {
            Debug.LogError($"[{gameObject.name}] Falta el componente CluesDisplayManager necesario para mostrar la pista asociada.", this);
        }

        if (isPuzzleTriggerObject && string.IsNullOrEmpty(puzzleAssociated))
        {
            Debug.LogError($"[{gameObject.name}] Falta el nombre del puzle que es necesario para mostrarlo.", this);
        }
    }
}
