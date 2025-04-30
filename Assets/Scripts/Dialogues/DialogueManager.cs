using System.Collections;
using UnityEngine;

// Enum de las fases que puede tener una conversación
public enum ConversationPhase
{
    None, Started, Continued, Ended
}

// Enum de los tipos de conversación
public enum ConversationType
{
    InspectDialogue, MultipleChoiceDialogue, StoryPhaseDialogueConversation, StoryPhaseDialogueTrigger, PlayerLogicDialogue, TheEndDialogue
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI Objects Section")]
    [SerializeField] private GameObject dialogueMark;

    [Header("Character Data Section")]
    [SerializeField] private Sprite characterProfileImage;  

    [Header("Variable Section")]
    [SerializeField] private float typingTime = 0.05f;
    [SerializeField] private int charsToPlaySound;
    
    private ConversationPhase conversationPhase = ConversationPhase.None;
    private ConversationType currentConversationType;
    private MultipleChoiceDialogue multipleChoiceDialogue;
    private StoryPhaseDialogue storyPhaseDialogue;
    private TheEndDialogue theEndDialogue;
    private InspectDialogue inspectDialogue;
    private IDialogueLogic dialogueLogic;
    private Coroutine markCoroutine;
    private string[] dialogueLines;
    private string[] characterNameLines;
    private int lineIndex;
    private bool isPlayerInRange;
    private bool isTypingDialogueText;

    // REVISAR AUDIO
    private AudioSource typingDialogueAudioSource;


    void Start()
    {        
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        typingDialogueAudioSource = audioSources[0];

        multipleChoiceDialogue = GetComponent<MultipleChoiceDialogue>();
        storyPhaseDialogue = GetComponent<StoryPhaseDialogue>();
        theEndDialogue = GetComponent<TheEndDialogue>();
        inspectDialogue = GetComponent<InspectDialogue>();
        dialogueLogic = GetComponent<IDialogueLogic>();
    }
    
    // Método para comenzar la conversación. Se llama desde otros scripts
    public void StartConversation(ConversationType conversationType, string[] newDialogueLines, string[] newCharacterNames)
    {
        StartCoroutine(StartConversationDelay(conversationType, newDialogueLines, newCharacterNames));
    }

    // Corrutina que espera un frame para asegurarse que se ha actualizado correctamente el estado del jugador
    private IEnumerator StartConversationDelay(ConversationType conversationType, string[] newDialogueLines, string[] newCharacterNames)
    {
        yield return null;

        if(GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerState is TalkingState)
        {
            conversationPhase = ConversationPhase.Started;
            currentConversationType = conversationType;
            dialogueLines = newDialogueLines;
            characterNameLines = newCharacterNames;
            StartDialogue();
        }
        else
        {
            Debug.LogError("No se ha iniciado correctamente la conversación.");
        }
    }

    // Método para empezar a mostrar el diálogo correspondiente
    private void StartDialogue()
    {
        // Si es un diálogo del tipo de multiples opciones se desactiva el panel de elecciones
        if(currentConversationType == ConversationType.MultipleChoiceDialogue) 
        GameLogicManager.Instance.UIManager.DialogueChoiceSection.SetActive(false);

        // Si es un diálogo del tipo de fase de la historia se desactiva el aviso de diálogo
        if(currentConversationType == ConversationType.StoryPhaseDialogueConversation
            || currentConversationType == ConversationType.TheEndDialogue) 
        dialogueMark.SetActive(false);
        
        // Se activa la sección de conversación en el panel de diálogo 
        GameLogicManager.Instance.UIManager.DialogueConversationSection.SetActive(true);

        // Si activa el panel de diálogo si no es un diálogo del tipo de multiples opciones  
        if(currentConversationType != ConversationType.MultipleChoiceDialogue) 
        GameLogicManager.Instance.UIManager.DialoguePanel.SetActive(true);

        lineIndex = 0;
        ShowProfileImage();
        ShowCharacterName();
        StartCoroutine(ShowLine());
    }

