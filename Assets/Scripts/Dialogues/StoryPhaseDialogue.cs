using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;

// Enum de los tipos de diálogo de fase de la historia que existen
public enum StoryPhaseDialogueType
{
    Conversation, Trigger
}

public class StoryPhaseDialogue : MonoBehaviour, IDialogueLogic
{    
    [Header("Variable Section")]
    [SerializeField] private StoryPhaseDialogueType dialogueType;
    
    private Coroutine waitCoroutine;
    private Coroutine skipCoroutine;
    private Coroutine restartCoroutine;
    private string advanceStoryPhase;    
    private bool isAdvanceStory;
    

    // Método que se llama cuando se entra en el rango de diálogo de un objeto
    public void WaitForDialogueInput()
    {
        if (skipCoroutine != null) StopCoroutine(skipCoroutine);        
        if (restartCoroutine != null) StopCoroutine(restartCoroutine);

        if (dialogueType == StoryPhaseDialogueType.Conversation) waitCoroutine = StartCoroutine(WaitUntilPlayerStartDialogue());
        else if (dialogueType == StoryPhaseDialogueType.Trigger) StartTriggerDialogue();
    }

    // Corrutina para esperar a que el jugador quiera comenzar el diálogo
    private IEnumerator WaitUntilPlayerStartDialogue()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        yield return null;

        PlayerEvents.StartTalking();
        SelectDialogue(ConversationType.StoryPhaseDialogueConversation);

        skipCoroutine = StartCoroutine(WaitToSkipDialogue());
        restartCoroutine = StartCoroutine(WaitUntilPlayerRestartDialogue());
    }

    // Método para seleccionar el diálogo correspondiente en función de la fase de la historia
    private void SelectDialogue(ConversationType conversationType)
    {
        string validSubphase = FindValidSubphase();

        string[] dialogueLines = GameStateManager.Instance.gameConversations.storyPhaseConversations
            .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.dialogues
            .FirstOrDefault(chat => $"{chat.phase}.{chat.subphase}" == validSubphase)?.dialogue
            .Select(dialogue => dialogue.line).ToArray() ?? new string[0];

        string[] characterNameLines = GameStateManager.Instance.gameConversations.storyPhaseConversations
            .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.dialogues
            .FirstOrDefault(chat => $"{chat.phase}.{chat.subphase}" == validSubphase)?.dialogue
            .Select(dialogue => dialogue.speaker).ToArray() ?? new string[0];

        CheckIsNecesaryAdvanceStory(validSubphase);
        
        GetComponent<DialogueManager>().StartConversation(conversationType, dialogueLines, characterNameLines);
    }

    // Método para buscar la subfase inmediatamente superior a la actual en la lista de subfases (indican el salto)
    private string FindValidSubphase()
    {
        List<string> selectedSubphases = GameStateManager.Instance.gameConversations.storyPhaseConversations
            .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.dialogues
            .Select(chat => $"{chat.phase}.{chat.subphase}")
            .OrderBy(order => StoryStateManager.CreateSubphasesList().IndexOf(order))
            .ToList() ?? new List<string>();
            
        bool isValidOrder = ValidateSubphasesOrder(selectedSubphases);

        string validSubphase = null;

        if(isValidOrder)
        {
            var reversedSubphases = selectedSubphases.AsEnumerable().Reverse().ToList();

            foreach (var subphase in reversedSubphases)
            {
                var comparison = GameLogicManager.Instance.CurrentStoryPhase.ComparePhase(subphase);

                if (comparison == SubphaseTemporaryOrder.IsDistantAfter || comparison == SubphaseTemporaryOrder.IsRecentAfter ||
                    comparison == SubphaseTemporaryOrder.IsCurrent)
                {
                    validSubphase = subphase;
                }
                else
                {
                    break;
                }
            }
            
            if (validSubphase == null) validSubphase = selectedSubphases.Last();
        }

        return validSubphase;
    }

    // Método para validad que las subfases seleccionadas estén en el orden correcto según el flujo de la historia
    private bool ValidateSubphasesOrder(List<string> selectedSubphases)
    {
        bool isValidOrder = true;
        List<string> subphases = StoryStateManager.CreateSubphasesList();

        for (int i = 1; i < selectedSubphases.Count; i++)
        {
            int previousIndex = subphases.IndexOf(selectedSubphases[i - 1]);
            int currentIndex = subphases.IndexOf(selectedSubphases[i]);

            if (previousIndex == -1 || currentIndex == -1)
            {
                Debug.LogError($"Una de las subfases '{selectedSubphases[i - 1]}' o '{selectedSubphases[i]}' no existe en la lista de fases.");
                isValidOrder = false;
                break;
            }

            if (previousIndex >= currentIndex)
            {
                Debug.LogError($"Las subfases no están en orden cronológico: '{selectedSubphases[i - 1]}' debería ir antes que '{selectedSubphases[i]}'.");
                isValidOrder = false;
                break;
            }
        }

        return isValidOrder;
    }

    // Método para comprobar si es necesario avanzar la historia
    private void CheckIsNecesaryAdvanceStory(string validSubphase)
    {
        isAdvanceStory = GameStateManager.Instance.gameConversations.storyPhaseConversations
            .FirstOrDefault(conversation => conversation.objectName == gameObject.name)?.dialogues
            .FirstOrDefault(chat => $"{chat.phase}.{chat.subphase}" == validSubphase)?.advanceStory ?? false;

        advanceStoryPhase = GameLogicManager.Instance.CurrentStoryPhase.GetPhaseToString();
    }

    // Método para dar comienzo a un diálogo de tipo trigger al entrar en su rango 
    private void StartTriggerDialogue()
    {
        string objectName = gameObject.name;

        if (!GameLogicManager.Instance.KnownDialogues.TryGetValue(objectName, out bool isKnown) || !isKnown)
        {
            Dictionary<string, bool> updatedDialogues = GameLogicManager.Instance.KnownDialogues;
            updatedDialogues[gameObject.name] = true;
            GameLogicManager.Instance.KnownDialogues = updatedDialogues;

            PlayerEvents.StartTalking();
            SelectDialogue(ConversationType.StoryPhaseDialogueTrigger);

            skipCoroutine = StartCoroutine(WaitToSkipDialogue());
        }
    }    

    // Corrutina para esperar a que el jugador quiera saltarse el diálogo una vez empezado
    private IEnumerator WaitToSkipDialogue()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        yield return null;

        if (GetComponent<DialogueManager>().CurrentConversationPhase != ConversationPhase.Ended)
        {
            ConversationType currentConversationType = ConversationType.StoryPhaseDialogueConversation;

            if (dialogueType == StoryPhaseDialogueType.Trigger) 
            currentConversationType = ConversationType.StoryPhaseDialogueTrigger;

            GetComponent<DialogueManager>().EndConversation(currentConversationType);
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
    }

    // Método para obtener el tipo de diálogo de fase de la historia que es
    public StoryPhaseDialogueType DialogueType
    {
        get { return dialogueType; }
    }

    // Método para obtener el nombre de la fase en el que se debe avanzar la historia
    public string AdvanceStoryPhase
    {
        get { return advanceStoryPhase; }
    }

    // Métodos para obtener y para cambiar si es necesario avanzar la historia
    public bool IsAdvanceStory
    {
        get { return isAdvanceStory; }
        set { isAdvanceStory = value; }
    }
}
