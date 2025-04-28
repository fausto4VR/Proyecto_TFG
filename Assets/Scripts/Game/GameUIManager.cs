using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("Sprites Section")]
    [SerializeField] private Sprite firstClueSprite1;    
    [SerializeField] private Sprite firstClueSprite2;
    [SerializeField] private Sprite secondClueSprite1;
    [SerializeField] private Sprite secondClueSprite2;
    [SerializeField] private Sprite thirdClueSprite1;
    [SerializeField] private Sprite thirdClueSprite2;

    private GameObject canvas;
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
    private GameObject cluePanel;
    private TMP_Text clueText;
    private Image clueImage;

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
        
        tutorialPanel = canvas.transform.Find("Tutorial Panel").gameObject;

        tutorialText = tutorialPanel.transform.Find("Tutorial Text").GetComponent<TMP_Text>();

        cluePanel = canvas.transform.Find("Clue Panel").gameObject;

        clueText = cluePanel.transform.Find("Clue Text").GetComponent<TMP_Text>();
        clueImage = cluePanel.transform.Find("Clue Image").GetComponent<Image>();
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

    // Método para obtener el primer 1 de la primera pista
    public Sprite FirstClueSprite1
    {
        get { return firstClueSprite1; }
    }

    // Método para obtener el primer 2 de la primera pista
    public Sprite FirstClueSprite2
    {
        get { return firstClueSprite2; }
    }

    // Método para obtener el primer 1 de la segunda pista
    public Sprite SecondClueSprite1
    {
        get { return secondClueSprite1; }
    }

    // Método para obtener el primer 2 de la segunda pista
    public Sprite SecondClueSprite2
    {
        get { return secondClueSprite2; }
    }

    // Método para obtener el primer 1 de la tercera pista
    public Sprite ThirdClueSprite1
    {
        get { return thirdClueSprite1; }
    }

    // Método para obtener el primer 2 de la tercera pista
    public Sprite ThirdClueSprite2
    {
        get { return thirdClueSprite2; }
    }

    // Método para obtener el objeto que refleja el gestor de audio
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
