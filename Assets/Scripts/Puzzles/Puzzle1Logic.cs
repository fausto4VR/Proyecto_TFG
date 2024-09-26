using UnityEngine;
using TMPro;
using System.Text;
using System.Text.RegularExpressions; 

public class Puzzle1Logic : MonoBehaviour
{
        
    [SerializeField, TextArea(6,12)] private string firstSupportText;    
    [SerializeField, TextArea(6,12)] private string secondSupportText;    
    [SerializeField, TextArea(6,12)] private string thirdSupportText;

    public GameObject inputField;

    void Start()
    {
        GetComponent<PuzzleUIManager>().firstSupportText = firstSupportText;
        GetComponent<PuzzleUIManager>().secondSupportText = secondSupportText;
        GetComponent<PuzzleUIManager>().thirdSupportText = thirdSupportText;
    }

    void Update()
    {
        if(GetComponent<PuzzleUIManager>().isCheckTrigger)
        {
            GetComponent<PuzzleUIManager>().isCheckTrigger = false;
            CheckResult();
        }

        if(GetComponent<PuzzleUIManager>().isReturnToPuzzleAfterFail)
        {
            inputField.GetComponent<TMP_InputField>().text = "";
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
            GetComponent<PuzzleUIManager>().isCorrectResult = 0;
        }
        else if(solutionString == "TIEMPO")
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 1;
        }
        else
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 2;
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
        return Regex.Replace(input, @"[^\w\s]", "");
    }
}
