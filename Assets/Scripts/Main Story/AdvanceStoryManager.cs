using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AdvanceStoryManager : MonoBehaviour
{
    [Header("Variable Section")]
    [SubphaseSelector]
    [SerializeField] private string selectedSubphase;

    private InspectDialogue inspectDialogue;
    private StoryPhaseDialogue storyPhaseDialogue;
    private TheEndManager theEndManager;


    void Start()
    {
        inspectDialogue = GetComponent<InspectDialogue>();
        storyPhaseDialogue = GetComponent<StoryPhaseDialogue>();
        theEndManager = GetComponent<TheEndManager>();

        if (GameLogicManager.Instance.TemporalPuzzleObject == gameObject.name &&
            (GameLogicManager.Instance.IsPuzzleCompleted || GameLogicManager.Instance.IsPuzzleIncomplete))
        GameLogicManager.Instance.LoadTemporarilyPosition();

        StartCoroutine(RestoreStateAfterFrame());

        if(GetComponent<InspectDialogue>() != null) selectedSubphase = GetComponent<InspectDialogue>().SelectedSubphase;
    }

    // Corrutina para esperar un frame y asegurarse que todo está correctamente inicializado
    private IEnumerator RestoreStateAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        AdvanceStoryAfterPuzzle();
    }

    // Método para obtener la fase seleccionada
    public string SelectedSubphase
    {
        get { return selectedSubphase; }
    }

    // Método para cargar la posición del jugador al volver de un puzle y avanzar la historia
    private void AdvanceStoryAfterPuzzle()
    {
        if (inspectDialogue != null && GameLogicManager.Instance.TemporalPuzzleObject == gameObject.name)
        {
            if (GameLogicManager.Instance.CurrentStoryPhase.ComparePhase(selectedSubphase) == SubphaseTemporaryOrder.IsCurrent
                && inspectDialogue.IsPuzzleTriggerObject && GameLogicManager.Instance.IsPuzzleCompleted
                && inspectDialogue.PuzzleAssociated == GameLogicManager.Instance.LastPuzzleComplete)
            {
                GameLogicManager.Instance.IsPuzzleCompleted = false;

                GameLogicManager.Instance.CurrentStoryPhase = StoryStateManager.AdvanceStory(
                    GameLogicManager.Instance.CurrentStoryPhase, GetPath(transform));

                GameStateManager.Instance.SaveData();
                inspectDialogue.ShowAfterPuzzleDialogue();
            }
            else if (inspectDialogue.IsPuzzleTriggerObject && GameLogicManager.Instance.IsPuzzleIncomplete)
            {
                PlayerEvents.FinishTalkingWithoutClue();
                GameLogicManager.Instance.IsPuzzleIncomplete = false;
            }
        }
    }

    // Método para avanzar el estado de la historia
    public void AdvanceStoryState()
    {
        if(GetComponent<StoryPhaseDialogue>() != null) selectedSubphase = GetComponent<StoryPhaseDialogue>().AdvanceStoryPhase;

        if(GameLogicManager.Instance.CurrentStoryPhase.ComparePhase(selectedSubphase) == SubphaseTemporaryOrder.IsCurrent)
        {
            if(inspectDialogue != null)
            {
                if(!inspectDialogue.IsPuzzleTriggerObject)
                {
                    PlayerEvents.FinishTalkingWithoutClue();
                    GameLogicManager.Instance.CurrentStoryPhase = StoryStateManager.AdvanceStory(
                        GameLogicManager.Instance.CurrentStoryPhase, GetPath(transform));
                }
                else
                {
                    GameLogicManager.Instance.SaveTemporarilyPosition(gameObject.name);
                    StartCoroutine(WaitAndLoadScene());
                }
            }

            if(storyPhaseDialogue != null || theEndManager != null)
            {
                GameLogicManager.Instance.CurrentStoryPhase = StoryStateManager.AdvanceStory(
                    GameLogicManager.Instance.CurrentStoryPhase, GetPath(transform));
            }
        }
    }

    // Corrutina para agragar un pequeño tiempo de transición antes de lanzar el puzle
    private IEnumerator WaitAndLoadScene()
    {
        GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerState.OnExit();
        GameLogicManager.Instance.UIManager.OutDetectionPanel.SetActive(true);

        GameStateManager.Instance.TransitionManager.StartOutTransition(TransitionType.Fade);
        yield return new WaitForSeconds(GameStateManager.Instance.TransitionDuration);
        
        if (Application.CanStreamedLevelBeLoaded(inspectDialogue.PuzzleScene))
        {
            GameLogicManager.Instance.UIManager.OutDetectionPanel.SetActive(false);
            SceneManager.LoadScene(inspectDialogue.PuzzleScene);
        }
        else
        {
            Debug.LogError($"No se pudo encontrar la escena '{inspectDialogue.PuzzleScene}'. Revisa el nombre o si está añadida en Build Settings.");
        }
    }

    // Método para generar un string a partir de la jerarquía del objeto. Se usará como el ID del objeto
    private string GetPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}
