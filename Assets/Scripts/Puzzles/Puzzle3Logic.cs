using UnityEngine;
using TMPro;
using System.Text;
using System.Text.RegularExpressions; 

public class Puzzle3Logic : MonoBehaviour
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
        solutionString = RemoveAccents(solutionString);
        solutionString = RemovePunctuation(solutionString); 
        solutionString = RemoveAccents(solutionString);

        if (solutionString == "")
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 0;
        }
        else if (solutionString == "ADKJCGBFNHIONEMPQRLS")
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
        input = input.Replace('Á', 'A')
                    .Replace('É', 'E')
                    .Replace('Í', 'I')
                    .Replace('Ó', 'O')
                    .Replace('Ú', 'U');
        return input;
    }

    public string RemovePunctuation(string input)
    {
        return Regex.Replace(input, @"[^a-zA-Z0-9\sÑñ]", "");
    }
}
