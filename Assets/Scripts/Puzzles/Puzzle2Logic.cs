using UnityEngine;
using TMPro;
using System.Text;
using System.Text.RegularExpressions; 

public class Puzzle2Logic : MonoBehaviour
{   
    [SerializeField, TextArea(6,12)] private string firstSupportText;    
    [SerializeField, TextArea(6,12)] private string secondSupportText;    
    [SerializeField, TextArea(6,12)] private string thirdSupportText; 

    public GameObject inputField;

    void Start()
    {
        GetComponent<PuzzleUIManager>().SetFirstSupportText(GameStateManager.Instance.gameText.puzzle_1.first_support_text);
        GetComponent<PuzzleUIManager>().SetSecondSupportText(GameStateManager.Instance.gameText.puzzle_1.second_support_text);
        GetComponent<PuzzleUIManager>().SetThirdSupportText(GameStateManager.Instance.gameText.puzzle_1.third_support_text);
    }

    void Update()
    {
        if(GetComponent<PuzzleUIManager>().isCheckTrigger)
        {
            GetComponent<PuzzleUIManager>().isCheckTrigger = false;
            CheckResult();
        }

        if(GetComponent<PuzzleUIManager>().isNecesaryResetInputs)
        {
            inputField.GetComponent<TMP_InputField>().text = "";
            GetComponent<PuzzleUIManager>().isNecesaryResetInputs = false;
        }
    }

    public void CheckResult()
    {
        string solutionString = inputField.GetComponent<TMP_InputField>().text;
        solutionString = solutionString.Replace(" ", "");
        solutionString = solutionString.ToUpper();
        solutionString = RemovePunctuation(solutionString); 
        solutionString = RemoveAccents(solutionString);

        if(solutionString == "")
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = ResultType.Empty;
        }
        else if(solutionString == "12:25:10" || solutionString == "XII:V:II")
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = ResultType.Success;
        }
        else
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = ResultType.Failure;
        }
    }

    private string RemoveAccents(string input)
    {
        string normalizedString = input.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);

            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public string RemovePunctuation(string input)
    {
        return Regex.Replace(input, @"[^\w\s:]", "");
    }
}
