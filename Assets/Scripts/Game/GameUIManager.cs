using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUIManager : MonoBehaviour
{
    [Header("Clues Sprites Section")]
    [SerializeField] private Sprite firstClueSprite1;    
    [SerializeField] private Sprite firstClueSprite2;
    [SerializeField] private Sprite secondClueSprite1;
    [SerializeField] private Sprite secondClueSprite2;
    [SerializeField] private Sprite thirdClueSprite1;
    [SerializeField] private Sprite thirdClueSprite2;

    [Header("Suspects Sprites Section")]
    [SerializeField] private Sprite suspectSprite1;
    [SerializeField] private Sprite suspectSprite2;
    [SerializeField] private Sprite suspectSprite3;
    [SerializeField] private Sprite suspectSprite4;
    [SerializeField] private Sprite suspectSprite5;
    [SerializeField] private Sprite suspectSprite6;
    [SerializeField] private Sprite suspectSprite7;
    [SerializeField] private Sprite suspectSprite8;
    
    [Header("Default Sprite Section")]
    [SerializeField] private Sprite defaultSprite;

    [Header("Cursors Textures Section")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D interactCursor;
    
    [Header("Variable Section")]
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    private GameObject canvas;

    private GameObject outDetectionPanel;

    private GameObject dialoguePanel;
    private GameObject dialogueChoiceSection;    
    private GameObject thirdOptionKeyInChoice;
    private GameObject firstOptionButtonInChoice;
    private GameObject secondOptionButtonInChoice;
    private GameObject thirdOptionButtonInChoice;
    private TMP_Text firstOptionTextInChoice;
    private TMP_Text secondOptionTextInChoice;
    private TMP_Text thirdOptionTextInChoice;
    private GameObject dialogueConversationSection;
    private TMP_Text dialogueConversationText;
    private TMP_Text dialogueConversationName;
    private Image dialogueConversationProfile; 

    private GameObject tutorialPanel;
    private TMP_Text tutorialText;
    private GameObject leftArrowTutorialButton;
    private GameObject rightArrowTutorialButton;
    private GameObject cluePanel;
    private TMP_Text clueText;
    private Image clueImage;

    private GameObject theEndSection;
    private GameObject theEndPanel;
    private  TMP_Text opportunitiesText;
    private GameObject guiltyDropdown;
    private TMP_Text sendButtonText;
    private GameObject anotherTryPanel;
    private  TMP_Text anotherTryText;
    private Image anotherTrySuspectImage;
    private GameObject badEndingPanel;
    private GameObject badEndingFailSection;
    private  TMP_Text badEndingText;
    private Image badEndingFailSuspectImage;
    private Image badEndingCorrectSuspectImage;
    private GameObject goodEndingPanel;
    private  TMP_Text goodEndingText;
    private Image goodEndingSuspectImage;

    private GameObject mapPanel;
    private GameObject thirdOptionKeyInMap;
    private GameObject thirdOptionPanelInMap;

    private GameObject briefcaseIconButton;
    private GameObject pausePanel;
    private TMP_Text objectiveTextInPause;
    private TMP_Text pointsTextInPause;
    private GameObject outPanelOptions;
    private GameObject afterSavePanel;
    private TMP_Text afterSaveText;
    private GameObject afterSaveOutPanel;
    private GameObject exitPanel;   
    private GameObject cluesPanelInPause;
    private Image firstClueImage;
    private Image secondClueImage;
    private Image thirdClueImage;
    private TMP_Text firstClueText;
    private TMP_Text secondClueText;
    private TMP_Text thirdClueText;
    private GameObject suspectsPanelInPause;
    private Image suspectImage1;
    private Image suspectImage2;
    private Image suspectImage3;
    private Image suspectImage4;
    private Image suspectImage5;
    private Image suspectImage6;
    private Image suspectImage7;
    private Image suspectImage8;
    private TMP_Text suspectText1;
    private TMP_Text suspectText2;
    private TMP_Text suspectText3;
    private TMP_Text suspectText4;
    private TMP_Text suspectText5;
    private TMP_Text suspectText6;
    private TMP_Text suspectText7;
    private TMP_Text suspectText8;

    private GameObject soundtrack;

    private GameObject audioManager;
    private AudioSource worldMusic;
    private AudioSource puzzleMusic;


    // Método para encontrar el objeto canvas de la escena
    public void FindCanvas()
    {
        GameObject newCanvas = GameObject.Find("Canvas - Immovable");

        if (newCanvas != null)
        {
            canvas = newCanvas;
            GetUIElements();
        }
    }

    // Método para encontrar el objeto soundtrack de la escena
    public void FindAudio()
    {
        GameObject newAudio = GameObject.Find("Soundtrack");

        if (newAudio != null)
        {
            soundtrack = newAudio;
            GetSoundElements();
        }
    }

    // Método para obtener todos los elementos del UI necesarios
    private void GetUIElements()
    {       
        outDetectionPanel = canvas.transform.Find("Out Detection Panel")?.gameObject; 

        dialoguePanel = canvas.transform.Find("Dialogue Panel").gameObject;

        dialogueChoiceSection = dialoguePanel.transform.Find("Choice Section").gameObject;
        dialogueConversationSection = dialoguePanel.transform.Find("Conversation Section").gameObject;

        thirdOptionKeyInChoice = dialogueChoiceSection.transform.Find("Third Option Key Image").gameObject;
        
        firstOptionButtonInChoice = dialogueChoiceSection.transform.Find("First Option Button").gameObject;        
        secondOptionButtonInChoice = dialogueChoiceSection.transform.Find("Second Option Button").gameObject;
        thirdOptionButtonInChoice = dialogueChoiceSection.transform.Find("Third Option Button").gameObject;

        firstOptionTextInChoice = firstOptionButtonInChoice.transform.GetChild(0).GetComponent<TMP_Text>();
        secondOptionTextInChoice = secondOptionButtonInChoice.transform.GetChild(0).GetComponent<TMP_Text>();
        thirdOptionTextInChoice = thirdOptionButtonInChoice.transform.GetChild(0).GetComponent<TMP_Text>();
        
        dialogueConversationText = dialogueConversationSection.transform.Find("Dialogue Text").GetComponent<TMP_Text>();
        dialogueConversationName = dialogueConversationSection.transform.Find("Tape Image").transform.GetChild(0)
            .GetComponent<TMP_Text>();
        dialogueConversationProfile = dialogueConversationSection.transform.Find("Profile Image").GetComponent<Image>();
        
        tutorialPanel = canvas.transform.Find("Tutorial Panel")?.gameObject;

        tutorialText = tutorialPanel?.transform.Find("Tutorial Text")?.GetComponent<TMP_Text>();
        leftArrowTutorialButton = tutorialPanel?.transform.Find("Left Arrow Button")?.gameObject;
        rightArrowTutorialButton = tutorialPanel?.transform.Find("Right Arrow Button")?.gameObject;

        cluePanel = canvas.transform.Find("Clue Panel").gameObject;

        clueText = cluePanel.transform.Find("Clue Text").GetComponent<TMP_Text>();
        clueImage = cluePanel.transform.Find("Clue Image").GetComponent<Image>();

        theEndSection = canvas.transform.Find("The End Section")?.gameObject;

        theEndPanel = theEndSection?.transform.Find("The End Panel")?.gameObject;
        anotherTryPanel = theEndSection?.transform.Find("Another Try Panel")?.gameObject;
        badEndingPanel = theEndSection?.transform.Find("Bad Ending Panel")?.gameObject;
        goodEndingPanel = theEndSection?.transform.Find("Good Ending Panel")?.gameObject;

        opportunitiesText = theEndPanel?.transform.GetChild(0).transform.Find("Opportunities Text")?.GetComponent<TMP_Text>();        
        guiltyDropdown = theEndPanel?.transform.GetChild(0).transform.Find("Guilty Dropdown")?.gameObject;
        sendButtonText = theEndPanel?.transform.GetChild(0).transform.Find("Send Yes Button")?.gameObject.transform.GetChild(0)
            .GetComponent<TMP_Text>();
        
        anotherTryText = anotherTryPanel?.transform.Find("Guilty Text").GetComponent<TMP_Text>();
        anotherTrySuspectImage = anotherTryPanel?.transform.Find("Fail Section").transform.GetChild(1).GetComponent<Image>();

        badEndingFailSection = badEndingPanel?.transform.Find("Fail Section")?.gameObject;
        badEndingText = badEndingPanel?.transform.Find("Guilty Text")?.GetComponent<TMP_Text>();
        badEndingFailSuspectImage = badEndingPanel?.transform.Find("Fail Section")?.transform.GetChild(1).GetComponent<Image>();
        badEndingCorrectSuspectImage = badEndingPanel?.transform.Find("Correct Section")?.transform.GetChild(1).GetComponent<Image>();
        
        goodEndingText = goodEndingPanel?.transform.Find("Guilty Text")?.GetComponent<TMP_Text>();
        goodEndingSuspectImage = goodEndingPanel?.transform.Find("Correct Section")?.transform.GetChild(1).GetComponent<Image>();
        
        mapPanel = canvas.transform.Find("Map Panel").gameObject;

        thirdOptionKeyInMap = mapPanel.transform.Find("Third Key Image").gameObject;
        thirdOptionPanelInMap = mapPanel.transform.Find("Park Panel").gameObject;

        briefcaseIconButton = canvas.transform.Find("Pause Menu").transform.GetChild(0).gameObject;
        pausePanel = canvas.transform.Find("Pause Menu").transform.GetChild(1).gameObject;

        objectiveTextInPause = pausePanel.transform.GetChild(0).transform.Find("Objective Panel").transform.GetChild(1)
            .GetComponent<TMP_Text>();
        pointsTextInPause = pausePanel.transform.GetChild(0).transform.Find("Postit Points Image").transform.GetChild(1)
            .GetComponent<TMP_Text>();
        
        outPanelOptions = pausePanel.transform.GetChild(0).transform.Find("Out Panel Options").gameObject;
        afterSavePanel = pausePanel.transform.GetChild(0).transform.Find("After Save Panel").gameObject;
        afterSaveOutPanel = pausePanel.transform.GetChild(0).transform.Find("Out Panel Detection Button").gameObject;
        exitPanel = pausePanel.transform.GetChild(0).transform.Find("Exit Panel").gameObject;
        cluesPanelInPause = pausePanel.transform.GetChild(0).transform.Find("Clues Board").gameObject;
        suspectsPanelInPause = pausePanel.transform.GetChild(0).transform.Find("Suspects Board").gameObject;

        afterSaveText = afterSavePanel.transform.GetChild(1).GetComponent<TMP_Text>();

        firstClueImage = cluesPanelInPause.transform.Find("First Clue").transform.Find("Clue Image").GetComponent<Image>();
        secondClueImage = cluesPanelInPause.transform.Find("Second Clue").transform.Find("Clue Image").GetComponent<Image>();
        thirdClueImage = cluesPanelInPause.transform.Find("Third Clue").transform.Find("Clue Image").GetComponent<Image>();

        firstClueText = cluesPanelInPause.transform.Find("First Clue").transform.Find("Clue Text").GetComponent<TMP_Text>();
        secondClueText = cluesPanelInPause.transform.Find("Second Clue").transform.Find("Clue Text").GetComponent<TMP_Text>();
        thirdClueText = cluesPanelInPause.transform.Find("Third Clue").transform.Find("Clue Text").GetComponent<TMP_Text>();

        suspectImage1 = suspectsPanelInPause.transform.Find("Suspect 1").transform.Find("Suspect Image").GetComponent<Image>();
        suspectImage2 = suspectsPanelInPause.transform.Find("Suspect 2").transform.Find("Suspect Image").GetComponent<Image>();
        suspectImage3 = suspectsPanelInPause.transform.Find("Suspect 3").transform.Find("Suspect Image").GetComponent<Image>();
        suspectImage4 = suspectsPanelInPause.transform.Find("Suspect 4").transform.Find("Suspect Image").GetComponent<Image>();
        suspectImage5 = suspectsPanelInPause.transform.Find("Suspect 5").transform.Find("Suspect Image").GetComponent<Image>();
        suspectImage6 = suspectsPanelInPause.transform.Find("Suspect 6").transform.Find("Suspect Image").GetComponent<Image>();
        suspectImage7 = suspectsPanelInPause.transform.Find("Suspect 7").transform.Find("Suspect Image").GetComponent<Image>();
        suspectImage8 = suspectsPanelInPause.transform.Find("Suspect 8").transform.Find("Suspect Image").GetComponent<Image>();
        
        suspectText1 = suspectsPanelInPause.transform.Find("Suspect 1").transform.Find("Suspect Text").GetComponent<TMP_Text>();
        suspectText2 = suspectsPanelInPause.transform.Find("Suspect 2").transform.Find("Suspect Text").GetComponent<TMP_Text>();
        suspectText3 = suspectsPanelInPause.transform.Find("Suspect 3").transform.Find("Suspect Text").GetComponent<TMP_Text>();
        suspectText4 = suspectsPanelInPause.transform.Find("Suspect 4").transform.Find("Suspect Text").GetComponent<TMP_Text>();
        suspectText5 = suspectsPanelInPause.transform.Find("Suspect 5").transform.Find("Suspect Text").GetComponent<TMP_Text>();
        suspectText6 = suspectsPanelInPause.transform.Find("Suspect 6").transform.Find("Suspect Text").GetComponent<TMP_Text>();
        suspectText7 = suspectsPanelInPause.transform.Find("Suspect 7").transform.Find("Suspect Text").GetComponent<TMP_Text>();
        suspectText8 = suspectsPanelInPause.transform.Find("Suspect 8").transform.Find("Suspect Text").GetComponent<TMP_Text>();
    }

    // Método para obtener todos los elementos del sonido necesarios
    private void GetSoundElements()
    {
        audioManager = soundtrack.transform.Find("Audio Sources Manager").gameObject;

        Transform worldTransform = soundtrack.transform.Find("World Music");
        if (worldTransform != null) worldMusic = worldTransform.GetComponent<AudioSource>();

        Transform puzzleTransform = soundtrack.transform.Find("Puzzle Music");
        if (puzzleTransform != null) puzzleMusic = puzzleTransform.GetComponent<AudioSource>();
    }

    // Método para obtener el objeto que refleja el panel exterior de las secciones del canvas
    public GameObject OutDetectionPanel
    {
        get { return outDetectionPanel; }
    }

    // Método para obtener el objeto que refleja el panel de diálogo
    public GameObject DialoguePanel
    {
        get { return dialoguePanel; }
    }

    // Método para obtener el objeto que refleja la sección de elegir una opción
    public GameObject DialogueChoiceSection
    {
        get { return dialogueChoiceSection; }
    }

    // Método para obtener el objeto que refleja la tercera tecla en el panel de elección
    public GameObject ThirdOptionKeyInChoice
    {
        get { return thirdOptionKeyInChoice; }
    }

    // Método para obtener el objeto que refleja el primer botón en el panel de elección
    public GameObject FirstOptionButtonInChoice
    {
        get { return firstOptionButtonInChoice; }
    }

    // Método para obtener el objeto que refleja el segundo botón en el panel de elección
    public GameObject SecondOptionButtonInChoice
    {
        get { return secondOptionButtonInChoice; }
    }

    // Método para obtener el objeto que refleja el tercer botón en el panel de elección
    public GameObject ThirdOptionButtonInChoice
    {
        get { return thirdOptionButtonInChoice; }
    }

    // Método para obtener el objeto que refleja el primer texto en el panel de elección
    public TMP_Text FirstOptionTextInChoice
    {
        get { return firstOptionTextInChoice; }
    }

    // Método para obtener el objeto que refleja el segundo texto en el panel de elección
    public TMP_Text SecondOptionTextInChoice
    {
        get { return secondOptionTextInChoice; }
    }

    // Método para obtener el objeto que refleja el tercer texto en el panel de elección
    public TMP_Text ThirdOptionTextInChoice
    {
        get { return thirdOptionTextInChoice; }
    }

    // Método para obtener el objeto que refleja la sección de conversación
    public GameObject DialogueConversationSection
    {
        get { return dialogueConversationSection; }
    }

    // Método para obtener el texto de la línea de diálogo en la conversación
    public TMP_Text DialogueConversationText
    {
        get { return dialogueConversationText; }
    }

    // Método para obtener el texto del nombre del personaje en la conversación
    public TMP_Text DialogueConversationName
    {
        get { return dialogueConversationName; }
    }

    // Método para obtener la imagen de perfil del personaje en la conversación
    public Image DialogueConversationProfile
    {
        get { return dialogueConversationProfile; }
    }

    // Método para obtener el objeto que refleja el panel del tutorial
    public GameObject TutorialPanel
    {
        get { return tutorialPanel; }
    }

    // Método para obtener el texto que se muestra en el tutorial
    public TMP_Text TutorialText
    {
        get { return tutorialText; }
    }

    // Método para obtener el objeto que refleja la flecha izquierda en el panel del tutorial
    public GameObject LeftArrowTutorialButton
    {
        get { return leftArrowTutorialButton; }
    }

    // Método para obtener el objeto que refleja la flecha derecha en el panel del tutorial
    public GameObject RightArrowTutorialButton
    {
        get { return rightArrowTutorialButton; }
    }

    // Método para obtener el objeto que refleja el panel de pistas
    public GameObject CluePanel
    {
        get { return cluePanel; }
    }

    // Método para obtener el texto del nombre de la pista
    public TMP_Text ClueText
    {
        get { return clueText; }
    }

    // Método para obtener la imagen de la pista
    public Image ClueImage
    {
        get { return clueImage; }
    }

    // Método para obtener el objeto que refleja la sección del final del juego
    public GameObject TheEndSection
    {
        get { return theEndSection; }
    }

    // Método para obtener el objeto que refleja el panel para dar la solución al final del juego
    public GameObject TheEndPanel
    {
        get { return theEndPanel; }
    }

    // Método para obtener el objeto que refleja el texto de oportunidades en el panel del final del juego
    public TMP_Text OpportunitiesText
    {
        get { return opportunitiesText; }
    }

    // Método para obtener el objeto que refleja el dropdown para elegir al culpable en el panel del final del juego
    public GameObject GuiltyDropdown
    {
        get { return guiltyDropdown; }
    }

    // Método para obtener el objeto que refleja el texto del botón de enviar en el panel del final del juego
    public TMP_Text SendButtonText
    {
        get { return sendButtonText; }
    }

    // Método para obtener el objeto que refleja el panel para mostrar que hay otro intento en caso de fallo al final del juego
    public GameObject AnotherTryPanel
    {
        get { return anotherTryPanel; }
    }

    // Método para obtener el objeto que refleja el texto del panel de otro intento
    public TMP_Text AnotherTryText
    {
        get { return anotherTryText; }
    }

    // Método para obtener el objeto que refleja la imagen del sospechoso elegido en el panel de otro intento
    public Image AnotherTrySuspectImage
    {
        get { return anotherTrySuspectImage; }
    }

    // Método para obtener el objeto que refleja el panel para el final malo del juego
    public GameObject BadEndingPanel
    {
        get { return badEndingPanel; }
    }

    // Método para obtener el objeto que refleja la sección de fallo en el final malo del juego
    public GameObject BadEndingFailSection
    {
        get { return badEndingFailSection; }
    }

    // Método para obtener el objeto que refleja el texto del panel en el final malo del juego
    public TMP_Text BadEndingText
    {
        get { return badEndingText; }
    }

    // Método para obtener el objeto que refleja la imagen fallida del sospechoso en el panel del final malo del juego
    public Image BadEndingFailSuspectImage
    {
        get { return badEndingFailSuspectImage; }
    }

    // Método para obtener el objeto que refleja la imagen correcta del sospechoso en el panel del final malo del juego
    public Image BadEndingCorrectSuspectImage
    {
        get { return badEndingCorrectSuspectImage; }
    }

    // Método para obtener el objeto que refleja el panel para el final bueno del juego
    public GameObject GoodEndingPanel
    {
        get { return goodEndingPanel; }
    }

    // Método para obtener el objeto que refleja el texto del panel en el final bueno del juego
    public TMP_Text GoodEndingText
    {
        get { return goodEndingText; }
    }

    // Método para obtener el objeto que refleja la imagen correcta del sospechoso en el panel del final bueno del juego
    public Image GoodEndingSuspectImage
    {
        get { return goodEndingSuspectImage; }
    }

    // Método para obtener el objeto que refleja el panel del mapa
    public GameObject MapPanel
    {
        get { return mapPanel; }
    }

    // Método para obtener el objeto que refleja la tercera tecla en el panel del mapa
    public GameObject ThirdOptionKeyInMap
    {
        get { return thirdOptionKeyInMap; }
    }

    // Método para obtener el objeto que refleja el panel de la tercera opción en el mapa
    public GameObject ThirdOptionPanelInMap
    {
        get { return thirdOptionPanelInMap; }
    }

    // Método para obtener el objeto que refleja el botón del maletín
    public GameObject BriefcaseIconButton
    {
        get { return briefcaseIconButton; }
    }

    // Método para obtener el objeto que refleja el panel de pausa
    public GameObject PausePanel
    {
        get { return pausePanel; }
    }

    // Método para obtener el texto del objetivo en el panel de pausa
    public TMP_Text ObjectiveTextInPause
    {
        get { return objectiveTextInPause; }
    }

    // Método para obtener el texto de los puntos obtenidos en el panel de pausa
    public TMP_Text PointsTextInPause
    {
        get { return pointsTextInPause; }
    }

    // Método para obtener el panel invisible posterior a abrir alguna opción
    public GameObject OutPanelOptions
    {
        get { return outPanelOptions; }
    }

    // Método para obtener el panel posterior al guardado
    public GameObject AfterSavePanel
    {
        get { return afterSavePanel; }
    }

    // Método para obtener el panel de detección posterior al guardado para cerrar el mensaje que salga
    public GameObject AfterSaveOutPanel
    {
        get { return afterSaveOutPanel; }
    }

    // Método para obtener el objeto que refleja el texto del panel posterior al guardado
    public TMP_Text AfterSaveText
    {
        get { return afterSaveText; }
    }

    // Método para obtener el objeto que refleja el panel de salida
    public GameObject ExitPanel
    {
        get { return exitPanel; }
    }

    // Método para obtener el objeto que refleja el panel de pistas en pausa
    public GameObject CluesPanelInPause
    {
        get { return cluesPanelInPause; }
    }

    // Método para obtener el objeto que refleja la imagen de la primera pista
    public Image FirstClueImage
    {
        get { return firstClueImage; }
    }

    // Método para obtener el objeto que refleja la imagen de la segunda pista
    public Image SecondClueImage
    {
        get { return secondClueImage; }
    }

    // Método para obtener el objeto que refleja la imagen de la tercera pista
    public Image ThirdClueImage
    {
        get { return thirdClueImage; }
    }

    // Método para obtener el objeto que refleja el texto de la primera pista
    public TMP_Text FirstClueText
    {
        get { return firstClueText; }
    }

    // Método para obtener el objeto que refleja el texto de la segunda pista
    public TMP_Text SecondClueText
    {
        get { return secondClueText; }
    }

    // Método para obtener el objeto que refleja el texto de la tercera pista
    public TMP_Text ThirdClueText
    {
        get { return thirdClueText; }
    }

    // Método para obtener el objeto que refleja el panel de sospechosos en pausa
    public GameObject SuspectsPanelInPause
    {
        get { return suspectsPanelInPause; }
    }

    // Método para obtener el objeto que refleja la imagen del sospechoso 1
    public Image SuspectImage1 
    { 
        get { return suspectImage1; } 
    }

    // Método para obtener el objeto que refleja la imagen del sospechoso 2
    public Image SuspectImage2 
    { 
        get { return suspectImage2; } 
    }

    // Método para obtener el objeto que refleja la imagen del sospechoso 3
    public Image SuspectImage3 
    { 
        get { return suspectImage3; } 
    }

    // Método para obtener el objeto que refleja la imagen del sospechoso 4
    public Image SuspectImage4 
    { 
        get { return suspectImage4; } 
    }

    // Método para obtener el objeto que refleja la imagen del sospechoso 5
    public Image SuspectImage5 
    { 
        get { return suspectImage5; } 
    }

    // Método para obtener el objeto que refleja la imagen del sospechoso 6
    public Image SuspectImage6 
    { 
        get { return suspectImage6; } 
    }

    // Método para obtener el objeto que refleja la imagen del sospechoso 7
    public Image SuspectImage7 
    { 
        get { return suspectImage7; } 
    }

    // Método para obtener el objeto que refleja la imagen del sospechoso 8
    public Image SuspectImage8 
    { 
        get { return suspectImage8; } 
    }

    // Método para obtener una lista de los objetos de las imágenes de los sospechosos
    public List<Image> SuspectImageList
    {
        get 
        {
            List<Image> suspectImageList = new List<Image>
            {
                SuspectImage1,
                SuspectImage2,
                SuspectImage3,
                SuspectImage4,
                SuspectImage5,
                SuspectImage6,
                SuspectImage7,
                SuspectImage8
            };

            return suspectImageList; 
        }
    }

    // Método para obtener el objeto que refleja el texto del sospechoso 1
    public TMP_Text SuspectText1 
    { 
        get { return suspectText1; } 
    }

    // Método para obtener el objeto que refleja el texto del sospechoso 2
    public TMP_Text SuspectText2 
    { 
        get { return suspectText2; } 
    }

    // Método para obtener el objeto que refleja el texto del sospechoso 3
    public TMP_Text SuspectText3 
    { 
        get { return suspectText3; } 
    }

    // Método para obtener el objeto que refleja el texto del sospechoso 4
    public TMP_Text SuspectText4 
    { 
        get { return suspectText4; } 
    }

    // Método para obtener el objeto que refleja el texto del sospechoso 5
    public TMP_Text SuspectText5 
    { 
        get { return suspectText5; } 
    }

    // Método para obtener el objeto que refleja el texto del sospechoso 6
    public TMP_Text SuspectText6 
    { 
        get { return suspectText6; } 
    }

    // Método para obtener el objeto que refleja el texto del sospechoso 7
    public TMP_Text SuspectText7 
    { 
        get { return suspectText7; } 
    }

    // Método para obtener el objeto que refleja el texto del sospechoso 8
    public TMP_Text SuspectText8 
    { 
        get { return suspectText8; } 
    }

    // Método para obtener una lista de los objetos de los textos de los sospechosos
    public List<TMP_Text> SuspectTextsList
    {
        get 
        {
            List<TMP_Text> suspectSpritesList = new List<TMP_Text>
            {
                SuspectText1,
                SuspectText2,
                SuspectText3,
                SuspectText4,
                SuspectText5,
                SuspectText6,
                SuspectText7,
                SuspectText8
            };

            return suspectSpritesList; 
        }
    }

    // Método para obtener el sprite 1 de la primera pista
    public Sprite FirstClueSprite1
    {
        get { return firstClueSprite1; }
    }

    // Método para obtener el sprite 2 de la primera pista
    public Sprite FirstClueSprite2
    {
        get { return firstClueSprite2; }
    }

    // Método para obtener el sprite 1 de la segunda pista
    public Sprite SecondClueSprite1
    {
        get { return secondClueSprite1; }
    }

    // Método para obtener el sprite 2 de la segunda pista
    public Sprite SecondClueSprite2
    {
        get { return secondClueSprite2; }
    }

    // Método para obtener el sprite 1 de la tercera pista
    public Sprite ThirdClueSprite1
    {
        get { return thirdClueSprite1; }
    }

    // Método para obtener el sprite 2 de la tercera pista
    public Sprite ThirdClueSprite2
    {
        get { return thirdClueSprite2; }
    }

    // Método para obtener el sprite del sospechoso 1
    public Sprite SuspectSprite1
    {
        get { return suspectSprite1; }
    }

    // Método para obtener el sprite del sospechoso 2
    public Sprite SuspectSprite2
    {
        get { return suspectSprite2; }
    }

    // Método para obtener el sprite del sospechoso 3
    public Sprite SuspectSprite3
    {
        get { return suspectSprite3; }
    }

    // Método para obtener el sprite del sospechoso 4
    public Sprite SuspectSprite4
    {
        get { return suspectSprite4; }
    }

    // Método para obtener el sprite del sospechoso 5
    public Sprite SuspectSprite5
    {
        get { return suspectSprite5; }
    }

    // Método para obtener el sprite del sospechoso 6
    public Sprite SuspectSprite6
    {
        get { return suspectSprite6; }
    }

    // Método para obtener el sprite del sospechoso 7
    public Sprite SuspectSprite7
    {
        get { return suspectSprite7; }
    }

    // Método para obtener el sprite del sospechoso 8
    public Sprite SuspectSprite8
    {
        get { return suspectSprite8; }
    }

    // Método para obtener una lista de los sprites de los sospechosos
    public List<Sprite> SuspectSpritesList
    {
        get 
        {
            List<Sprite> suspectSpritesList = new List<Sprite>
            {
                SuspectSprite1,
                SuspectSprite2,
                SuspectSprite3,
                SuspectSprite4,
                SuspectSprite5,
                SuspectSprite6,
                SuspectSprite7,
                SuspectSprite8
            };

            return suspectSpritesList; 
        }
    }

    // Método para obtener la textura necesaria del cursor por defecto
    public Texture2D DefaultCursor
    {
        get { return defaultCursor; }
    }

    // Método para obtener la textura necesaria del cursor cuando puede interactuar con un elemento
    public Texture2D InteractCursor
    {
        get { return interactCursor; }
    }

    // Método que efectua el cambio de cursor
    public void SetCursor(Texture2D cursorTexture)
    {
        if (cursorTexture != null)
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    // Método para obtener el sprite por defecto para una imagen que es desconocida
    public Sprite DefaultSprite
    {
        get { return defaultSprite; }
    }

    // Método para obtener una lista de los sprites de los sospechosos
     public GameObject AudioManager
    {
        get { return audioManager; }
    }

    // Método para obtener el objeto que refleja la música del mundo
    public AudioSource WorldMusic
    {
        get { return worldMusic; }
    }

    // Método para obtener el objeto que refleja la música de la escena de puzle
    public AudioSource PuzzleMusic
    {
        get { return puzzleMusic; }
    }
}