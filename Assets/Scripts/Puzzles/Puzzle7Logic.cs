using UnityEngine;
using TMPro;

public class Puzzle7Logic : MonoBehaviour
{   
    [SerializeField, TextArea(6,12)] private string firstSupportText;    
    [SerializeField, TextArea(6,12)] private string secondSupportText;    
    [SerializeField, TextArea(6,12)] private string thirdSupportText; 

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

        if(solutionString1 == "PICAS" && solutionString2 == "PICAS" && solutionString3 == "PICAS" && solutionString4 == "PICAS" 
            && solutionString5 == "PICAS")
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 0;
        }
        else if(solutionString1 == "CORAZONES" && solutionString2 == "JOKER" && solutionString3 == "DIAMANTES" 
            && solutionString4 == "TRÉBOLES" && solutionString5 == "PICAS")
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 1;
        }
        else
        {
            GetComponent<PuzzleUIManager>().isCorrectResult = 2;
        }
    }
}