    // Método para mostrar la siguiente línea de diálogo
    private void NextDialogueLine()
    {
        if(lineIndex < dialogueLines.Length)
        { 
            ShowProfileImage();
            ShowCharacterName();
            StartCoroutine(ShowLine());
        }
        else
        {
            EndDialogue();
        }
    }

    // Método para terminar con el diálogo correspondiente
    private void EndDialogue()
    {
        GameLogicManager.Instance.UIManager.DialoguePanel.SetActive(false);
        GameLogicManager.Instance.UIManager.DialogueConversationSection.SetActive(false);

        // Si es un diálogo del tipo de fase de la historia o del tipo de multiples opciones se activa el aviso de diálogo
        if(currentConversationType == ConversationType.StoryPhaseDialogueConversation 
            || currentConversationType == ConversationType.TheEndDialogue
            || currentConversationType == ConversationType.MultipleChoiceDialogue) 
        dialogueMark.SetActive(true);

        // Si es un diálogo del tipo lógica del jugador o del tipo inspección se da por completada la acción de inspeccionar
        if(currentConversationType == ConversationType.PlayerLogicDialogue
            || currentConversationType == ConversationType.InspectDialogue)
        GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().IsInspectionComplete = false;

        // Si es un diálogo del tipo inspección se comprueba si se hace falta avanzar la historia o mostrar la pista
        if(currentConversationType == ConversationType.InspectDialogue)
        {
            // Se hace avanzar la historia cuando sea necesario
            if(inspectDialogue.IsAdvanceStory)
            {
                if(GetComponent<AdvanceStoryManager>() != null)
                {
                    GetComponent<AdvanceStoryManager>().AdvanceStoryState();
                }
                else
                {
                    Debug.LogError("No existe el componente necesario para avanzar la historia.");
                }
                
                inspectDialogue.IsAdvanceStory = false;
            }

            // Se muestra la pista cuando sea necesario
            if(inspectDialogue.IsNecesaryShowClue)
            {
                if(GetComponent<CluesDisplayManager>() != null)
                {
                    PlayerEvents.FinishTalkingWithClue();
                    GetComponent<CluesDisplayManager>().ShowDiscoveredClue();
                }
                else
                {
                    Debug.LogError("No existe el componente necesario para mostrar la pista.");
                }

                inspectDialogue.IsNecesaryShowClue = false;
            }
            else
            {
                PlayerEvents.FinishTalkingWithoutClue();
            }
        }
        else
        {
            PlayerEvents.FinishTalkingWithoutClue();
        }

        conversationPhase = ConversationPhase.Ended;
    }

    // Método para que otros scripts puedan acabar la conversación cuando sea necesario
    public void EndConversation(ConversationType endConversationType)
    {
        typingDialogueAudioSource.Stop();        

        currentConversationType = endConversationType;
        
        StopAllCoroutines();

        EndDialogue();
    }

    // Método para mostrar la imagen de perfil del personaje que está hablando
    private void ShowProfileImage()
    {
        if(characterNameLines[lineIndex] == "Player")
        {
            GameLogicManager.Instance.UIManager.DialogueConversationProfile.sprite = 
                GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerImage;
        }
        else
        {
            GameLogicManager.Instance.UIManager.DialogueConversationProfile.sprite = characterProfileImage;
        }        
    }

    // Método para mostrar el nombre del personaje que está hablando
    private void ShowCharacterName()
    {
        if(characterNameLines[lineIndex] == "Player")
        {
            GameLogicManager.Instance.UIManager.DialogueConversationName.text = 
                GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerName;
        }
        else
        {
            GameLogicManager.Instance.UIManager.DialogueConversationName.text = characterNameLines[lineIndex];
        }        
    }
    
