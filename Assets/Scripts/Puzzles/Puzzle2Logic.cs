using UnityEngine;
using TMPro;

public class Puzzle2Logic : MonoBehaviour, IPuzzleLogic
{   
    [Header("Solution Section")]
    [SerializeField] private TMP_InputField inputField;


    void Start()
    {
        GetComponent<PuzzleUIManager>().FirstSupportText = GameStateManager.Instance.gameText.puzzle2.firstSupportText;
        GetComponent<PuzzleUIManager>().SecondSupportText = GameStateManager.Instance.gameText.puzzle2.secondSupportText;
        GetComponent<PuzzleUIManager>().ThirdSupportText = GameStateManager.Instance.gameText.puzzle2.thirdSupportText;

        GetComponent<PuzzleLogicManager>().ShowStatement(GameStateManager.Instance.gameText.puzzle2.puzzleStatementText);

        // Agrega el método FilterInput como listener para detectar cambios en el InputField
        inputField.onValueChanged.AddListener(FilterInput);
    }

    // Método para limpiar los inputs de la solución en caso de fallo - Implementación de la interfaz
    public void ResetSolutionInputs()
    {
        inputField.GetComponent<TMP_InputField>().text = "";
    }

    // Método para comprobar si el resultado proporcionado es acertado o no - Implementación de la interfaz
    public void CheckResult()
    {
        
        string solutionString = inputField.GetComponent<TMP_InputField>().text;
        solutionString = solutionString.Replace(" ", "");
        solutionString = PuzzleUtils.RemoveNonClockSymbols(solutionString);
        solutionString = solutionString.ToUpper();

        if(solutionString == "12:25:10" || solutionString == "XII:V:II")
        {
            GetComponent<PuzzleUIManager>().ShowSuccessPanel();
        }
        else if (!(solutionString == "12:25:10" || solutionString == "XII:V:II" || solutionString == ""))
        {
            GetComponent<PuzzleUIManager>().ShowFailurePanel();
        }
    }

    // Método para filtrar caracteres no apropiados para un reloj en el InputField
    private void FilterInput(string textToCheck)
    {
        inputField.text = PuzzleUtils.RemoveNonClockSymbols(textToCheck); 
    }
}
