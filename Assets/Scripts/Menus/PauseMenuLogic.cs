using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class PauseMenuLogic : MonoBehaviour
{
    [Header("Variable Section")]
    [SerializeField] private string menuSceneName = "MenuScene";

    private List<Image> suspectImages = new List<Image>();
    private List<Sprite> suspectSprites = new List<Sprite>();
    private List<TMP_Text> suspectTexts = new List<TMP_Text>();
    private Coroutine selectCoroutine;
    private Coroutine selectSoundCoroutine;
    private Coroutine closePanelCoroutine;
    private Coroutine closePanelSoundCoroutine;
    private Coroutine closeOptionPanelCoroutine;
    private Coroutine closeOptionSoundCoroutine;
    private bool isPauseMenuOpen;
    private bool isOptionPanelShown;
    private bool isSelectCoroutineRunning;
    private bool isNecesaryGoToMainMenu;
    
    // REVISAR AUDIO
    private AudioSource buttonsAudioSource;
    private AudioSource pauseAudioSource;


    void Start() 
    {
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;        
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        buttonsAudioSource = audioSources[1];
        pauseAudioSource = audioSources[6];
        
        suspectSprites = GameLogicManager.Instance.UIManager.SuspectSpritesList;
        suspectImages = GameLogicManager.Instance.UIManager.SuspectImageList;
        suspectTexts = GameLogicManager.Instance.UIManager.SuspectTextsList;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            DisplayMenuPanel(true);
        }
    }

    // Método para mostrar el panel de pausa donde se puede seleccionar entre diferentes opciones
    public void DisplayMenuPanel(bool showPanel)
    {
        if (GameLogicManager.Instance.Player.GetComponent<PlayerLogicManager>().PlayerState is IdleState || isPauseMenuOpen)
        {            
            pauseAudioSource.Play();

            if (showPanel)
            {
                PlayerEvents.StartShowingInformation();

                UpdateObjectiveText();
                GameLogicManager.Instance.UIManager.BriefcaseIconButton.SetActive(false);
                GameLogicManager.Instance.UIManager.PausePanel.SetActive(true);
                isPauseMenuOpen = true;

                closePanelCoroutine = StartCoroutine(WaitUntilPlayerClosePanel());
                selectCoroutine = StartCoroutine(WaitUntilPlayerSelectOption());
            }
            else
            {
                closePanelSoundCoroutine = StartCoroutine(WaitForSoundAndClosePanel());
            }
        }
    }

    // Método para actualizar el texto de objetivos en el menú de pausa
    private void UpdateObjectiveText()
    {
        string validSubphase = GameLogicManager.Instance.CurrentStoryPhase.GetPhaseToString();

        GameLogicManager.Instance.UIManager.ObjectiveTextInPause.text = GameStateManager.Instance.gameText.objectivesInMenu
            .FirstOrDefault(text => $"{text.phase}.{text.subphase}" == validSubphase)?.objective 
            ?? GameStateManager.Instance.gameText.objectivesInMenu
            .FirstOrDefault(text => $"{text.phase}.{text.subphase}" == "Default.Default")?.objective;
    }

    // Corrutina para esperar a que el jugador quiera cerrar el panel de pausa
    private IEnumerator WaitUntilPlayerClosePanel()
    {
        yield return null;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.I));
        yield return null;
        
        DisplayMenuPanel(false);
    }

    // Corrutina para esperar a que el jugador elija una de las opciones disponibles
    private IEnumerator WaitUntilPlayerSelectOption()
    {
        isSelectCoroutineRunning = true;
        bool isOptionChosen = false;

        while (!isOptionChosen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                isOptionChosen = true;
                isSelectCoroutineRunning = false;
                HandleOptionSelected(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                isOptionChosen = true;
                isSelectCoroutineRunning = false;
                HandleOptionSelected(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                isOptionChosen = true;
                isSelectCoroutineRunning = false;
                HandleOptionSelected(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                isOptionChosen = true;
                isSelectCoroutineRunning = false;
                HandleOptionSelected(4);
            }

            yield return null;
        }
    }

    // Método para seleccionar la opción del menú de pausa por parte del jugador
    public void HandleOptionSelected(int option)
    {
        if (!isOptionPanelShown)
        {
            buttonsAudioSource.Play();

            if (selectCoroutine != null) StopCoroutine(selectCoroutine);

            selectSoundCoroutine = StartCoroutine(WaitForSoundAndSelectOption(option));            
        }
        else
        {
            if (!isSelectCoroutineRunning)
            {
                selectCoroutine = StartCoroutine(WaitUntilPlayerSelectOption());
            }
        }      
    }    

    // Corrutina para esperar que suene el sonido del botón y se selecciona la opción elejida
    private IEnumerator WaitForSoundAndSelectOption(int option)
    {
        // Por si se quisiera esperar a que suene el audio del botón entero
        // yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        yield return null;

        isOptionPanelShown = true;

        if (option == 1) DisplayCluesBoard(true);
        else if (option == 2) DisplaySuspectsBoard(true);
        else if (option == 3) SaveGame(true);
        else if (option == 4) DisplayGoToMainMenuPanel(true);
        else Debug.LogError("Ha habido un error al elegir una de las opciones en el menú de pausa.");
    }

    // Método para mostrar el panel de pistas
    private void DisplayCluesBoard(bool showPanel)
    {
        if (showPanel)
        {
            UnlockClues();
            GameLogicManager.Instance.UIManager.OutPanelOptions.SetActive(true);
            GameLogicManager.Instance.UIManager.CluesPanelInPause.SetActive(true);
            closeOptionPanelCoroutine = StartCoroutine(WaitUntilPlayerCloseOptionPanel(1));
        }
        else
        {
            GameLogicManager.Instance.UIManager.OutPanelOptions.SetActive(false);
            GameLogicManager.Instance.UIManager.CluesPanelInPause.SetActive(false);
            isOptionPanelShown = false;
            selectCoroutine = StartCoroutine(WaitUntilPlayerSelectOption());
        }
    }

    // Método para desbloquear las pistas que se pueden mostrar
    private void UnlockClues()
    {
        if (GameLogicManager.Instance.KnownClues[0])
        {
            GameLogicManager.Instance.UIManager.FirstClueText.text = GameLogicManager.Instance.Clues[0];

            if (GameLogicManager.Instance.FirstClueGroup1.Contains(GameLogicManager.Instance.Guilty))
            {
                GameLogicManager.Instance.UIManager.FirstClueImage.sprite = GameLogicManager.Instance.UIManager.FirstClueSprite1;
            }
            else if (GameLogicManager.Instance.FirstClueGroup2.Contains(GameLogicManager.Instance.Guilty))
            {
                GameLogicManager.Instance.UIManager.FirstClueImage.sprite = GameLogicManager.Instance.UIManager.FirstClueSprite2;
            }
        }
        else
        {
            GameLogicManager.Instance.UIManager.FirstClueText.text = "Desconocido";
            GameLogicManager.Instance.UIManager.FirstClueImage.sprite = GameLogicManager.Instance.UIManager.DefaultSprite;
        }

        if (GameLogicManager.Instance.KnownClues[1])
        {
            GameLogicManager.Instance.UIManager.SecondClueText.text = GameLogicManager.Instance.Clues[1];

            if (GameLogicManager.Instance.SecondClueGroup1.Contains(GameLogicManager.Instance.Guilty))
            {
                GameLogicManager.Instance.UIManager.SecondClueImage.sprite = GameLogicManager.Instance.UIManager.SecondClueSprite1;
            }
            else if (GameLogicManager.Instance.SecondClueGroup2.Contains(GameLogicManager.Instance.Guilty))
            {
                GameLogicManager.Instance.UIManager.SecondClueImage.sprite = GameLogicManager.Instance.UIManager.SecondClueSprite2;
            }
        }
        else
        {
            GameLogicManager.Instance.UIManager.SecondClueText.text = "Desconocido";
            GameLogicManager.Instance.UIManager.SecondClueImage.sprite = GameLogicManager.Instance.UIManager.DefaultSprite;
        }

        if (GameLogicManager.Instance.KnownClues[2])
        {
            GameLogicManager.Instance.UIManager.ThirdClueText.text = GameLogicManager.Instance.Clues[2];

            if (GameLogicManager.Instance.ThirdClueGroup1.Contains(GameLogicManager.Instance.Guilty))
            {
                GameLogicManager.Instance.UIManager.ThirdClueImage.sprite = GameLogicManager.Instance.UIManager.ThirdClueSprite1;
            }
            else if (GameLogicManager.Instance.ThirdClueGroup2.Contains(GameLogicManager.Instance.Guilty))
            {
                GameLogicManager.Instance.UIManager.ThirdClueImage.sprite = GameLogicManager.Instance.UIManager.ThirdClueSprite2;
            }
        }
        else
        {
            GameLogicManager.Instance.UIManager.ThirdClueText.text = "Desconocido";
            GameLogicManager.Instance.UIManager.ThirdClueImage.sprite = GameLogicManager.Instance.UIManager.DefaultSprite;
        }
    }

    // Método para mostrar el panel de sospechosos
    private void DisplaySuspectsBoard(bool showPanel)
    {
        if (showPanel)
        {
            UnlockSuspects();
            GameLogicManager.Instance.UIManager.OutPanelOptions.SetActive(true);
            GameLogicManager.Instance.UIManager.SuspectsPanelInPause.SetActive(true);
            closeOptionPanelCoroutine = StartCoroutine(WaitUntilPlayerCloseOptionPanel(2));
        }
        else
        {
            GameLogicManager.Instance.UIManager.OutPanelOptions.SetActive(false);
            GameLogicManager.Instance.UIManager.SuspectsPanelInPause.SetActive(false);
            isOptionPanelShown = false;
            selectCoroutine = StartCoroutine(WaitUntilPlayerSelectOption());
        }
    }

    // Método para desbloquear los sospechosos que se pueden mostrar
    private void UnlockSuspects()
    {
        for (int i = 0; i < GameLogicManager.Instance.GuiltyNames.Count(); i++)
        {
            if (GameLogicManager.Instance.KnownSuspects[i])
            {
                suspectTexts[i].text = GameLogicManager.Instance.GuiltyNames[i];
                suspectImages[i].sprite = suspectSprites[i];
            }
            else
            {
                suspectTexts[i].text = "Desconocido";
                suspectImages[i].sprite = GameLogicManager.Instance.UIManager.DefaultSprite;
            }
        }
    }

    // Método para guardar la partida
    private void SaveGame(bool showPanel)
    {
        if (showPanel)
        {
            GameStateManager.Instance.SaveData();
            GameData gameData = SaveManager.LoadGameData();

            Debug.Log(GameLogicManager.Instance.CurrentStoryPhase.GetPhaseToString());
            Debug.Log(gameData.gameStoryPhase.ToStoryPhase().GetPhaseToString());

            if(gameData != null && GameLogicManager.Instance.CurrentStoryPhase.GetPhaseToString() 
                == gameData.gameStoryPhase.ToStoryPhase().GetPhaseToString())
            GameLogicManager.Instance.UIManager.AfterSaveText.text = "La partida se ha guardado correctamente.";

            else
            GameLogicManager.Instance.UIManager.AfterSaveText.text = "Ha habido un error al guardar la partida. Inténtalo otra vez.";
            
            GameLogicManager.Instance.UIManager.AfterSavePanel.SetActive(true);
            GameLogicManager.Instance.UIManager.AfterSaveOutPanel.SetActive(true);
            closeOptionPanelCoroutine = StartCoroutine(WaitUntilPlayerCloseOptionPanel(3));
        }
        else
        {
            GameLogicManager.Instance.UIManager.AfterSavePanel.SetActive(false);
            GameLogicManager.Instance.UIManager.AfterSaveOutPanel.SetActive(false);
            isOptionPanelShown = false;
            selectCoroutine = StartCoroutine(WaitUntilPlayerSelectOption());
        }
    }

    // Método para mostrar el panel de ir al menú principal
    private void DisplayGoToMainMenuPanel(bool showPanel)
    {
        if (showPanel)
        {
            GameLogicManager.Instance.UIManager.OutPanelOptions.SetActive(true);
            GameLogicManager.Instance.UIManager.ExitPanel.SetActive(true);
            closeOptionPanelCoroutine = StartCoroutine(WaitUntilPlayerCloseOptionPanel(4));
        }
        else
        {
            GameLogicManager.Instance.UIManager.OutPanelOptions.SetActive(false);
            GameLogicManager.Instance.UIManager.ExitPanel.SetActive(false);
            isOptionPanelShown = false;
            selectCoroutine = StartCoroutine(WaitUntilPlayerSelectOption());
        }
    }

    // Corrutina para esperar a que el jugador quiera cerrar el panel de opciones correspondiente
    private IEnumerator WaitUntilPlayerCloseOptionPanel(int option)
    {
        bool isOptionChosen = false;

        while (!isOptionChosen)
        {
            if (option == 1 && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1) 
                || Input.GetKeyDown(KeyCode.Space)))
            {
                isOptionChosen = true;
                HandleOptionReturn(1);
            }
            else if (option == 2 && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2) 
                || Input.GetKeyDown(KeyCode.Space)))
            {
                isOptionChosen = true;
                HandleOptionReturn(2);
            }
            else if (option == 3 && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3) 
                || Input.GetKeyDown(KeyCode.Space)))
            {
                isOptionChosen = true;
                HandleOptionReturn(3);
            }
            else if (option == 4 && (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4) 
                || Input.GetKeyDown(KeyCode.N)))
            {
                isOptionChosen = true;
                HandleOptionReturn(4);
            }
            else if (option == 4 && Input.GetKeyDown(KeyCode.Y))
            {
                isOptionChosen = true;
                HandleOptionReturn(5);
            }

            yield return null;
        }
    }

    // Método para volver al panel del menú desde la opción elejida anteriormente
    public void HandleOptionReturn(int option)
    {
        if (isOptionPanelShown)
        {
            buttonsAudioSource.Play();

            if (closeOptionPanelCoroutine != null) StopCoroutine(closeOptionPanelCoroutine);

            closeOptionSoundCoroutine = StartCoroutine(WaitForSoundAndReturnOption(option)); 
        }
        else
        {
            Debug.LogError("Ha habido un error al intentar cerrar un panel de opción.");

            if (!isSelectCoroutineRunning)
            {
                selectCoroutine = StartCoroutine(WaitUntilPlayerSelectOption());
            }
        }
    }

    // Corrutina para esperar que suene el sonido del botón y se selecciona la opción elejida
    private IEnumerator WaitForSoundAndReturnOption(int option)
    {
        // Por si se quisiera esperar a que suene el audio del botón entero
        // yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        yield return null;

        if (option == 1) DisplayCluesBoard(false);
        else if (option == 2) DisplaySuspectsBoard(false);
        else if (option == 3) SaveGame(false);
        else if (option == 4) DisplayGoToMainMenuPanel(false);        
        else if (option == 5) GoToMainMenu();
        else Debug.LogError("Ha habido un error a la hora de cerrar un panel de las opciones en el menú de pausa.");
    }

    // Método para indicar que hay que ir al menú principal
    private void GoToMainMenu()
    {
        isNecesaryGoToMainMenu = true;
        closePanelSoundCoroutine = StartCoroutine(WaitForSoundAndClosePanel());
    }

    // Corrutina para esperar que suene el sonido del botón y se cierre el panel de pausa
    private IEnumerator WaitForSoundAndClosePanel()
    {
        // Por si se quisiera esperar a que suene el audio del menú de pausa entero
        // yield return new WaitForSeconds(pauseAudioSource.clip.length);
        yield return null;

        GameLogicManager.Instance.UIManager.OutPanelOptions.SetActive(false);
        GameLogicManager.Instance.UIManager.CluesPanelInPause.SetActive(false);
        GameLogicManager.Instance.UIManager.SuspectsPanelInPause.SetActive(false);
        GameLogicManager.Instance.UIManager.AfterSavePanel.SetActive(false);
        GameLogicManager.Instance.UIManager.AfterSaveOutPanel.SetActive(false);
        GameLogicManager.Instance.UIManager.ExitPanel.SetActive(false);

        GameLogicManager.Instance.UIManager.PausePanel.SetActive(false);
        GameLogicManager.Instance.UIManager.BriefcaseIconButton.SetActive(true);

        PlayerEvents.FinishShowingInformation();

        isOptionPanelShown = false;

        if (selectCoroutine != null)
        {
            StopCoroutine(selectCoroutine);
            selectCoroutine = null;
        }

        if (selectSoundCoroutine != null)
        {
            StopCoroutine(selectSoundCoroutine);
            selectSoundCoroutine = null;
        }

        if (closePanelCoroutine != null)
        {
            StopCoroutine(closePanelCoroutine);
            closePanelCoroutine = null;
        }

        if (closePanelSoundCoroutine != null)
        {
            StopCoroutine(closePanelSoundCoroutine);
            closePanelSoundCoroutine = null;
        }

        if (closeOptionPanelCoroutine != null)
        {
            StopCoroutine(closeOptionPanelCoroutine);
            closeOptionPanelCoroutine = null;
        }

        if (closeOptionSoundCoroutine != null)
        {
            StopCoroutine(closeOptionSoundCoroutine);
            closeOptionSoundCoroutine = null;
        }

        isPauseMenuOpen = false;

        if (isNecesaryGoToMainMenu) SceneManager.LoadScene(menuSceneName);
    }
}