    // Corrutina para mostrar la línea de diálogo de forma progresiva
    private IEnumerator ShowLine() 
    {
        GameLogicManager.Instance.UIManager.DialogueConversationText.text = string.Empty;
        int charIndex = 0;

        // Se llama a la corrutina para detectar si se cumple la condición para mostrar la línea de diálogo directamente
        isTypingDialogueText = true;
        StartCoroutine(WaitForSkip());

        foreach(char ch in dialogueLines[lineIndex])
        {
            if (!isTypingDialogueText) break;

            GameLogicManager.Instance.UIManager.DialogueConversationText.text += ch;
            
            if(charIndex % charsToPlaySound == 0) typingDialogueAudioSource.Play();
            charIndex++;

            yield return new WaitForSeconds(typingTime);
        }

        // En caso de que se salte el tipeo, se muestra directamente la línea de diálogo
        if (GameLogicManager.Instance.UIManager.DialogueConversationText.text.Length < dialogueLines[lineIndex].Length)
        GameLogicManager.Instance.UIManager.DialogueConversationText.text = dialogueLines[lineIndex];

        isTypingDialogueText = false;

        // Se llama a la corrutina para continuar con el diálogo
        StartCoroutine(ContinueDialogue());
    }

    // Corrutina para mostrar la línea de diálogo directamente seá necesario
    private IEnumerator WaitForSkip() 
    {
        while (isTypingDialogueText)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                isTypingDialogueText = false;
                yield break;
            }
            yield return null;
        }
    }   

    // Corrutina para que se pueda continuar con la siguiente línea de diálogo
    private IEnumerator ContinueDialogue()
    {       
        yield return new WaitUntil(() => !isTypingDialogueText);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
        yield return null;

        lineIndex++;
        conversationPhase = ConversationPhase.Continued;
        NextDialogueLine();
    }

    // Método para obtener si el jugador está dentro del rango de un objeto o personaje
    public bool IsPlayerInRange
    {
        get { return isPlayerInRange; }
    }

    // Métodos para obtener y cambiar la fase en la que se encuentra la conversación
    public ConversationPhase CurrentConversationPhase
    {
        get { return conversationPhase; }
        set { conversationPhase = value; }
    }

    // Método para obtener el aviso de diálogo
    public GameObject DialogueMark
    {
        get { return dialogueMark; }
    }

    // Corrutina para activar o desactivar el aviso de diálogo en función de si se está inspeccionando o no 
    private IEnumerator CheckPlayerStateForMark()
    {
        while (true)
        {
            var playerState = GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerState.StateName;

            if (playerState == PlayerStatePhase.Inspection)
            {
                if (dialogueMark.activeSelf)
                    dialogueMark.SetActive(false);
            }
            else
            {
                if (!dialogueMark.activeSelf)
                    dialogueMark.SetActive(true);
            }

            yield return null; // Espera al siguiente frame
        }
    }

    // Método que se llama cuando un objeto entra en el área de colisión del trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // Se activa el aviso de diálogo cuando sea necesario
            if(multipleChoiceDialogue != null || theEndDialogue != null
                || (storyPhaseDialogue != null && storyPhaseDialogue.DialogueType == StoryPhaseDialogueType.Conversation)) 
            dialogueMark.SetActive(true);

            // Se avisa al jugador que hay un objeto a inspeccionar en rango
            if(inspectDialogue != null) 
            GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().IsInspectObjectInRange = true;

            // Se llama al método de los scripts auxiliares para que permanezcan atentos a si se inicia la conversación
            if (dialogueLogic != null) dialogueLogic.WaitForDialogueInput();

            markCoroutine = StartCoroutine(CheckPlayerStateForMark());

            isPlayerInRange = true;
        }
    }

    // Método que se llama cuando un objeto sale del área de colisión del trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // Se desactiva el aviso de diálogo cuando sea necesario
            if(multipleChoiceDialogue != null || theEndDialogue != null
                || (storyPhaseDialogue != null && storyPhaseDialogue.DialogueType == StoryPhaseDialogueType.Conversation))
            dialogueMark.SetActive(false);

            // Se avisa al jugador que ya no hay un objeto a inspeccionar en rango
            if(inspectDialogue != null)
            GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().IsInspectObjectInRange = false;

            // Se llama al método de los scripts auxiliares para que se deje de esperar a iniciar la conversación
            if (dialogueLogic != null) dialogueLogic.ExitOfDialogueRange();

            StopAllCoroutines();

            conversationPhase = ConversationPhase.None;

            if (markCoroutine != null)
            {
                StopCoroutine(markCoroutine);
                markCoroutine = null;
            }

            isPlayerInRange = false;
        }
    }
}
