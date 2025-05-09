using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Puzzle3Logic : MonoBehaviour, IPuzzleLogic
{   
    private List<string> activePaths = new List<string>();


    void Start()
    {
        GetComponent<PuzzleUIManager>().FirstSupportText = GameStateManager.Instance.gameText.puzzle3.firstSupportText;
        GetComponent<PuzzleUIManager>().SecondSupportText = GameStateManager.Instance.gameText.puzzle3.secondSupportText;
        GetComponent<PuzzleUIManager>().ThirdSupportText = GameStateManager.Instance.gameText.puzzle3.thirdSupportText;

        GetComponent<PuzzleLogicManager>().ShowStatement(GameStateManager.Instance.gameText.puzzle3.puzzleStatementText);
    }

    // Método para limpiar los inputs de la solución en caso de fallo - Implementación de la interfaz
    public void ResetSolutionInputs()
    {
        // Los caminos se quedan igual, por lo que no hace falta reiniciar nada
    }

    // Método para comprobar si el resultado proporcionado es acertado o no - Implementación de la interfaz
    public void CheckResult()
    {
        if(CheckSolution())
        {
            GetComponent<PuzzleUIManager>().ShowSuccessPanel();
        }
        else if (!(CheckSolution() || activePaths.Count == 0))
        {
            GetComponent<PuzzleUIManager>().ShowFailurePanel();
        }
    }

    // Método auxiliar para actualizar la lista de caminos (y activar cada uno)
    public void DisplayPath(Toggle toggle)
    {
        PuzzleUtils.UpdateToggleList(toggle, activePaths);
    }

    // Método que implementa la lógica para comprobar si la solución dada es correcta o no
    private bool CheckSolution()
    {
        // Camino: ADKJCGBFÑHIONEMPQRLS
        List<string> requiredPath = new List<string> {"AD", "DK", "JK", "CJ", "CG", "BG", "BF", "FÑ", "HÑ", "HI", "IO", "NO", 
            "EN", "EM", "MP", "PQ", "QR", "LR", "LS"};

        return PuzzleUtils.ValidateDisplaySolution(activePaths, requiredPath);
    }
}
