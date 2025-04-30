using System.Collections;
using UnityEngine;
using System.Linq;

public class TheEndDialogue : MonoBehaviour, IDialogueLogic
{
    private Coroutine waitCoroutine;
    private Coroutine skipCoroutine;
    private Coroutine restartCoroutine;


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
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.E) 
            && GameLogicManager.Instance.CurrentStoryPhase.phaseName == StoryPhaseOption.Ending);
        yield return null;

        PlayerEvents.StartTalking();
        SelectDialogue();

        skipCoroutine = StartCoroutine(WaitToSkipDialogue());
        restartCoroutine = StartCoroutine(WaitUntilPlayerRestartDialogue());
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
        
        GetComponent<DialogueManager>().StartConversation(ConversationType.StoryPhaseDialogueConversation, 
            dialogueLines, characterNameLines);
    }

    // Corrutina para esperar a que el jugador quiera saltarse el diálogo una vez empezado
    private IEnumerator WaitToSkipDialogue()
    {
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.E));
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
    }
}
