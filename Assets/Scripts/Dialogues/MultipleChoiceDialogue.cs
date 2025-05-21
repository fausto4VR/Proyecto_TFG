using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class MultipleChoiceDialogue: MonoBehaviour, IDialogueLogic
{ 
    [Header("Variable Section")]
    [SerializeField] private ClueType clueToUnlockDialogue;
    [SerializeField] private int suspectIndex;
    [SerializeField] private string firstOptionText;
    [SerializeField] private string secondOptionText;
    [SerializeField] private string thirdOptionText;
    
    private Coroutine waitCoroutine;
    private Coroutine skipCoroutine;
    private Coroutine restartCoroutine;
    private bool isThirdOptionUnlock;
    
    // REVISAR AUDIO
    private AudioSource buttonsAudioSource;


    void Start()
    {
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        buttonsAudioSource = audioSources[1];
    }

    // Método que se llama cuando se entra en el rango de diálogo de un objeto
    public void WaitForDialogueInput()
    {
        if (skipCoroutine != null) StopCoroutine(skipCoroutine);        
        if (restartCoroutine != null) StopCoroutine(restartCoroutine);

        waitCoroutine = StartCoroutine(WaitUntilPlayerStartDialogue());
    }

    // Corrutina para esperar a que el jugador quiera comenzar el diálogo
    private IEnumerator WaitUntilPlayerStartDialogue()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        yield return null;

        PlayerEvents.StartTalking();

        buttonsAudioSource.Play();

        GameLogicManager.Instance.UIManager.FirstOptionTextInChoice.text = firstOptionText;
        GameLogicManager.Instance.UIManager.SecondOptionTextInChoice.text = secondOptionText;
        GameLogicManager.Instance.UIManager.ThirdOptionTextInChoice.text = thirdOptionText;

        bool isOptionChosen = false;
        ManageButtonLogic(isOptionChosen);

        GetComponent<DialogueManager>().DialogueMark.SetActive(false);
        ShowThirdOption();

        GameLogicManager.Instance.UIManager.DialoguePanel.SetActive(true);
        GameLogicManager.Instance.UIManager.DialogueChoiceSection.SetActive(true);

        skipCoroutine = StartCoroutine(WaitToSkipDialogue());

        while (!isOptionChosen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                isOptionChosen = true;
                HandleDialogueOptionSelected(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                isOptionChosen = true;
                HandleDialogueOptionSelected(2);
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) && isThirdOptionUnlock)
            {
                isOptionChosen = true;
                HandleDialogueOptionSelected(3);
            }

            yield return null;
        }        
    }

    // Método para gestionar la lógica de los botones y que al pulsarlos hagan lo mismo que al seleccionar un número
    private void ManageButtonLogic(bool isOptionChosen)
    {
        Button firstButton = GameLogicManager.Instance.UIManager.FirstOptionButtonInChoice.GetComponent<Button>();
        Button secondButton = GameLogicManager.Instance.UIManager.SecondOptionButtonInChoice.GetComponent<Button>();
        Button thirdButton = GameLogicManager.Instance.UIManager.ThirdOptionButtonInChoice.GetComponent<Button>();

        firstButton.onClick.RemoveAllListeners();
        secondButton.onClick.RemoveAllListeners();
        thirdButton.onClick.RemoveAllListeners();

        firstButton.onClick.AddListener(() =>
        {
            if (!isOptionChosen)
            {
                isOptionChosen = true;
                HandleDialogueOptionSelected(1);
            }
        });

        secondButton.onClick.AddListener(() =>
        {
            if (!isOptionChosen)
            {
                isOptionChosen = true;
                HandleDialogueOptionSelected(2);
            }
        });

        thirdButton.onClick.AddListener(() =>
        {
            if (isThirdOptionUnlock && !isOptionChosen)
            {
                isOptionChosen = true;
                HandleDialogueOptionSelected(3);
            }
        });
    }

    // Método para establecer si se puede mostrar la tercera opción de diálogo
    private void ShowThirdOption()
    {
        int indexCurrentClue = (int) clueToUnlockDialogue;

        if(clueToUnlockDialogue == ClueType.None || GameLogicManager.Instance.KnownClues[indexCurrentClue])
        {
            isThirdOptionUnlock = true;
            GameLogicManager.Instance.UIManager.ThirdOptionKeyInChoice.SetActive(true);
            GameLogicManager.Instance.UIManager.ThirdOptionButtonInChoice.SetActive(true);
        }
    }

    // Método para manejar cada opción de diálogo 
    private void HandleDialogueOptionSelected(int optionSelected)
    {
        bool[] newKnownSuspects = GameLogicManager.Instance.KnownSuspects;
        newKnownSuspects[suspectIndex] = true;
        GameLogicManager.Instance.KnownSuspects = newKnownSuspects;

        if (optionSelected == 1)
        {
            string[] dialogueLines = GameStateManager.Instance.gameConversations.multipleChoiceConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.firstDialogue
                .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

            string[] characterNameLines = GameStateManager.Instance.gameConversations.multipleChoiceConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.firstDialogue
                .Select(dialogue => dialogue.speaker).ToArray() ?? new string[0];
            
            GetComponent<DialogueManager>().StartConversation(ConversationType.MultipleChoiceDialogue, 
                dialogueLines, characterNameLines);
            
            restartCoroutine = StartCoroutine(WaitUntilPlayerRestartDialogue());
        }
        else if (optionSelected == 2)
        {
            string[] dialogueLines = GameStateManager.Instance.gameConversations.multipleChoiceConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.secondDialogue
                .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

            string[] characterNameLines = GameStateManager.Instance.gameConversations.multipleChoiceConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.secondDialogue
                .Select(dialogue => dialogue.speaker).ToArray() ?? new string[0];
            
            GetComponent<DialogueManager>().StartConversation(ConversationType.MultipleChoiceDialogue, 
                dialogueLines, characterNameLines);
            
            restartCoroutine = StartCoroutine(WaitUntilPlayerRestartDialogue());
        }
        else if (optionSelected == 3)
        {
            string[] dialogueLines = GameStateManager.Instance.gameConversations.multipleChoiceConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.thirdDialogue
                .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

            string[] characterNameLines = GameStateManager.Instance.gameConversations.multipleChoiceConversations
                .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.thirdDialogue
                .Select(dialogue => dialogue.speaker).ToArray() ?? new string[0];
            
            GetComponent<DialogueManager>().StartConversation(ConversationType.MultipleChoiceDialogue, 
                dialogueLines, characterNameLines);
            
            restartCoroutine = StartCoroutine(WaitUntilPlayerRestartDialogue());
        }
    }

    // Corrutina para esperar a que el jugador quiera saltarse el diálogo una vez empezado
    private IEnumerator WaitToSkipDialogue()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        yield return null;

        if (GetComponent<DialogueManager>().CurrentConversationPhase != ConversationPhase.Ended)
        {
            if (waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
                waitCoroutine = null;
            }

            restartCoroutine = StartCoroutine(WaitUntilPlayerRestartDialogue());

            GameLogicManager.Instance.UIManager.DialogueChoiceSection.SetActive(false);
            GetComponent<DialogueManager>().DialogueMark.SetActive(true);

            GetComponent<DialogueManager>().EndConversation(ConversationType.MultipleChoiceDialogue);
        }
    }

    // Corrutina para esperar a que el jugador quiera volver a empezar el diálogo
    private IEnumerator WaitUntilPlayerRestartDialogue()
    {
        yield return new WaitUntil(() => GetComponent<DialogueManager>().CurrentConversationPhase == ConversationPhase.Ended);
        yield return null;

        GetComponent<DialogueManager>().CurrentConversationPhase = ConversationPhase.None;

        WaitForDialogueInput();
    }

   // Método que se llama cuando se sale del rango de diálogo de un objeto
   public void ExitOfDialogueRange()
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }

        if (skipCoroutine != null)
        {
            StopCoroutine(skipCoroutine);
            skipCoroutine = null;
        }
        
        if (restartCoroutine != null)
        {
            StopCoroutine(restartCoroutine);
            restartCoroutine = null;
        }
        
        isThirdOptionUnlock = false;
        GameLogicManager.Instance.UIManager.ThirdOptionKeyInChoice.SetActive(false);
        GameLogicManager.Instance.UIManager.ThirdOptionButtonInChoice.SetActive(false);
    }
}
