using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleLogicManager : MonoBehaviour
{
    [Header("Statement Section")]
    [SerializeField] private TMP_Text puzzleStatement;  
    [SerializeField]private GameObject spaceKeyStatement;

    [Header("Sound Section")]
    [SerializeField] private GameObject audioSourcesManager;   

    [Header("Variable Section")]
    [SerializeField] private string puzzleScene;
    [SerializeField] private string puzzleName;
    [SerializeField]private float typingTime = 0.03f;
    [SerializeField]private int charsToPlaySound = 5; 

    // QUITAR
    [Header("Otros")]
    public bool[] puzzleSupports;
    public int puzzlePoints;

    // REVISAR
    private string puzzleStatementText;
    private Coroutine typingCoroutine;
    private PuzzleData puzzleData;
    private AudioSource typingAudioSource;

    // REVISAR ---------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        typingAudioSource = audioSources[0];

        puzzleData = SaveManager.LoadPuzzleData();
        SelectPuzzleData(puzzleData);
    }

    // QUITAR
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            string supportsContent = string.Join(", ", puzzleSupports);
            Debug.Log("Puzzle Supports: " + supportsContent);
            Debug.Log("Puzzle Points: " + puzzlePoints);
        }
    }

    // Método para inicializar la corrutina donde se cierra el panel de solución
    public void DetectToClosePanel(bool isSuccessResult)
    {        
        StartCoroutine(WaitForInputToClosePanel(isSuccessResult));
    }

    // Corrutina para cerrar el panel de solución, ya sea de fallo o acierto
    private IEnumerator WaitForInputToClosePanel(bool isSuccessResult)
    {
        // Esperar un frame para que no salte directamente 
        yield return null; 
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0));

        if (isSuccessResult)
        {
            CompleteAndFinishPuzzle();
        }
        else if (!isSuccessResult)
        {
            GetComponent<PuzzleUIManager>().ReturnToPuzzleAfterFail();
        }
    }

    // Método para mostrar el enunciado del puzle
    public void ShowStatement(string statementTextInput)
    {   
        puzzleStatementText = statementTextInput;   
        typingCoroutine = StartCoroutine(ShowLine(statementTextInput));
    }

    // Corrutina para mostrar el texto progresivamente
    private IEnumerator ShowLine(string statementTextInput) 
    {
        puzzleStatement.text = string.Empty;
        int charIndex = 0;
        bool skipActivated = false;

        foreach (char ch in statementTextInput)
        {
            puzzleStatement.text += ch;
            
            if (charIndex % charsToPlaySound == 0)
            {
                typingAudioSource.Play();
            }

            charIndex++;
            yield return new WaitForSeconds(typingTime);

            // Los dos siguientes if es para activar el salto del texto cuando se deje de pulsar la tecla correspondiente
            if(Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            {
                skipActivated = true;
            }

            if(!(Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && skipActivated)
            {
                Debug.Log("Enunciado del puzle mostrado directamente");
                SkipText();
                break;
            }
        }

        GetComponent<PuzzleUIManager>().OutStatementDisplay();
        spaceKeyStatement.SetActive(false);
    }

    // Método para saltar el tipado del texto y mostrarlo completo
    private void SkipText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            puzzleStatement.text = puzzleStatementText;
            spaceKeyStatement.SetActive(false);
            GetComponent<PuzzleUIManager>().OutStatementDisplay();
        }
    }

    // ------------------------------------------------------------------------------------------------------------------------
    // Método para omitir el puzle
    public void SkipPuzzleLogic()
    {
        puzzlePoints = 1;
        GameLogicManager.Instance.endOpportunities = 1;
        CompleteAndFinishPuzzle();
    }

    // REVISAR ---------------------------------------------------------------------------------------------------------------------
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

    // REVISAR ---------------------------------------------------------------------------------------------------------------------
    public void ReturnToGameScene()
    {
        SaveTemporarilyPuzzleData();
        GameStateManager.Instance.isPuzzleIncomplete = true;
        GameStateManager.Instance.actualPuzzleName = puzzleName;
        SceneManager.LoadScene(puzzleScene);
    }

    // REVISAR ---------------------------------------------------------------------------------------------------------------------
    private void CompleteAndFinishPuzzle()
    {
        GameStateManager.Instance.lastPuzzleSupports = puzzleSupports;
        GameStateManager.Instance.lastPuzzlePoints = puzzlePoints;
        GameLogicManager.Instance.lastPuzzleComplete = puzzleName;
        GameStateManager.Instance.isPuzzleRecentlyCompleted = true;
        SavePuzzleData();
        SceneManager.LoadScene(puzzleScene);
    }

    // REVISAR ---------------------------------------------------------------------------------------------------------------------
    private void SaveTemporarilyPuzzleData()
    {
        GameStateManager.Instance.lastPuzzleSupports = puzzleSupports;
        GameStateManager.Instance.lastPuzzlePoints = puzzlePoints;
    }

    // REVISAR ---------------------------------------------------------------------------------------------------------------------
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

    // REVISAR ---------------------------------------------------------------------------------------------------------------------
    private void createPuzzleSupports()
    {
        puzzleSupports = new bool[3];
        for (int i = 0; i < puzzleSupports.Length; i++)
        {
            puzzleSupports[i] = false;
        }
    }

    // REVISAR ---------------------------------------------------------------------------------------------------------------------
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
