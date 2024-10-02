using UnityEngine;
using TMPro;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class Puzzle5Logic : MonoBehaviour
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
            GetComponent<PuzzleUIManager>().isCorrectResult = 0;
        }
        else if(CheckSolution(solutionString))
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
        return Regex.Replace(input, @"[^\w\s:]", "");
    }

    private bool CheckSolution(string figureString)
    {
        //A6:B8:C2:E1:H3 y D4:F7:G5
        bool isCorrect = true;
        bool isFullDoublePoints = figureString.All(c => c == ':');

        if(figureString != "" && !isFullDoublePoints)
        {
            string[] words = figureString.Split(':');
            foreach(string word in words)
            {
                if(word != "" && word != "A6" && word != "B8" && word != "C2" && word != "E1" && word != "H3" && word != "D4" 
                    && word != "F7" && word != "G5")
                {
                    isCorrect = false;
                }
            }
        }
        else
        {
            isCorrect = false;
        }

        return isCorrect;
    }
}
