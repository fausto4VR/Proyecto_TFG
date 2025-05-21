using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Puzzle4Logic : MonoBehaviour, IPuzzleLogic
{ 
    [Header("UI Objects Section")] 
    [SerializeField] private TMP_Text explanationFailureText;

    private List<string> activeSquares = new List<string>();


    void Start()
    {
        GetComponent<PuzzleUIManager>().FirstSupportText = GameStateManager.Instance.gameText.puzzle4.firstSupportText;
        GetComponent<PuzzleUIManager>().SecondSupportText = GameStateManager.Instance.gameText.puzzle4.secondSupportText;
        GetComponent<PuzzleUIManager>().ThirdSupportText = GameStateManager.Instance.gameText.puzzle4.thirdSupportText;

        GetComponent<PuzzleLogicManager>().ShowStatement(GameStateManager.Instance.gameText.puzzle4.puzzleStatementText);
    }

    // Método para limpiar los inputs de la solución en caso de fallo - Implementación de la interfaz
    public void ResetSolutionInputs()
    {
        // Los cuadrados se quedan igual, por lo que no hace falta reiniciar nada
    }

    // Método para comprobar si el resultado proporcionado es acertado o no - Implementación de la interfaz
    public void CheckResult()
    {
        if(CheckSolution())
        {
            GetComponent<PuzzleUIManager>().ShowSuccessPanel();
        }
        else if (!(CheckSolution() || activeSquares.Count == 0))
        {
            if(CheckSolutionFigure1() && CheckSolutionFigure2())
            explanationFailureText.text = "Aún así has acertado la Figura 1 y la 2";
            
            else if(CheckSolutionFigure1() && CheckSolutionFigure3())
            explanationFailureText.text = "Aún así has acertado la Figura 1 y la 3";
            
            else if(CheckSolutionFigure2() && CheckSolutionFigure3())
            explanationFailureText.text = "Aún así has acertado la Figura 2 y la 3";
            
            else if(CheckSolutionFigure1())
            explanationFailureText.text = "Aún así has acertado la Figura 1";
            
            else if(CheckSolutionFigure2())
            explanationFailureText.text = "Aún así has acertado la Figura 2";
            
            else if(CheckSolutionFigure3())
            explanationFailureText.text = "Aún así has acertado la Figura 3";

            else
            explanationFailureText.text = "No has acertado ninguna de las tres figuras.";

            GetComponent<PuzzleUIManager>().ShowFailurePanel();
        }
    }

    // Método auxiliar para actualizar la lista de cuadrados (y activar cada uno)
    public void DisplaySquare(Toggle toggle)
    {
        PuzzleUtils.UpdateToggleList(toggle, activeSquares);
    }

    // Método que implementa la lógica para comprobar si la solución dada es correcta o no
    private bool CheckSolution()
    {
        if(CheckSolutionFigure1() && CheckSolutionFigure2() && CheckSolutionFigure3()) return true;

        else return false;
    }

    // Método que implementa la lógica para verificar la solución de la figura 1
    private bool CheckSolutionFigure1()
    {
        //Figura 1 - A2:B2:C1:C2:C3
        List<string> requiredSquares = new List<string> {"Fig 1 A2", "Fig 1 B2", "Fig 1 C1", "Fig 1 C2", "Fig 1 C3"};

        return PuzzleUtils.ValidateDisplaySolution(activeSquares, requiredSquares, "Fig 1");
    }

    // Método que implementa la lógica para verificar la solución de la figura 2
    private bool CheckSolutionFigure2()
    {
        //Figura 2 - A1:A2:A3:B2:C1:C2:C3
        List<string> requiredSquares = new List<string> {"Fig 2 A1", "Fig 2 A2", "Fig 2 A3", "Fig 2 B2", "Fig 2 C1", "Fig 2 C2", 
            "Fig 2 C3"};

        return PuzzleUtils.ValidateDisplaySolution(activeSquares, requiredSquares, "Fig 2");
    }

    // Método que implementa la lógica para verificar la solución de la figura 3
    private bool CheckSolutionFigure3()
    {
        //Figura 3 - A1:A3
        List<string> requiredSquares = new List<string> {"Fig 3 A1", "Fig 3 A3"};

        return PuzzleUtils.ValidateDisplaySolution(activeSquares, requiredSquares, "Fig 3");
    }
}