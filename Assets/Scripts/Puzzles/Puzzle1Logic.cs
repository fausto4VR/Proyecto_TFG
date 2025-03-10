using UnityEngine;
using TMPro;

public class Puzzle1Logic : MonoBehaviour, IPuzzleLogic
{

    [Header("Solution Section")]
    [SerializeField] private TMP_InputField inputField;

    void Start()
    {
        GetComponent<PuzzleUIManager>().SetFirstSupportText(GameStateManager.Instance.gameText.puzzle_1.first_support_text);
        GetComponent<PuzzleUIManager>().SetSecondSupportText(GameStateManager.Instance.gameText.puzzle_1.second_support_text);
        GetComponent<PuzzleUIManager>().SetThirdSupportText(GameStateManager.Instance.gameText.puzzle_1.third_support_text);

        GetComponent<PuzzleLogicManager>().ShowStatement(GameStateManager.Instance.gameText.puzzle_1.puzzle_statement_text);

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
        solutionString = PuzzleUtils.RemoveNonAlphanumeric(solutionString); 
        solutionString = PuzzleUtils.RemoveAccents(solutionString);        
        solutionString = solutionString.ToUpper();

        if(solutionString == "TIEMPO")
        {
            GetComponent<PuzzleUIManager>().ShowSuccessPanel();
        }
        else if (!(solutionString == "TIEMPO" || solutionString == ""))
        {
            GetComponent<PuzzleUIManager>().ShowFailurePanel();
        }
    }

    // Método para filtrar caracteres no alfanuméricos en el InputField
    private void FilterInput(string textToCheck)
    {
        inputField.text = PuzzleUtils.RemoveNonAlphanumeric(textToCheck); 
    }
}
