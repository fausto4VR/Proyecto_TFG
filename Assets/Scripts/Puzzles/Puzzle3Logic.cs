using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Puzzle3Logic : MonoBehaviour
{   
    [SerializeField, TextArea(6,12)] private string firstSupportText;    
    [SerializeField, TextArea(6,12)] private string secondSupportText;    
    [SerializeField, TextArea(6,12)] private string thirdSupportText; 

    private List<string> activePaths = new List<string>();

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
            // Los caminos se quedan igual
            GetComponent<PuzzleUIManager>().isNecesaryResetInputs = false;
        }
    }

    public void CheckResult()
    {
        // ADKJCGBFNHIONEMPQRLS

        if (activePaths.Count == 0)
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 0;
        }
        else if (CheckSolution())
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 1;
        }
        else
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 2;
        }
    }

    public void DisplayPath(Toggle toggle)
    {
        if (toggle.isOn)
        {
            if (!activePaths.Contains(toggle.name))
            {
                activePaths.Add(toggle.name);
            }
        }
        else
        {
            if (activePaths.Contains(toggle.name))
            {
                activePaths.Remove(toggle.name);
            }
        }
    }

    private bool CheckSolution()
    {
        // Camino: ADKJCGBFÑHIONEMPQRLS
        List<string> requiredPath = new List<string> {"AD", "DK", "JK", "CJ", "CG", "BG", "BF", "FÑ", "HÑ", "HI", "IO", "NO", 
            "EN", "EM", "MP", "PQ", "QR", "LR", "LS"};

        if(activePaths.Count == requiredPath.Count)
        {
            foreach (string path in activePaths)
            {
                bool found = false;

                foreach (string required in requiredPath)
                {
                    if (path.ToUpper().Contains(required.ToUpper()))
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
        else
        {
            return false;
        }
    }
}
