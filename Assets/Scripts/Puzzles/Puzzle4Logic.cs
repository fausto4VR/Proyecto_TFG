using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Puzzle4Logic : MonoBehaviour
{   
    [SerializeField, TextArea(6,12)] private string firstSupportText;    
    [SerializeField, TextArea(6,12)] private string secondSupportText;    
    [SerializeField, TextArea(6,12)] private string thirdSupportText;    
    [SerializeField] private TMP_Text explanationFailureText;

    private List<string> activeSquares = new List<string>();
    private bool isFailurePanelShown;

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

        if(isFailurePanelShown)
        {
            // Se da un aviso al jugador de las figuras que ha acertado

            if(CheckSolutionFigure1() && CheckSolutionFigure2())
            {
                explanationFailureText.text = "Aún así has acertado la Figura 1 y la 2";
            }
            else if(CheckSolutionFigure1() && CheckSolutionFigure3())
            {
                explanationFailureText.text = "Aún así has acertado la Figura 1 y la 3";
            }
            else if(CheckSolutionFigure2() && CheckSolutionFigure3())
            {
                explanationFailureText.text = "Aún así has acertado la Figura 2 y la 3";
            }
            else if(CheckSolutionFigure1())
            {
                explanationFailureText.text = "Aún así has acertado la Figura 1";
            }
            else if(CheckSolutionFigure2())
            {
                explanationFailureText.text = "Aún así has acertado la Figura 2";
            }
            else if(CheckSolutionFigure3())
            {
                explanationFailureText.text = "Aún así has acertado la Figura 3";
            }

            isFailurePanelShown = false;
        }

        if(GetComponent<PuzzleUIManager>().isNecesaryResetInputs)
        {
            // Los cuadrados se quedan igual
            GetComponent<PuzzleUIManager>().isNecesaryResetInputs = false;
        }
    }

    public void CheckResult()
    {
        if(activeSquares.Count == 0)
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = ResultType.Empty;
        }
        else if(CheckSolution())
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = ResultType.Success;
        }
        else
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = ResultType.Failure;
            isFailurePanelShown = true;
        }
    }

    public void DisplaySquare(Toggle toggle)
    {
        if (toggle.isOn)
        {
            if (!activeSquares.Contains(toggle.name))
            {
                activeSquares.Add(toggle.name);
            }
        }
        else
        {
            if (activeSquares.Contains(toggle.name))
            {
                activeSquares.Remove(toggle.name);
            }
        }
    }

    private bool CheckSolution()
    {
        if(CheckSolutionFigure1() && CheckSolutionFigure2() && CheckSolutionFigure3())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckSolutionFigure1()
    {
        //Figura 1 - A2:B2:C1:C2:C3
        List<string> requiredSquares = new List<string> {"Fig 1 A2", "Fig 1 B2", "Fig 1 C1", "Fig 1 C2", "Fig 1 C3"};
        List<string> activeSquaresFigure1 = FilterByWord(activeSquares, "Fig 1");

        if(activeSquaresFigure1.Count == requiredSquares.Count)
        {
            foreach (string square in activeSquaresFigure1)
            {
                bool found = false;

                foreach (string required in requiredSquares)
                {
                    if (square.ToUpper().Contains(required.ToUpper()))
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

    private bool CheckSolutionFigure2()
    {
        //Figura 2 - A1:A2:A3:B2:C1:C2:C3
        List<string> requiredSquares = new List<string> {"Fig 2 A1", "Fig 2 A2", "Fig 2 A3", "Fig 2 B2", "Fig 2 C1", "Fig 2 C2", 
            "Fig 2 C3"};
        List<string> activeSquaresFigure2 = FilterByWord(activeSquares, "Fig 2");

        if(activeSquaresFigure2.Count == requiredSquares.Count)
        {
            foreach (string square in activeSquaresFigure2)
            {
                bool found = false;

                foreach (string required in requiredSquares)
                {
                    if (square.ToUpper().Contains(required.ToUpper()))
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

    private bool CheckSolutionFigure3()
    {
        //Figura 3 - A1:A3
        List<string> requiredSquares = new List<string> {"Fig 3 A1", "Fig 3 A3"};
        List<string> activeSquaresFigure3 = FilterByWord(activeSquares, "Fig 3");

        if(activeSquaresFigure3.Count == requiredSquares.Count)
        {
            foreach (string square in activeSquaresFigure3)
            {
                bool found = false;

                foreach (string required in requiredSquares)
                {
                    if (square.ToUpper().Contains(required.ToUpper()))
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

    public List<string> FilterByWord(List<string> originalList, string word)
    {
        List<string> filteredList = originalList.FindAll(item => item.Contains(word));
        return filteredList;
    }
}
