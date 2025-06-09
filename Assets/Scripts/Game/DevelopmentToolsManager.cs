using UnityEngine;
using UnityEngine.SceneManagement;

public class DevelopmentToolsManager : MonoBehaviour
{
    private string currentSceneName;
    private bool isMessageDisplayed;

    void Update()
    {
        // Solo permitir herramientas en el editor
        if (!IsDevelopmentMode()) return;

        if (!isMessageDisplayed)
        {
            Debug.Log($"Se han activado las herramientas de desarrollo en la escena {currentSceneName}.");
            isMessageDisplayed = true;
        }

        // Se obtiene un resumen de los aspectos más importantes del juego pulsando V
        if (Input.GetKeyDown(KeyCode.V) && currentSceneName != "MenuScene")
        {
            Debug.Log("Guilty: " + GameLogicManager.Instance.Guilty);
            Debug.Log("First Clue: " + GameLogicManager.Instance.Clues[0]);
            Debug.Log("Second Clue: " + GameLogicManager.Instance.Clues[1]);
            Debug.Log("Third Clue: " + GameLogicManager.Instance.Clues[2]);
            Debug.Log("Game Started: " + GameStateManager.Instance.IsGameStarted);
            Debug.Log("Story Phase: " + GameLogicManager.Instance.CurrentStoryPhase.phaseName);
            Debug.Log("Story Subphase: " + GameLogicManager.Instance.CurrentStoryPhase.currentSubphase.subphaseName);
            string LastPuzzle = GameLogicManager.Instance.LastPuzzleComplete;
            Debug.Log((LastPuzzle == null || LastPuzzle == "") ? "Last Puzzle Complete: None" : "Last Puzzle Complete: " + LastPuzzle);
            string cluesContent = string.Join(", ", GameLogicManager.Instance.KnownClues);
            Debug.Log("Known Clues: " + cluesContent);
            string suspectsContent = string.Join(", ", GameLogicManager.Instance.KnownSuspects);
            Debug.Log("Known Suspects: " + suspectsContent);
            string tutorialsContent = string.Join(", ", GameLogicManager.Instance.KnownTutorials);
            Debug.Log("Known Tutorials: " + tutorialsContent);
            string dialoguesContent = string.Join(", ", GameLogicManager.Instance.KnownDialogues);
            Debug.Log("Known Dialogues: " + dialoguesContent);
            Debug.Log("Is Bad Ending: " + GameLogicManager.Instance.IsBadEnding);
            Debug.Log("End Opportunities: " + GameLogicManager.Instance.EndOpportunities);
            Debug.Log("Total Puzzle Points: " + GameLogicManager.Instance.TotalPuzzlePoints);
            Debug.Log("Player State: " + GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerState.StateName);
            Debug.Log("Player Temporarily State: " + GameLogicManager.Instance.TemporalPlayerState.StateName);
            Debug.Log("Player Position: (" + GameLogicManager.Instance.Player.transform.position.x + ", "
                + GameLogicManager.Instance.Player.transform.position.y + ", "
                + GameLogicManager.Instance.Player.transform.position.z + ")");
            Debug.Log("Camera Position: (" + GameLogicManager.Instance.VirtualCamera.transform.position.x + ", "
                + GameLogicManager.Instance.VirtualCamera.transform.position.y + ", "
                + GameLogicManager.Instance.VirtualCamera.transform.position.z + ")");
        }

        // Se obtiene la fase actual de la historia en formato [fase].[subfase] pulsando P
        if (Input.GetKeyDown(KeyCode.P) && SceneManager.GetActiveScene().name != "MenuScene")
        {
            Debug.Log("Current Phase: " + GameLogicManager.Instance.CurrentStoryPhase.GetPhaseToString());
        }

        // Se obtiene la lista de fases de la historia en formato [fase].[subfase] pulsando T
        if (Input.GetKeyDown(KeyCode.T))
        {
            string gameStoryString = string.Join(", ", StoryStateManager.CreateSubphasesList());
            Debug.Log("Game Story: " + gameStoryString);
        }

        // Se puede reiniciar, guardar y cargar los datos pulsando R, G y C, respectivamente
        if (Input.GetKeyDown(KeyCode.R) && !currentSceneName.Contains("Puzzle"))
        {
            GameStateManager.Instance.ResetData();
        }
        if (Input.GetKeyDown(KeyCode.G) && currentSceneName != "MenuScene" && !currentSceneName.Contains("Puzzle"))
        {
            GameStateManager.Instance.SaveData();
        }
        if (Input.GetKeyDown(KeyCode.C) && currentSceneName != "MenuScene" && !currentSceneName.Contains("Puzzle"))
        {
            GameStateManager.Instance.LoadData(true);
        }
    }

    // Método que se llama cuando se activa el objeto y sirve para suscribirse a eventos
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        isMessageDisplayed = false;

        if (currentSceneName == null) currentSceneName = SceneManager.GetActiveScene().name;
    }

    // Método que se llama cuando se desactiva o se destruye el objeto y sirve para desuscribirse a eventos
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        isMessageDisplayed = true;
    }

    // Método que se ejecuta cada vez que se carga una escena
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
    }
    
    // Método que verifica si se está ejecutando desde el Editor
    bool IsDevelopmentMode()
    {
        #if UNITY_EDITOR
                return Application.isPlaying && Application.isEditor;
        #else
                return false;
        #endif
    }
}
