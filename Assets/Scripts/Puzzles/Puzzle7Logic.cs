using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;

public class Puzzle7Logic : MonoBehaviour, IPuzzleLogic
{       
    [Header("UI Objects Section")]
    [SerializeField] private GameObject cardImagePosition1; 
    [SerializeField] private GameObject cardImagePosition2; 
    [SerializeField] private GameObject cardImagePosition3; 
    [SerializeField] private GameObject cardImagePosition4; 
    [SerializeField] private GameObject cardImagePosition5;
    [SerializeField] private GameObject textBackgroundImage1; 
    [SerializeField] private GameObject textBackgroundImage2; 
    [SerializeField] private GameObject textBackgroundImage3; 
    [SerializeField] private GameObject textBackgroundImage4; 
    [SerializeField] private GameObject textBackgroundImage5;
    [SerializeField] private TMP_Dropdown positionDropdown1;
    [SerializeField] private TMP_Dropdown positionDropdown2;
    [SerializeField] private TMP_Dropdown positionDropdown3;
    [SerializeField] private TMP_Dropdown positionDropdown4;
    [SerializeField] private TMP_Dropdown positionDropdown5;   

    [Header("Sprites Section")]  
    [SerializeField] private Sprite cardSpriteSpades;     
    [SerializeField] private Sprite cardSpriteClubs;     
    [SerializeField] private Sprite cardSpriteDiamonds;     
    [SerializeField] private Sprite cardSpriteHearts;     
    [SerializeField] private Sprite cardSpriteJoker; 

    private GameObject[] cardImagePositions;
    private GameObject[] textBackgroundImages;
    private int selectedIndex1;
    private int selectedIndex2;
    private int selectedIndex3;
    private int selectedIndex4;
    private int selectedIndex5;


    void Start()
    {
        GetComponent<PuzzleUIManager>().FirstSupportText = GameStateManager.Instance.gameText.puzzle7.firstSupportText;
        GetComponent<PuzzleUIManager>().SecondSupportText = GameStateManager.Instance.gameText.puzzle7.secondSupportText;
        GetComponent<PuzzleUIManager>().ThirdSupportText = GameStateManager.Instance.gameText.puzzle7.thirdSupportText;

        GetComponent<PuzzleLogicManager>().ShowStatement(GameStateManager.Instance.gameText.puzzle7.puzzleStatementText);

        cardImagePositions = new GameObject[]
        {
            cardImagePosition1,
            cardImagePosition2,
            cardImagePosition3,
            cardImagePosition4,
            cardImagePosition5
        };

        textBackgroundImages = new GameObject[]
        {
            textBackgroundImage1,
            textBackgroundImage2,
            textBackgroundImage3,
            textBackgroundImage4,
            textBackgroundImage5
        };
    }

    // Método para limpiar los inputs de la solución en caso de fallo - Implementación de la interfaz
    public void ResetSolutionInputs()
    {
        positionDropdown1.value = selectedIndex1;
        positionDropdown2.value = selectedIndex2;
        positionDropdown3.value = selectedIndex3;
        positionDropdown4.value = selectedIndex4;
        positionDropdown5.value = selectedIndex5;
    }

    // Método para comprobar si el resultado proporcionado es acertado o no - Implementación de la interfaz
    public void CheckResult()
    {
        TMP_Dropdown[] dropdowns = { positionDropdown1, positionDropdown2, positionDropdown3, positionDropdown4, positionDropdown5 };
        string[] solutionStrings = new string[dropdowns.Length];

        for (int i = 0; i < dropdowns.Length; i++)
        {
            int selectedIndex = dropdowns[i].value;
            solutionStrings[i] = dropdowns[i].options[selectedIndex].text.ToUpper();
        }

        bool allEmpty = solutionStrings.All(s => s == "----");
        bool solution = solutionStrings[0] == "CORAZONES" && solutionStrings[1] == "JOKER" && solutionStrings[2] == "DIAMANTES" 
            && solutionStrings[3] == "TRÉBOLES" && solutionStrings[4] == "PICAS";

        if(solution)
        {
            GetComponent<PuzzleUIManager>().ShowSuccessPanel();
        }
        else if (!(solution || allEmpty))
        {
            GetComponent<PuzzleUIManager>().ShowFailurePanel();
        }
    }

    // Método auxiliar para mostrar la carta seleccionada
    public void DisplayCard(TMP_Dropdown dropdown)
    {
        int selectedIndex = dropdown.value;
        string selectedText = dropdown.options[selectedIndex].text.ToUpper();

        int posIndex = int.Parse(Regex.Match(dropdown.name, @"\d+").Value) - 1;

        GameObject cardImage = cardImagePositions[posIndex];
        GameObject textBackgroundImage = textBackgroundImages[posIndex];
        Sprite selectedSprite = null;

        switch (selectedText)
        {
            case "PICAS": selectedSprite = cardSpriteSpades; break;
            case "TRÉBOLES": selectedSprite = cardSpriteClubs; break;
            case "DIAMANTES": selectedSprite = cardSpriteDiamonds; break;
            case "CORAZONES": selectedSprite = cardSpriteHearts; break;
            case "JOKER": selectedSprite = cardSpriteJoker; break;
        }

        if (selectedSprite != null)
        {
            cardImage.SetActive(true);
            textBackgroundImage.SetActive(false);
            cardImage.GetComponent<Image>().sprite = selectedSprite;
        }
        else
        {
            cardImage.SetActive(false);
            textBackgroundImage.SetActive(true);
        }
    }
}