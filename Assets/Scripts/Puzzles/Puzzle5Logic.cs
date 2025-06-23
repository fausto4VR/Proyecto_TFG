using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Puzzle5Logic : MonoBehaviour, IPuzzleLogic
{   
    private int totalQueensPlaced;
    private List<string> activeQueens = new List<string>();


    void Start()
    {
        GetComponent<PuzzleUIManager>().FirstSupportText = GameStateManager.Instance.gameText.puzzle5.firstSupportText;
        GetComponent<PuzzleUIManager>().SecondSupportText = GameStateManager.Instance.gameText.puzzle5.secondSupportText;
        GetComponent<PuzzleUIManager>().ThirdSupportText = GameStateManager.Instance.gameText.puzzle5.thirdSupportText;

        GetComponent<PuzzleLogicManager>().ShowStatement(GameStateManager.Instance.gameText.puzzle5.puzzleStatementText);
    }

    // Método para limpiar los inputs de la solución en caso de fallo - Implementación de la interfaz
    public void ResetSolutionInputs()
    {
        // Las reinas se quedan igual, por lo que no hace falta reiniciar nada
    }

    // Método para comprobar si el resultado proporcionado es acertado o no - Implementación de la interfaz
    public void CheckResult()
    {
        if(CheckSolution())
        {
            GetComponent<PuzzleUIManager>().ShowSuccessPanel();
        }
        else if (!(CheckSolution() || activeQueens.Count == 0))
        {
            GetComponent<PuzzleUIManager>().ShowFailurePanel();
        }
    }

    // Método auxiliar para actualizar la lista de reinas (y activar cada una) y el número de ellas puestas
    public void DisplayQueen(Button button)
    {
        totalQueensPlaced = PuzzleUtils.UpdateButtonList(button, activeQueens, totalQueensPlaced);
    }

    // Método que implementa la lógica para comprobar si la solución dada es correcta o no
    private bool CheckSolution()
    {
        //A6:B8:C2:E1:H3 y D4:F7:G5
        List<string> requiredQueens = new List<string> {"A6", "B8", "C2", "E1", "H3"};

        return PuzzleUtils.ValidateDisplaySolution(activeQueens, requiredQueens);
    }
}
