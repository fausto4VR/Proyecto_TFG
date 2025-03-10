using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Puzzle7Logic : MonoBehaviour
{   
    [SerializeField, TextArea(6,12)] private string firstSupportText;    
    [SerializeField, TextArea(6,12)] private string secondSupportText;    
    [SerializeField, TextArea(6,12)] private string thirdSupportText;     
    [SerializeField] private GameObject cardImagePosition1; 
    [SerializeField] private GameObject cardImagePosition2; 
    [SerializeField] private GameObject cardImagePosition3; 
    [SerializeField] private GameObject cardImagePosition4; 
    [SerializeField] private GameObject cardImagePosition5;     
    [SerializeField] private Sprite cardSpriteSpades;     
    [SerializeField] private Sprite cardSpriteClubs;     
    [SerializeField] private Sprite cardSpriteDiamonds;     
    [SerializeField] private Sprite cardSpriteHearts;     
    [SerializeField] private Sprite cardSpriteJoker; 

    public TMP_Dropdown positionDropdown1;
    public TMP_Dropdown positionDropdown2;
    public TMP_Dropdown positionDropdown3;
    public TMP_Dropdown positionDropdown4;
    public TMP_Dropdown positionDropdown5;

    private int selectedIndex1;
    private int selectedIndex2;
    private int selectedIndex3;
    private int selectedIndex4;
    private int selectedIndex5;

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

        if(GetComponent<PuzzleUIManager>().isNecesaryResetInputs)
        {
            positionDropdown1.value = selectedIndex1;
            positionDropdown2.value = selectedIndex2;
            positionDropdown3.value = selectedIndex3;
            positionDropdown4.value = selectedIndex4;
            positionDropdown5.value = selectedIndex5;
            GetComponent<PuzzleUIManager>().isNecesaryResetInputs = false;
        }
    }

    public void CheckResult()
    {
        selectedIndex1 = positionDropdown1.value;
        string solutionString1 = positionDropdown1.options[selectedIndex1].text.ToUpper();

        selectedIndex2 = positionDropdown2.value;
        string solutionString2 = positionDropdown2.options[selectedIndex2].text.ToUpper();

        selectedIndex3 = positionDropdown3.value;
        string solutionString3 = positionDropdown3.options[selectedIndex3].text.ToUpper();

        selectedIndex4 = positionDropdown4.value;
        string solutionString4 = positionDropdown4.options[selectedIndex4].text.ToUpper();

        selectedIndex5 = positionDropdown5.value;
        string solutionString5 = positionDropdown5.options[selectedIndex5].text.ToUpper();

        if(solutionString1 == "----" && solutionString2 == "----" && solutionString3 == "----" && solutionString4 == "----" 
            && solutionString5 == "----")
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = ResultType.Empty;
        }
        else if(solutionString1 == "CORAZONES" && solutionString2 == "JOKER" && solutionString3 == "DIAMANTES" 
            && solutionString4 == "TRÉBOLES" && solutionString5 == "PICAS")
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = ResultType.Success;
        }
        else
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = ResultType.Failure;
        }
    }

    public void DisplayCard(TMP_Dropdown dropdown)
    {
        int selectedIndex = dropdown.value;
        string selectedString = dropdown.options[selectedIndex].text.ToUpper();

        if(dropdown.name.ToUpper().Contains("POS 1"))
        {
            cardImagePosition1.SetActive(true);
            if(selectedString == "PICAS")
            {
                cardImagePosition1.GetComponent<Image>().sprite = cardSpriteSpades;
            }
            else if(selectedString == "TRÉBOLES")
            {
                cardImagePosition1.GetComponent<Image>().sprite = cardSpriteClubs;
            }
            else if(selectedString == "DIAMANTES")
            {
                cardImagePosition1.GetComponent<Image>().sprite = cardSpriteDiamonds;
            }
            else if(selectedString == "CORAZONES")
            {
                cardImagePosition1.GetComponent<Image>().sprite = cardSpriteHearts;
            }
            else if(selectedString == "JOKER")
            {
                cardImagePosition1.GetComponent<Image>().sprite = cardSpriteJoker;
            }
            else
            {
                cardImagePosition1.SetActive(false);
            }
        }
        else if(dropdown.name.ToUpper().Contains("POS 2"))
        {
            cardImagePosition2.SetActive(true);
            if(selectedString == "PICAS")
            {
                cardImagePosition2.GetComponent<Image>().sprite = cardSpriteSpades;
            }
            else if(selectedString == "TRÉBOLES")
            {
                cardImagePosition2.GetComponent<Image>().sprite = cardSpriteClubs;
            }
            else if(selectedString == "DIAMANTES")
            {
                cardImagePosition2.GetComponent<Image>().sprite = cardSpriteDiamonds;
            }
            else if(selectedString == "CORAZONES")
            {
                cardImagePosition2.GetComponent<Image>().sprite = cardSpriteHearts;
            }
            else if(selectedString == "JOKER")
            {
                cardImagePosition2.GetComponent<Image>().sprite = cardSpriteJoker;
            }
            else
            {
                cardImagePosition2.SetActive(false);
            }
        }
        else if(dropdown.name.ToUpper().Contains("POS 3"))
        {
            cardImagePosition3.SetActive(true);
            if(selectedString == "PICAS")
            {
                cardImagePosition3.GetComponent<Image>().sprite = cardSpriteSpades;
            }
            else if(selectedString == "TRÉBOLES")
            {
                cardImagePosition3.GetComponent<Image>().sprite = cardSpriteClubs;
            }
            else if(selectedString == "DIAMANTES")
            {
                cardImagePosition3.GetComponent<Image>().sprite = cardSpriteDiamonds;
            }
            else if(selectedString == "CORAZONES")
            {
                cardImagePosition3.GetComponent<Image>().sprite = cardSpriteHearts;
            }
            else if(selectedString == "JOKER")
            {
                cardImagePosition3.GetComponent<Image>().sprite = cardSpriteJoker;
            }
            else
            {
                cardImagePosition3.SetActive(false);
            }
        }
        else if(dropdown.name.ToUpper().Contains("POS 4"))
        {
            cardImagePosition4.SetActive(true);
            if(selectedString == "PICAS")
            {
                cardImagePosition4.GetComponent<Image>().sprite = cardSpriteSpades;
            }
            else if(selectedString == "TRÉBOLES")
            {
                cardImagePosition4.GetComponent<Image>().sprite = cardSpriteClubs;
            }
            else if(selectedString == "DIAMANTES")
            {
                cardImagePosition4.GetComponent<Image>().sprite = cardSpriteDiamonds;
            }
            else if(selectedString == "CORAZONES")
            {
                cardImagePosition4.GetComponent<Image>().sprite = cardSpriteHearts;
            }
            else if(selectedString == "JOKER")
            {
                cardImagePosition4.GetComponent<Image>().sprite = cardSpriteJoker;
            }
            else
            {
                cardImagePosition4.SetActive(false);
            }
        }
        else if(dropdown.name.ToUpper().Contains("POS 5"))
        {
            cardImagePosition5.SetActive(true);
            if(selectedString == "PICAS")
            {
                cardImagePosition5.GetComponent<Image>().sprite = cardSpriteSpades;
            }
            else if(selectedString == "TRÉBOLES")
            {
                cardImagePosition5.GetComponent<Image>().sprite = cardSpriteClubs;
            }
            else if(selectedString == "DIAMANTES")
            {
                cardImagePosition5.GetComponent<Image>().sprite = cardSpriteDiamonds;
            }
            else if(selectedString == "CORAZONES")
            {
                cardImagePosition5.GetComponent<Image>().sprite = cardSpriteHearts;
            }
            else if(selectedString == "JOKER")
            {
                cardImagePosition5.GetComponent<Image>().sprite = cardSpriteJoker;
            }
            else
            {
                cardImagePosition5.SetActive(false);
            }
        }
    }
}