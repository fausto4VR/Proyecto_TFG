using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleLogicManager : MonoBehaviour
{
    [SerializeField] private string puzzleScene;
    [SerializeField] private string puzzleName;

    public void ReturnToGameScene()
    {
        GameLogicManager.Instance.lastPuzzleComplete = puzzleName;
        GameStateManager.Instance.isPuzzleRecentlyCompleted = true;
        SceneManager.LoadScene(puzzleScene);
    }
}
