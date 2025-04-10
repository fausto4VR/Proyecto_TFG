using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AdvanceStoryManager : MonoBehaviour
{
    [Header("Variable Section")]
    [SerializeField] private float transitionTime = 0.5f;

    [SubphaseSelector]
    [SerializeField] private string selectedSubphase;

    private InspectDialogue inspectDialogue;
    private StoryPhaseDialogue storyPhaseDialogue;


    void Start()
    {
        inspectDialogue = GetComponent<InspectDialogue>();
        storyPhaseDialogue = GetComponent<StoryPhaseDialogue>();
        StartCoroutine(RestoreStateAfterFrame());
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
        if(inspectDialogue != null)
        {
            if(GameLogicManager.Instance.CurrentStoryPhase.CheckCurrentPhase(selectedSubphase) == SubphaseTemporaryOrder.IsCurrent 
                && inspectDialogue.isPuzzleTriggerObject && GameLogicManager.Instance.IsPuzzleCompleted
                && inspectDialogue.puzzleAssociated == GameLogicManager.Instance.LastPuzzleComplete)
            {
                GameLogicManager.Instance.IsPuzzleCompleted = false;
                GameLogicManager.Instance.LoadTemporarilyPosition();
                
                GameLogicManager.Instance.CurrentStoryPhase = StoryStateManager.AdvanceStory(
                    GameLogicManager.Instance.CurrentStoryPhase, GetPath(transform));

                GameStateManager.Instance.SaveData();

                // QUITAR ----------------------------------------------------------
                GetComponent<InspectDialogue>().isPuzzleReturn = true;
                // -----------------------------------------------------------------
            }
        }
    }

    // Método para avanzar el estado de la historia
    public void AdvanceStoryState()
    {
        if(GameLogicManager.Instance.CurrentStoryPhase.CheckCurrentPhase(selectedSubphase) == SubphaseTemporaryOrder.IsCurrent)
        {
            if(inspectDialogue != null)
            {
                if(!inspectDialogue.isPuzzleTriggerObject)
                {
                    GameLogicManager.Instance.CurrentStoryPhase = StoryStateManager.AdvanceStory(
                        GameLogicManager.Instance.CurrentStoryPhase, GetPath(transform));
                }
                else
                {
                    GameLogicManager.Instance.SaveTemporarilyPosition();
                    StartCoroutine(WaitAndLoadScene());
                }
            }

            if(storyPhaseDialogue != null)
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
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(inspectDialogue.puzzleScene);
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
