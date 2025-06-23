using System.Collections;
using UnityEngine;
using System.Linq;

public class TheEndDialogue : MonoBehaviour, IDialogueLogic
{
    private Coroutine waitCoroutine;
    private Coroutine skipCoroutine;
    private Coroutine skipCreditsCoroutine;
    private Coroutine restartCoroutine;
    private bool startDialogueFromClick;

    // REVISAR AUDIO
    private AudioSource creditsAudioSource;
    private AudioSource worldMusic;


    void Start()
    {
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        creditsAudioSource = audioSources[10];
        worldMusic = GameLogicManager.Instance.UIManager.WorldMusic;
    }

    // Método que se llama cuando se entra en el rango de diálogo de un objeto
    public void WaitForDialogueInput()
    {
        if (skipCoroutine != null) StopCoroutine(skipCoroutine);
        if (skipCreditsCoroutine != null) StopCoroutine(skipCreditsCoroutine);
        if (restartCoroutine != null) StopCoroutine(restartCoroutine);

        GetComponent<DialogueClickDetector>().StartDetection();

        waitCoroutine = StartCoroutine(WaitUntilPlayerStartDialogue());
    }

    // Corrutina para esperar a que el jugador quiera comenzar el diálogo
    private IEnumerator WaitUntilPlayerStartDialogue()
    {
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.E) || startDialogueFromClick)
            && GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerState is IdleState
            && GameLogicManager.Instance.CurrentStoryPhase.phaseName == StoryPhaseOption.Ending);
        yield return null;

        startDialogueFromClick = false;

        PlayerEvents.StartTalking();
        SelectDialogue();

        skipCoroutine = StartCoroutine(WaitToSkipDialogue());
    }

    // Método para iniciar la conversación al hacer clic con el cursor sobre el personaje
    public void TryStartDialogueOnClick()
    {
        if (!GetComponent<DialogueManager>().IsPlayerInRange
            || GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerState is not IdleState
            || GetComponent<DialogueManager>().CurrentConversationPhase != ConversationPhase.None
            || GameLogicManager.Instance.CurrentStoryPhase.phaseName != StoryPhaseOption.Ending)
            return;

        startDialogueFromClick = true;
    }

    // Método para seleccionar el diálogo correspondiente en función de la fase de la historia
    private void SelectDialogue()
    {
        string[] dialogueLines = GameStateManager.Instance.gameConversations.theEndConversations
            .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.dialogues
            .FirstOrDefault(chat => chat.guilty == GameLogicManager.Instance.Guilty)?.dialogue
            .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

        string[] characterNameLines = GameStateManager.Instance.gameConversations.theEndConversations
            .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.dialogues
            .FirstOrDefault(chat => chat.guilty == GameLogicManager.Instance.Guilty)?.dialogue
            .Select(dialogue => dialogue.speaker).ToArray() ?? new string[0];

        GetComponent<DialogueManager>().StartConversation(ConversationType.TheEndDialogue, dialogueLines, characterNameLines,
            success =>
            {
                if (!success) WaitForDialogueInput();
            });
    }

    // Corrutina para esperar a que el jugador quiera saltarse el diálogo una vez empezado
    private IEnumerator WaitToSkipDialogue()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        yield return null;

        if (GetComponent<DialogueManager>().CurrentConversationPhase != ConversationPhase.Ended)
            GetComponent<DialogueManager>().EndConversation(ConversationType.TheEndDialogue);
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
        StopAllCoroutines();

        if (waitCoroutine != null) waitCoroutine = null;
        if (skipCoroutine != null) skipCoroutine = null;
        if (skipCreditsCoroutine != null) skipCreditsCoroutine = null;
        if (restartCoroutine != null) restartCoroutine = null;

        GetComponent<DialogueClickDetector>().StopDetection();
    }

    // Método para terminar el diálogo del final del juego y mostrar los créditos
    public void FinishTheEndDialogue()
    {
        PlayerEvents.ShowCredits();

        worldMusic.Pause();
        creditsAudioSource.Play();

        GameLogicManager.Instance.UIManager.TheEndSection.SetActive(true);
        GameLogicManager.Instance.UIManager.Credits.SetActive(true);

        skipCreditsCoroutine = StartCoroutine(WaitUntilFinishCredits());
    }

    // Corrutina para esperar a que el jugador quiera saltarse los créditos una vez empezados
    private IEnumerator WaitUntilFinishCredits()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.E));
        yield return null;

        creditsAudioSource.Stop();
        worldMusic.Play();

        GameLogicManager.Instance.UIManager.Credits.SetActive(false);
        GameLogicManager.Instance.UIManager.TheEndSection.SetActive(false);

        PlayerEvents.FinishShowingInformation();
        
        restartCoroutine = StartCoroutine(WaitUntilPlayerRestartDialogue());
    }
}
