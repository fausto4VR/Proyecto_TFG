using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Enum de los tipos de manecillas de un reloj
public enum ClockHandType
{
    None, Hour, Minute, Second
}

public class Puzzle2Logic : MonoBehaviour, IPuzzleLogic
{
    [Header("UI Objects Section")]
    [SerializeField] private GameObject hourHand;
    [SerializeField] private GameObject minuteHand;
    [SerializeField] private GameObject secondHand;

    [Header("Colors Toogle Section")]
    [SerializeField] private Color hourToggleColorActive = new Color32(203, 161, 168, 255);   // #CBA1A8
    [SerializeField] private Color hourToggleColorInactive = new Color32(255, 225, 229, 255); // #FFE1E5
    [SerializeField] private Color minuteToggleColorActive = new Color32(203, 183, 161, 255);   // #CBB7A1
    [SerializeField] private Color minuteToggleColorInactive = new Color32(255, 235, 213, 255); // #FFEBD5
    [SerializeField] private Color secondToggleColorActive = new Color32(202, 207, 165, 255);   // #CACFA5
    [SerializeField] private Color secondToggleColorInactive = new Color32(252, 255, 225, 255); // #FCFFE1

    private Toggle currentActiveToggle;
    private ClockHandType activeHand = ClockHandType.None;
    private Dictionary<ClockHandType, int> handPositions = new Dictionary<ClockHandType, int>()
    {
        { ClockHandType.Hour, 0 },
        { ClockHandType.Minute, 0 },
        { ClockHandType.Second, 0 }
    };


    void Start()
    {
        GetComponent<PuzzleUIManager>().FirstSupportText = GameStateManager.Instance.gameText.puzzle2.firstSupportText;
        GetComponent<PuzzleUIManager>().SecondSupportText = GameStateManager.Instance.gameText.puzzle2.secondSupportText;
        GetComponent<PuzzleUIManager>().ThirdSupportText = GameStateManager.Instance.gameText.puzzle2.thirdSupportText;

        GetComponent<PuzzleLogicManager>().ShowStatement(GameStateManager.Instance.gameText.puzzle2.puzzleStatementText);
    }

    // Método para limpiar los inputs de la solución en caso de fallo - Implementación de la interfaz
    public void ResetSolutionInputs()
    {
        // Las manecillas del reloj se quedan igual, por lo que no hace falta reiniciar nada
    }

    // Método para comprobar si el resultado proporcionado es acertado o no - Implementación de la interfaz
    public void CheckResult()
    {
        if (CheckSolution())
        {
            GetComponent<PuzzleUIManager>().ShowSuccessPanel();
        }
        else if (!(CheckSolution() || (handPositions[ClockHandType.Hour] == 0 && handPositions[ClockHandType.Minute] == 0 &&
            handPositions[ClockHandType.Second] == 0)))
        {
            GetComponent<PuzzleUIManager>().ShowFailurePanel();
        }
    }

    // Método auxiliar para poner la manecilla correspondiente en su lugar
    public void DisplayClockHand(int clockNumber)
    {
        float angle = clockNumber * 30f;

        // Se guarda el número al que apunta
        handPositions[activeHand] = clockNumber;

        switch (activeHand)
        {
            case ClockHandType.Hour:
                Vector3 hourRotation = hourHand.transform.eulerAngles;
                hourRotation.z = -angle;
                hourHand.transform.eulerAngles = hourRotation;
                break;

            case ClockHandType.Minute:
                Vector3 minuteRotation = minuteHand.transform.eulerAngles;
                minuteRotation.z = -angle;
                minuteHand.transform.eulerAngles = minuteRotation;
                break;

            case ClockHandType.Second:
                Vector3 secondRotation = secondHand.transform.eulerAngles;
                secondRotation.z = -angle;
                secondHand.transform.eulerAngles = secondRotation;
                break;
        }
    }

    // Método auxiliar para activar el toggle de la manecilla del reloj que se quiere cambiar
    public void ActiveClockHandsToogle(Toggle toggle)
    {
        if (toggle == currentActiveToggle)
        {
            ResetToggleColor(toggle);
            toggle.isOn = false;
            currentActiveToggle = null;
            activeHand = ClockHandType.None;
        }
        else
        {
            if (currentActiveToggle != null)
            {
                ResetToggleColor(currentActiveToggle);
                currentActiveToggle.isOn = false;
            }

            toggle.isOn = true;
            currentActiveToggle = toggle;

            string toggleName = toggle.name.ToLower();

            if (toggleName.Contains("hour"))
            {
                activeHand = ClockHandType.Hour;
                SetToggleColor(toggle, hourToggleColorActive, hourToggleColorInactive);
            }
            else if (toggleName.Contains("minute"))
            {
                activeHand = ClockHandType.Minute;
                SetToggleColor(toggle, minuteToggleColorActive, minuteToggleColorInactive);
            }
            else if (toggleName.Contains("second"))
            {
                activeHand = ClockHandType.Second;
                SetToggleColor(toggle, secondToggleColorActive, secondToggleColorInactive);
            }
            else
            {
                activeHand = ClockHandType.None;
            }
        }
    }    

    // Método auxiliar para cambiar el color de un toggle por otro    
    private void SetToggleColor(Toggle toggle, Color visibleColor, Color transitionColor)
    {
        ColorBlock cb = toggle.colors;

        cb.selectedColor = visibleColor;
        cb.normalColor = visibleColor;
        cb.highlightedColor = visibleColor;

        cb.pressedColor = transitionColor;
        cb.disabledColor = transitionColor;

        toggle.colors = cb;
    }

    // Método auxiliar para cambiar el color de un toggle por su color original
    private void ResetToggleColor(Toggle toggle)
    {
        string toggleName = toggle.name.ToLower();
        Color originalVisibleColor = Color.white;
        Color originalTransitionColor = Color.white;

        if (toggleName.Contains("hour"))
        {
            originalVisibleColor = hourToggleColorInactive;
            originalTransitionColor = hourToggleColorActive;
        }
        else if (toggleName.Contains("minute"))
        {
            originalVisibleColor = minuteToggleColorInactive;
            originalTransitionColor = minuteToggleColorActive;
        }
        else if (toggleName.Contains("second"))
        {
            originalVisibleColor = secondToggleColorInactive;
            originalTransitionColor = secondToggleColorActive;
        }

        ColorBlock cb = toggle.colors;

        cb.selectedColor = originalVisibleColor;
        cb.normalColor = originalVisibleColor;
        cb.highlightedColor = originalVisibleColor;

        cb.pressedColor = originalTransitionColor;
        cb.disabledColor = originalTransitionColor;

        toggle.colors = cb;
    }

    // Método que implementa la lógica para comprobar si la solución dada es correcta o no
    private bool CheckSolution()
    {
        //12:25:10 que es apuntar a la hora 12, a los minutos 5 y a los segundos 2 
        bool result = (handPositions[ClockHandType.Hour] == 12 || handPositions[ClockHandType.Hour] == 0) && handPositions[ClockHandType.Minute] == 5 &&
            handPositions[ClockHandType.Second] == 2;

        return result;
    }
}
