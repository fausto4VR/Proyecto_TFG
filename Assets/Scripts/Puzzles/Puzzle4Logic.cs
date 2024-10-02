using UnityEngine;
using TMPro;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

public class Puzzle4Logic : MonoBehaviour
{   
    [SerializeField, TextArea(6,12)] private string firstSupportText;    
    [SerializeField, TextArea(6,12)] private string secondSupportText;    
    [SerializeField, TextArea(6,12)] private string thirdSupportText; 

    public GameObject inputFieldFigure1;
    public GameObject inputFieldFigure2;
    public GameObject inputFieldFigure3;

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
            string solutionString1 = inputFieldFigure1.GetComponent<TMP_InputField>().text;
            string solutionString2 = inputFieldFigure2.GetComponent<TMP_InputField>().text;
            string solutionString3 = inputFieldFigure3.GetComponent<TMP_InputField>().text;

            solutionString1 = solutionString1.Replace(" ", "");
            solutionString1 = solutionString1.ToUpper();
            solutionString1 = RemovePunctuation(solutionString1); 
            solutionString1 = RemoveAccents(solutionString1);

            solutionString2 = solutionString2.Replace(" ", "");
            solutionString2 = solutionString2.ToUpper();
            solutionString2 = RemovePunctuation(solutionString2); 
            solutionString2 = RemoveAccents(solutionString2);

            solutionString3 = solutionString3.Replace(" ", "");
            solutionString3 = solutionString3.ToUpper();
            solutionString3 = RemovePunctuation(solutionString3); 
            solutionString3 = RemoveAccents(solutionString3);

            if(CheckFirstFigure(solutionString1))
            {
                inputFieldFigure1.GetComponent<TMP_InputField>().text = "A2:B2:C1:C2:C3";
            }
            else
            {
                inputFieldFigure1.GetComponent<TMP_InputField>().text = "";
            }

            if(CheckSecondFigure(solutionString2))
            {
                inputFieldFigure2.GetComponent<TMP_InputField>().text = "A1:A2:A3:B2:C1:C2:C3";
            }
            else
            {
                inputFieldFigure2.GetComponent<TMP_InputField>().text = "";
            }

            if(CheckThirdFigure(solutionString3))
            {
                inputFieldFigure3.GetComponent<TMP_InputField>().text = "A1:A3";
            }
            else
            {
                inputFieldFigure3.GetComponent<TMP_InputField>().text = "";
            }

            GetComponent<PuzzleUIManager>().isNecesaryResetInputs = false;
        }
    }

    public void CheckResult()
    {
        string solutionString1 = inputFieldFigure1.GetComponent<TMP_InputField>().text;
        string solutionString2 = inputFieldFigure2.GetComponent<TMP_InputField>().text;
        string solutionString3 = inputFieldFigure3.GetComponent<TMP_InputField>().text;

        solutionString1 = solutionString1.Replace(" ", "");
        solutionString1 = solutionString1.ToUpper();
        solutionString1 = RemovePunctuation(solutionString1); 
        solutionString1 = RemoveAccents(solutionString1);

        solutionString2 = solutionString2.Replace(" ", "");
        solutionString2 = solutionString2.ToUpper();
        solutionString2 = RemovePunctuation(solutionString2); 
        solutionString2 = RemoveAccents(solutionString2);

        solutionString3 = solutionString3.Replace(" ", "");
        solutionString3 = solutionString3.ToUpper();
        solutionString3 = RemovePunctuation(solutionString3); 
        solutionString3 = RemoveAccents(solutionString3);

        if(solutionString1 == "" && solutionString2 == "" && solutionString3 == "")
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 0;
        }
        else if(CheckFirstFigure(solutionString1) && CheckSecondFigure(solutionString2) && CheckThirdFigure(solutionString3))
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

    private bool CheckFirstFigure(string figureString)
    {
        //A2:B2:C1:C2:C3
        bool isCorrect = true;

        if(figureString != "")
        {
            string[] words = figureString.Split(':');
            foreach(string word in words)
            {
                if(word != "" && word != "A2" && word != "B2" && word != "C1" && word != "C2" && word != "C3")
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

    private bool CheckSecondFigure(string figureString)
    {
        //A1:A2:A3:B2:C1:C2:C3
        bool isCorrect = true;

        if(figureString != "")
        {
            string[] words = figureString.Split(':');
            foreach(string word in words)
            {
                if(word != "" && word != "A1" && word != "A2" && word != "A3" && word != "B2" && word != "C1" && word != "C2" 
                    && word != "C3")
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

    private bool CheckThirdFigure(string figureString)
    {
        //A1:A3
        bool isCorrect = true;

        if(figureString != "")
        {
            string[] words = figureString.Split(':');
            foreach(string word in words)
            {
                if(word != "" && word != "A1" && word != "A3")
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
