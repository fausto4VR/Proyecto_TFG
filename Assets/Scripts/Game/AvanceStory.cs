using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AvanceStory : MonoBehaviour
{
    [SerializeField] private int nextStoryPhase;
    [SerializeField] private string puzzleScene;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject virtualCamara;

    private InspectDialogue inspectDialogue;    
    private bool isPuzzleTriggerObject;
    private string puzzleAssociated;

    void Start()
    {
        inspectDialogue = GetComponent<InspectDialogue>();
        if(inspectDialogue != null)
        {
            isPuzzleTriggerObject = inspectDialogue.isPuzzleTriggerObject;
            puzzleAssociated = inspectDialogue.puzzleAssociated;
        }
    }

    void Update()
    {
        // QUITAR AUX
        if(nextStoryPhase == (GameLogicManager.Instance.StoryPhaseAux + 1) && inspectDialogue.isStoryAdvanced)
        {
            inspectDialogue.isStoryAdvanced = false;

            if(!isPuzzleTriggerObject)
            {
                GameLogicManager.Instance.StoryPhaseAux = nextStoryPhase; // QUITAR
            }
            else if(isPuzzleTriggerObject)
            {
                SaveActualPosition();
                StartCoroutine(WaitAndLoadScene());
            }
        }

        // QUITAR AUX
        if(nextStoryPhase == (GameLogicManager.Instance.StoryPhaseAux + 1) && isPuzzleTriggerObject 
            && puzzleAssociated == GameLogicManager.Instance.LastPuzzleComplete 
            && GameStateManager.Instance.isPuzzleRecentlyCompleted)
        {
            player.transform.position = new Vector3(GameStateManager.Instance.actualPlayerPosition[0],
                GameStateManager.Instance.actualPlayerPosition[1], GameStateManager.Instance.actualPlayerPosition[2]);

            virtualCamara.transform.position = new Vector3(GameStateManager.Instance.actualCameraPosition[0],
                GameStateManager.Instance.actualCameraPosition[1], GameStateManager.Instance.actualPlayerPosition[2]);

            GameStateManager.Instance.isPuzzleRecentlyCompleted = false;
            GameLogicManager.Instance.StoryPhaseAux = nextStoryPhase; // QUITAR
            GetComponent<InspectDialogue>().isPuzzleReturn = true;

            if(SceneManager.GetActiveScene().name == GameStateManager.Instance.MainScene)
            {
                GameStateManager.Instance.SaveData(); 
            }
        }
    }

    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(puzzleScene);
    }

    private void SaveActualPosition()
    {
        float[] actualPlayerPosition = new float[3];
        actualPlayerPosition[0] = player.transform.position.x;
        actualPlayerPosition[1] = player.transform.position.y;
        actualPlayerPosition[2] = player.transform.position.z;
        GameStateManager.Instance.actualPlayerPosition = actualPlayerPosition;

        float[] actualCameraPosition = new float[3];
        actualCameraPosition[0] = virtualCamara.transform.position.x;
        actualCameraPosition[1] = virtualCamara.transform.position.y;
        actualCameraPosition[2] = virtualCamara.transform.position.z;
        GameStateManager.Instance.actualCameraPosition = actualCameraPosition;
    }
}
