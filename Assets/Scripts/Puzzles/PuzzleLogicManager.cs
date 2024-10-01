using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleLogicManager : MonoBehaviour
{
    [SerializeField] private string puzzleScene;
    [SerializeField] private string puzzleName;
    [SerializeField] private TMP_Text puzzleStatement;    
    [SerializeField, TextArea(20, 40)] private string puzzleStatementText;
    [SerializeField]private float typingTime = 0.02f;
    [SerializeField]private GameObject spaceKeyStatement;    

    public bool[] puzzleSupports;
    public int puzzlePoints;

    private PuzzleData puzzleData;
    private bool isStatementComplete;

    void Start()
    {
        puzzleData = SaveManager.LoadPuzzleData();
        SelectPuzzleData(puzzleData);
        ShowStatement();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            string supportsContent = string.Join(", ", puzzleSupports);
            Debug.Log("Puzzle Supports: " + supportsContent);
            Debug.Log("Puzzle Points: " + puzzlePoints);
        }

        if((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isStatementComplete) 
        {
            StopAllCoroutines();
            puzzleStatement.text = puzzleStatementText;
            isStatementComplete = true;
            spaceKeyStatement.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Space) && GetComponent<PuzzleUIManager>().isSuccessPanelShown)
        {
            CompleteAndFinishPuzzle();
        }

        if(Input.GetKeyDown(KeyCode.Space) && GetComponent<PuzzleUIManager>().isFailurePanelShown)
        {
            GetComponent<PuzzleUIManager>().isReturnToPuzzleAfterFail = true;
            GetComponent<PuzzleUIManager>().isCorrectResult = 0;
        }

        if(GetComponent<PuzzleUIManager>().isPuzzleSkipped)
        {
            puzzlePoints = 1;
            GameLogicManager.Instance.endOpportunities = 1;
            CompleteAndFinishPuzzle();
        }
    }

    private void ShowStatement()
    {   
        isStatementComplete = false;     
        StartCoroutine(ShowLine());
    }

    private IEnumerator ShowLine() 
    {
        puzzleStatement.text = string.Empty;

        foreach(char ch in puzzleStatementText)
        {
            puzzleStatement.text += ch;
            yield return new WaitForSeconds(typingTime);
        }

        if(puzzleStatement.text == puzzleStatementText)
        {
            spaceKeyStatement.SetActive(false);
        }
    }

    public int CalculatePoints(bool isCorrect)
    {
        if(isCorrect)
        {
            if(puzzlePoints == 0)
            {
                puzzlePoints = 50;
            }
        }
        else
        {
            if(puzzlePoints == 0)
            {
                puzzlePoints = 50;
            }

            puzzlePoints -= 5;

            if(puzzlePoints <= 0)
            {
                puzzlePoints = 1;
            }
        }

        return puzzlePoints;
    }

    public void ReturnToGameScene()
    {
        SaveTemporarilyPuzzleData();
        GameStateManager.Instance.isPuzzleIncomplete = true;
        GameStateManager.Instance.actualPuzzleName = puzzleName;
        SceneManager.LoadScene(puzzleScene);
    }

    private void CompleteAndFinishPuzzle()
    {
        GameStateManager.Instance.lastPuzzleSupports = puzzleSupports;
        GameStateManager.Instance.lastPuzzlePoints = puzzlePoints;
        GameLogicManager.Instance.lastPuzzleComplete = puzzleName;
        GameStateManager.Instance.isPuzzleRecentlyCompleted = true;
        SavePuzzleData();
        SceneManager.LoadScene(puzzleScene);
    }

    private void SaveTemporarilyPuzzleData()
    {
        GameStateManager.Instance.lastPuzzleSupports = puzzleSupports;
        GameStateManager.Instance.lastPuzzlePoints = puzzlePoints;
    }

    private void SelectPuzzleData(PuzzleData puzzleData)
    {
        if(GameStateManager.Instance.lastPuzzleSupports != null && GameStateManager.Instance.lastPuzzleSupports.Length > 0)
        {
            puzzleSupports = GameStateManager.Instance.lastPuzzleSupports;
            puzzlePoints = GameStateManager.Instance.lastPuzzlePoints;
        }
        else if(puzzleName == "Puzzle1")
        {
            if(puzzleData != null)
            {
                if(puzzleData.gamePuzzle1Supports != null && puzzleData.gamePuzzle1Supports.Length > 0)
                {
                    puzzleSupports = puzzleData.gamePuzzle1Supports;
                }
                else
                {
                    createPuzzleSupports();
                }

                if(puzzleData.gamePuzzle1Points != 0)
                {
                    puzzlePoints = puzzleData.gamePuzzle1Points;
                }
                else
                {
                    puzzlePoints = 0;
                }
            }
            else
            {
                createPuzzleSupports();
                puzzlePoints = 0;
            }

            
        }
        else if (puzzleName == "Puzzle2")
        {
            if (puzzleData != null)
            {
                if (puzzleData.gamePuzzle2Supports != null && puzzleData.gamePuzzle2Supports.Length > 0)
                {
                    puzzleSupports = puzzleData.gamePuzzle2Supports;
                }
                else
                {
                    createPuzzleSupports();
                }

                if (puzzleData.gamePuzzle2Points != 0)
                {
                    puzzlePoints = puzzleData.gamePuzzle2Points;
                }
                else
                {
                    puzzlePoints = 0;
                }
            }
            else
            {
                createPuzzleSupports();
                puzzlePoints = 0;
            }
        }
        else if (puzzleName == "Puzzle3")
        {
            if (puzzleData != null)
            {
                if (puzzleData.gamePuzzle3Supports != null && puzzleData.gamePuzzle3Supports.Length > 0)
                {
                    puzzleSupports = puzzleData.gamePuzzle3Supports;
                }
                else
                {
                    createPuzzleSupports();
                }

                if (puzzleData.gamePuzzle3Points != 0)
                {
                    puzzlePoints = puzzleData.gamePuzzle3Points;
                }
                else
                {
                    puzzlePoints = 0;
                }
            }
            else
            {
                createPuzzleSupports();
                puzzlePoints = 0;
            }
        }
        else if (puzzleName == "Puzzle4")
        {
            if (puzzleData != null)
            {
                if (puzzleData.gamePuzzle4Supports != null && puzzleData.gamePuzzle4Supports.Length > 0)
                {
                    puzzleSupports = puzzleData.gamePuzzle4Supports;
                }
                else
                {
                    createPuzzleSupports();
                }

                if (puzzleData.gamePuzzle4Points != 0)
                {
                    puzzlePoints = puzzleData.gamePuzzle4Points;
                }
                else
                {
                    puzzlePoints = 0;
                }
            }
            else
            {
                createPuzzleSupports();
                puzzlePoints = 0;
            }
        }
        else if (puzzleName == "Puzzle5")
        {
            if (puzzleData != null)
            {
                if (puzzleData.gamePuzzle5Supports != null && puzzleData.gamePuzzle5Supports.Length > 0)
                {
                    puzzleSupports = puzzleData.gamePuzzle5Supports;
                }
                else
                {
                    createPuzzleSupports();
                }

                if (puzzleData.gamePuzzle5Points != 0)
                {
                    puzzlePoints = puzzleData.gamePuzzle5Points;
                }
                else
                {
                    puzzlePoints = 0;
                }
            }
            else
            {
                createPuzzleSupports();
                puzzlePoints = 0;
            }
        }
        else if (puzzleName == "Puzzle6")
        {
            if (puzzleData != null)
            {
                if (puzzleData.gamePuzzle6Supports != null && puzzleData.gamePuzzle6Supports.Length > 0)
                {
                    puzzleSupports = puzzleData.gamePuzzle6Supports;
                }
                else
                {
                    createPuzzleSupports();
                }

                if (puzzleData.gamePuzzle6Points != 0)
                {
                    puzzlePoints = puzzleData.gamePuzzle6Points;
                }
                else
                {
                    puzzlePoints = 0;
                }
            }
            else
            {
                createPuzzleSupports();
                puzzlePoints = 0;
            }
        }
        else if (puzzleName == "Puzzle7")
        {
            if (puzzleData != null)
            {
                if (puzzleData.gamePuzzle7Supports != null && puzzleData.gamePuzzle7Supports.Length > 0)
                {
                    puzzleSupports = puzzleData.gamePuzzle7Supports;
                }
                else
                {
                    createPuzzleSupports();
                }

                if (puzzleData.gamePuzzle7Points != 0)
                {
                    puzzlePoints = puzzleData.gamePuzzle7Points;
                }
                else
                {
                    puzzlePoints = 0;
                }
            }
            else
            {
                createPuzzleSupports();
                puzzlePoints = 0;
            }
        }
        else if (puzzleName == "Puzzle8")
        {
            if (puzzleData != null)
            {
                if (puzzleData.gamePuzzle8Supports != null && puzzleData.gamePuzzle8Supports.Length > 0)
                {
                    puzzleSupports = puzzleData.gamePuzzle8Supports;
                }
                else
                {
                    createPuzzleSupports();
                }

                if (puzzleData.gamePuzzle8Points != 0)
                {
                    puzzlePoints = puzzleData.gamePuzzle8Points;
                }
                else
                {
                    puzzlePoints = 0;
                }
            }
        }
        else
        {
            Debug.Log("No se han cargado correctamente los datos de " + puzzleName);
        }
    }

    private void createPuzzleSupports()
    {
        puzzleSupports = new bool[3];
        for (int i = 0; i < puzzleSupports.Length; i++)
        {
            puzzleSupports[i] = false;
        }
    }

    private void SavePuzzleData()
    {
        if(puzzleName == "Puzzle1")
        {
            SaveManager.SavePuzzleData(puzzleSupports, puzzlePoints, null, 0, null, 0, null, 0, null, 0, null, 0, null, 0, null, 0);
            Debug.Log("Datos de puzles guardados");
        }
        else if(puzzleName == "Puzzle2")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleSupports, puzzlePoints, 
                null, 0, null, 0, null, 0, null, 0, null, 0, null, 0);
            Debug.Log("Datos de puzles guardados");
        }
        else if(puzzleName == "Puzzle3")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleSupports, puzzlePoints, null, 0, null, 0, null, 0, null, 0, null, 0);
            Debug.Log("Datos de puzles guardados");
        }
        else if(puzzleName == "Puzzle4")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleData.gamePuzzle3Supports, puzzleData.gamePuzzle3Points, 
                puzzleSupports, puzzlePoints, null, 0, null, 0, null, 0, null, 0);
            Debug.Log("Datos de puzles guardados");
        }
        else if(puzzleName == "Puzzle5")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleData.gamePuzzle3Supports, puzzleData.gamePuzzle3Points, 
                puzzleData.gamePuzzle4Supports, puzzleData.gamePuzzle4Points, puzzleSupports, puzzlePoints, null, 0, null, 0, null, 0);
            Debug.Log("Datos de puzles guardados");
        }
        else if(puzzleName == "Puzzle6")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleData.gamePuzzle3Supports, puzzleData.gamePuzzle3Points, 
                puzzleData.gamePuzzle4Supports, puzzleData.gamePuzzle4Points, puzzleData.gamePuzzle5Supports, 
                puzzleData.gamePuzzle5Points, puzzleSupports, puzzlePoints, null, 0, null, 0);
            Debug.Log("Datos de puzles guardados");
        }
        else if(puzzleName == "Puzzle7")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleData.gamePuzzle3Supports, puzzleData.gamePuzzle3Points, 
                puzzleData.gamePuzzle4Supports, puzzleData.gamePuzzle4Points, puzzleData.gamePuzzle5Supports, 
                puzzleData.gamePuzzle5Points, puzzleData.gamePuzzle6Supports, puzzleData.gamePuzzle6Points, 
                puzzleSupports, puzzlePoints, null, 0);
            Debug.Log("Datos de puzles guardados");
        }
        else if(puzzleName == "Puzzle8")
        {
            SaveManager.SavePuzzleData(puzzleData.gamePuzzle1Supports, puzzleData.gamePuzzle1Points, puzzleData.gamePuzzle2Supports, 
                puzzleData.gamePuzzle2Points, puzzleData.gamePuzzle3Supports, puzzleData.gamePuzzle3Points, 
                puzzleData.gamePuzzle4Supports, puzzleData.gamePuzzle4Points, puzzleData.gamePuzzle5Supports, 
                puzzleData.gamePuzzle5Points, puzzleData.gamePuzzle6Supports, puzzleData.gamePuzzle6Points, 
                puzzleData.gamePuzzle7Supports, puzzleData.gamePuzzle7Points, puzzleSupports, puzzlePoints);
            Debug.Log("Datos de puzles guardados");
        }
        else
        {
            Debug.Log("No se han guardado correctamente los datos de " + puzzleName);
        }
    }
}
