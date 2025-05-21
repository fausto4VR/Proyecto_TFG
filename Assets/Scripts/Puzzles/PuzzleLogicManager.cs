using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PuzzleLogicManager : MonoBehaviour
{
    [Header("Statement Section")]
    [SerializeField] private TMP_Text puzzleStatement;  
    [SerializeField]private GameObject spaceKeyStatement;

    [Header("Variable Section")]
    [SerializeField] private string sceneToReturn;
    [SerializeField] private string puzzleName;
    [SerializeField]private float typingTime = 0.03f;
    [SerializeField]private int charsToPlaySound = 5;
    [SerializeField] private  int maxPunctuation = 50; 
    [SerializeField] private  int failurePunctuationDeduction = 5;
    
    private string puzzleStatementText;
    private Coroutine typingCoroutine;
    private bool[] puzzleSupports; 
    private int puzzlePoints;

    // REVISAR AUDIO
    private AudioSource typingAudioSource;
    

    void Start()
    {
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        typingAudioSource = audioSources[0];

        SelectPuzzleData();
    }

    // Método para consultar la lista de ayudas
    public bool[] PuzzleSupports
    {
        get { return (bool[])puzzleSupports.Clone(); }
    }

    // Método para cambiar un valor de la lista de ayudas
    public void SetPuzzleSupportsByIndex(int position, bool value)
    {
        puzzleSupports[position] = value;
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
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0));

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

    // Método para omitir el puzle
    public void SkipPuzzleLogic()
    {
        puzzlePoints = 1;
        if (GameLogicManager.Instance.EndOpportunities > 0) GameLogicManager.Instance.EndOpportunities = 1;
        CompleteAndFinishPuzzle();
    }

    // Método para actualizar los puntos obtenidos en un puzle
    public int UpdatePoints(bool isCorrect)
    {
        // Si los puntos valen 0 es que aún no han sido actualizados con el valor máximo
        if(puzzlePoints == 0)
        {
            puzzlePoints = maxPunctuation;
        }

        if(!isCorrect)
        {
            puzzlePoints -= failurePunctuationDeduction;

            if(puzzlePoints <= 0)
            {
                puzzlePoints = 1;
            }
        }

        return puzzlePoints;
    }

    // Método para volver al mundo con el puzle incompleto
    public void ReturnToGameScene()
    {
        UpdatePuzzleData(false);
        GameLogicManager.Instance.IsPuzzleIncomplete = true;
        SceneManager.LoadScene(sceneToReturn);
    }

    // Método para dar por finalizado el puzle y volver al mundo después de que el jugador haya acertado la solución
    private void CompleteAndFinishPuzzle()
    {
        UpdatePuzzleData(true);
        GameLogicManager.Instance.LastPuzzleComplete = puzzleName;
        GameLogicManager.Instance.IsPuzzleCompleted = true;
        SceneManager.LoadScene(sceneToReturn);
    }

    // Método para actualizar los datos del puzle 
    private void UpdatePuzzleData(bool isPuzzleComplete)
    {
        List<PuzzleState> puzzleStateList = GameLogicManager.Instance.PuzzleStateList;
        PuzzleState foundPuzzle = puzzleStateList.FirstOrDefault(p => p.gamePuzzleName == puzzleName);

        if (foundPuzzle != null)
        {
            foundPuzzle.gamePuzzleSupports = puzzleSupports;
            foundPuzzle.gamePuzzlePoints = puzzlePoints;
            foundPuzzle.gameIsPuzzleComplete = isPuzzleComplete;
        }
        else
        {
            PuzzleState newPuzzle = new PuzzleState(puzzleName, puzzleSupports, puzzlePoints, isPuzzleComplete);
            puzzleStateList.Add(newPuzzle);
        }

        GameLogicManager.Instance.PuzzleStateList = puzzleStateList;
    }

    // Método para cargar los puntos obtenidos y las ayudas disponibles
    private void SelectPuzzleData()
    {
        List<PuzzleState> puzzleStateList = GameLogicManager.Instance.PuzzleStateList;
        PuzzleState foundPuzzle = puzzleStateList.FirstOrDefault(p => p.gamePuzzleName == puzzleName);

        if (foundPuzzle != null)
        {
            puzzleSupports = (bool[])foundPuzzle.gamePuzzleSupports.Clone();
            puzzlePoints = foundPuzzle.gamePuzzlePoints;
        }
        else
        {
            puzzleSupports = new bool[3];
            puzzlePoints = 0;
        }
    }
}
