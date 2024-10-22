using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Puzzle5Logic : MonoBehaviour
{   
    [SerializeField, TextArea(6,12)] private string firstSupportText;    
    [SerializeField, TextArea(6,12)] private string secondSupportText;    
    [SerializeField, TextArea(6,12)] private string thirdSupportText;

    private Image buttonImage;
    private int totalQueensPlaced;
    private List<string> activeQueens = new List<string>();

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
            // Las reinas se quedan igual
            GetComponent<PuzzleUIManager>().isNecesaryResetInputs = false;
        }
    }

    public void CheckResult()
    {
        if(activeQueens.Count == 0)
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 0;
        }
        else if(CheckSolution())
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 1;
        }
        else
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 2;
        }
    }

    public void DisplayQueen(Button button)
    {
        buttonImage = button.GetComponent<Image>();

        if (buttonImage == null) return;

        // Si la imagen es visible
        if (buttonImage.color.a > 0)
        {
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0);
            totalQueensPlaced -= 1;
            activeQueens.Remove(button.name);
        }
        // Si la imagen no es visible
        else
        {
            if(totalQueensPlaced < 5)
            {
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 1);
                totalQueensPlaced += 1;
                if (!activeQueens.Contains(button.name))
                {
                    activeQueens.Add(button.name);
                }
            }
        }
    }

    private bool CheckSolution()
    {
        //A6:B8:C2:E1:H3 y D4:F7:G5
        List<string> requiredQueens = new List<string> {"A6", "B8", "C2", "E1", "H3"};

        foreach (string required in requiredQueens)
        {
            bool found = false;

            foreach (string queen in activeQueens)
            {
                if (queen.ToUpper().Contains(required))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
        }

        return true;
    }
}
