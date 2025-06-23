using UnityEngine;
using System.Linq;

public class Puzzle0Logic : MonoBehaviour, IPuzzleLogic
{
    void Start()
    {
        GetComponent<PuzzleUIManager>().FirstSupportText = GameStateManager.Instance.gameText.puzzle0.firstSupportText;
        GetComponent<PuzzleUIManager>().SecondSupportText = GameStateManager.Instance.gameText.puzzle0.secondSupportText;
        GetComponent<PuzzleUIManager>().ThirdSupportText = GameStateManager.Instance.gameText.puzzle0.thirdSupportText;

        GetComponent<PuzzleLogicManager>().ShowStatement(GameStateManager.Instance.gameText.puzzle0.puzzleStatementText);        
    }

    // Método para limpiar los inputs de la solución en caso de fallo - Implementación de la interfaz
    public void ResetSolutionInputs()
    {
        // No hay ningún input, por lo que no hace falta reiniciar nada
    }

    // Método para comprobar si el resultado proporcionado es acertado o no - Implementación de la interfaz
    public void CheckResult()
    {
        if(GetComponent<PuzzleLogicManager>().PuzzleSupports.All(support => support))
        {
            GetComponent<PuzzleUIManager>().ShowSuccessPanel();
        }
        else
        {
            GetComponent<PuzzleUIManager>().ShowFailurePanel();
        }
    }
}
