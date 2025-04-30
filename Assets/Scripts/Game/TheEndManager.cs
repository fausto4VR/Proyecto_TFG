using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

// Enum de los tipos de pantallas para el final
public enum EndScreenType
{
    ShowEnding, AnotherTry, BadEnding, GoodEnding
}

public class TheEndManager : MonoBehaviour
{
    [Header("UI Objects Section")]
    [SerializeField] private GameObject theEndMark;
    
    [Header("NPCs Section")]
    [SerializeField] private GameObject fatherNPC;
    [SerializeField] private GameObject victimNPC;

    [Header("Variable Section")]
    [SerializeField] private string lastPuzzle = "Puzzle7";
    [SerializeField] private bool showAllOpportunities;

    
    private List<Sprite> suspectSprites = new List<Sprite>();    
    private Coroutine waitCoroutine;
    private Coroutine closePanelCoroutine;
    private Coroutine closePanelSoundCoroutine;
    private Coroutine sendGuiltyCoroutine;
    private Coroutine sendGuiltySoundCoroutine;
    private Coroutine closeScreenCoroutine;
    private TMP_Dropdown guiltyDropdownOptions;
    private bool isAnySuspectsKnown;
    
    // REVISAR AUDIO
    private AudioSource buttonsAudioSource;
    private AudioSource theEndAudioSource;
    private AudioSource worldMusic;


    void Start()
    {
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        buttonsAudioSource = audioSources[1];
        theEndAudioSource = audioSources[7];
        worldMusic = GameLogicManager.Instance.UIManager.WorldMusic;

        suspectSprites = GameLogicManager.Instance.UIManager.SuspectSpritesList;
    }

    // Corrutina para esperar a que el jugador quiera abrir el panel del final del juego
    private IEnumerator WaitUntilPlayerStartTheEnd()
    {
        if (closePanelCoroutine != null) StopCoroutine(closePanelCoroutine);
        if (closePanelSoundCoroutine != null) StopCoroutine(closePanelSoundCoroutine);
        if (sendGuiltyCoroutine != null) StopCoroutine(sendGuiltyCoroutine);
        if (sendGuiltySoundCoroutine != null) StopCoroutine(sendGuiltySoundCoroutine);
        if (closeScreenCoroutine != null) StopCoroutine(closeScreenCoroutine);

        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.E));
        yield return null;

        DisplayTheEndPanel(true);
    }

    // Corrutina para esperar a que el jugador quiera cerrar el panel del final del juego
    private IEnumerator WaitUntilPlayerClosePanel()
    {
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.N));
        yield return null;
        
        DisplayTheEndPanel(false);
    }
    
    // Método para mostrar el panel de final del juego donde se puede seleccionar el culpable
    public void DisplayTheEndPanel(bool showPanel)
    {
        buttonsAudioSource.Play();
        
        if(showPanel)
        {
            PlayerEvents.StartShowingInformation();

            if((GameLogicManager.Instance.EndOpportunities == 2 && GameLogicManager.Instance.LastPuzzleComplete == lastPuzzle) 
                || GameLogicManager.Instance.EndOpportunities == 2 && showAllOpportunities)
            GameLogicManager.Instance.UIManager.OpportunitiesText.text = "Te quedan 2 intentos.";

            else if(GameLogicManager.Instance.EndOpportunities == 0)
            GameLogicManager.Instance.UIManager.OpportunitiesText.text = "No te quedan más intentos.";

            else if(GameLogicManager.Instance.EndOpportunities == 1 || !(GameLogicManager.Instance.LastPuzzleComplete == lastPuzzle))
            GameLogicManager.Instance.UIManager.OpportunitiesText.text = "Te queda 1 intento.";

            else
            Debug.LogError("No se cumplen las condiciones para establecer el texto de oportunidades restantes.");

            if(GameLogicManager.Instance.EndOpportunities == 0)
            {
                GameLogicManager.Instance.UIManager.SendButtonText.text = "Mostrar";
            }
            
            UploadDropdown();
            GameLogicManager.Instance.UIManager.TheEndPanel.SetActive(true);
            GameLogicManager.Instance.UIManager.TheEndSection.SetActive(true);
            
            closePanelCoroutine = StartCoroutine(WaitUntilPlayerClosePanel());
            sendGuiltyCoroutine = StartCoroutine(WaitForSendPlayer());
        }
        else
        {            
            if (closePanelCoroutine != null) StopCoroutine(closePanelCoroutine);
            if (sendGuiltyCoroutine != null) StopCoroutine(sendGuiltyCoroutine);

            closePanelSoundCoroutine = StartCoroutine(WaitForSoundAndClosePanel());
        }       
    } 

    // Método para cargar el dropdown de culpables conocidos por el jugador
    private void UploadDropdown()
    {
        guiltyDropdownOptions = GameLogicManager.Instance.UIManager.GuiltyDropdown.GetComponent<TMP_Dropdown>();
        guiltyDropdownOptions.ClearOptions();

        List<string> options = new List<string>();
        
        if(GameLogicManager.Instance.EndOpportunities == 0)
        {
            options.Add("Ya sabes quien es el culpable");
            guiltyDropdownOptions.AddOptions(options);
            isAnySuspectsKnown = true;
            return;
        }

        options.AddRange(GameLogicManager.Instance.GuiltyNames.Where((name, i) => GameLogicManager.Instance.KnownSuspects[i]));

        if(options.Count > 0)
        {
            guiltyDropdownOptions.AddOptions(options);
            isAnySuspectsKnown = true;
        }
        else
        {
            options.Add("No conoces a ningún sospechoso");
            guiltyDropdownOptions.AddOptions(options);
            isAnySuspectsKnown = false;
        }
    }

    // Corrutina para esperar a que el jugador quiera mandar al sospechoso seleccionado
    private IEnumerator WaitForSendPlayer()
    {
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Y) && isAnySuspectsKnown);
        yield return null;

        SendGuilty();
    }

    // Método para que el jugador mande al sospechoso seleccionado 
    public void SendGuilty()
    {
        if(isAnySuspectsKnown)
        {            
            if (closePanelCoroutine != null) StopCoroutine(closePanelCoroutine);
            if (sendGuiltyCoroutine != null) StopCoroutine(sendGuiltyCoroutine);

            buttonsAudioSource.Play();
            sendGuiltySoundCoroutine = StartCoroutine(WaitForSoundAndSendGuilty());            
        }
    }

    // Corrutina para esperar que suene el sonido del botón y se mande al sospechoso
    private IEnumerator WaitForSoundAndSendGuilty()
    {
        yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        yield return null;

        worldMusic.Pause();
        theEndAudioSource.Play();

        if(GameLogicManager.Instance.EndOpportunities == 0)
        {
            SelectEndScreen(EndScreenType.ShowEnding, "");
        }
        else
        {
            int selectedIndex = guiltyDropdownOptions.GetComponent<TMP_Dropdown>().value;
            string selectedGuilty = guiltyDropdownOptions.GetComponent<TMP_Dropdown>().options[selectedIndex].text;

            if(GameLogicManager.Instance.Guilty == selectedGuilty)
            {
                SelectEndScreen(EndScreenType.GoodEnding, selectedGuilty);
            }
            else
            {
                if((GameLogicManager.Instance.EndOpportunities == 2 && GameLogicManager.Instance.LastPuzzleComplete == lastPuzzle) 
                    || showAllOpportunities)
                SelectEndScreen(EndScreenType.AnotherTry, selectedGuilty);

                else if(GameLogicManager.Instance.EndOpportunities == 1 || !(GameLogicManager.Instance.LastPuzzleComplete == lastPuzzle))
                SelectEndScreen(EndScreenType.BadEnding, selectedGuilty);
            }
        }
    }

    // Método para elegir la pantalla de final correspondiente
    private void SelectEndScreen(EndScreenType endScreenType, string selectedGuilty)
    {
        GameLogicManager.Instance.UIManager.TheEndPanel.SetActive(false);

        if (endScreenType == EndScreenType.ShowEnding)
        {            
            if(GameLogicManager.Instance.IsBadEnding)
            {
                GameLogicManager.Instance.UIManager.BadEndingText.text = 
                    "El culpable era " + GameLogicManager.Instance.Guilty + " y no lo has elegido.";

                GameLogicManager.Instance.UIManager.BadEndingCorrectSuspectImage.sprite = 
                    suspectSprites[GameLogicManager.Instance.GuiltyNames.FindIndex(name => name == GameLogicManager.Instance.Guilty)];
                
                GameLogicManager.Instance.UIManager.BadEndingFailSection.SetActive(false);
                GameLogicManager.Instance.UIManager.BadEndingPanel.SetActive(true);
            }
            else
            {
                GameLogicManager.Instance.UIManager.GoodEndingPanel.SetActive(true);
                GameLogicManager.Instance.UIManager.GoodEndingText.text = 
                    "El culpable era " + GameLogicManager.Instance.Guilty + " y lo has elegido.";

                GameLogicManager.Instance.UIManager.GoodEndingSuspectImage.sprite = 
                    suspectSprites[GameLogicManager.Instance.GuiltyNames.FindIndex(name => name == GameLogicManager.Instance.Guilty)];
            }
        }
        else if (endScreenType == EndScreenType.AnotherTry)
        {            
            GameLogicManager.Instance.UIManager.AnotherTryText.text = "El culpable no era " + selectedGuilty + " y lo has elegido.";

            GameLogicManager.Instance.UIManager.AnotherTrySuspectImage.sprite = 
                suspectSprites[GameLogicManager.Instance.GuiltyNames.FindIndex(name => name == selectedGuilty)];
            
            GameLogicManager.Instance.UIManager.AnotherTryPanel.SetActive(true);
            
            GameLogicManager.Instance.EndOpportunities = 1;
        }
        else if (endScreenType == EndScreenType.BadEnding)
        {            
            GameLogicManager.Instance.UIManager.BadEndingText.text = 
                "El culpable era " + GameLogicManager.Instance.Guilty + " y has elegido a " + selectedGuilty + ".";

            GameLogicManager.Instance.UIManager.BadEndingCorrectSuspectImage.sprite = 
                suspectSprites[GameLogicManager.Instance.GuiltyNames.FindIndex(name => name == GameLogicManager.Instance.Guilty)];
            
            GameLogicManager.Instance.UIManager.BadEndingFailSuspectImage.sprite = 
                suspectSprites[GameLogicManager.Instance.GuiltyNames.FindIndex(name => name == selectedGuilty)];

            GameLogicManager.Instance.UIManager.BadEndingPanel.SetActive(true);
            
            GameLogicManager.Instance.EndOpportunities = 0;
            GameLogicManager.Instance.IsBadEnding = true;
            ShowFinalNPC();
        }
        else if (endScreenType == EndScreenType.GoodEnding)
        {
            GameLogicManager.Instance.UIManager.GoodEndingText.text = 
                "El culpable era " + GameLogicManager.Instance.Guilty + " y lo has elegido.";

            GameLogicManager.Instance.UIManager.GoodEndingSuspectImage.sprite = 
                suspectSprites[GameLogicManager.Instance.GuiltyNames.FindIndex(name => name == GameLogicManager.Instance.Guilty)];
                
            GameLogicManager.Instance.UIManager.GoodEndingPanel.SetActive(true);
            
            GameLogicManager.Instance.EndOpportunities = 0;
            GameLogicManager.Instance.IsBadEnding = false;
            ShowFinalNPC();
        }
        else
        {
            Debug.LogError("No se cumplen las condiciones para mostrar correctamente ninguna pantalla de final.");
        }

        closeScreenCoroutine = StartCoroutine(WaitUntilPlayerCloseScreen());
    }

    // Método para mostrar el NPC correspondiente en función del final conseguido
    private void ShowFinalNPC()
    {
        if(GameLogicManager.Instance.IsBadEnding) fatherNPC.SetActive(true);
        else victimNPC.SetActive(true);

        GameLogicManager.Instance.CurrentStoryPhase = StoryStateManager.CreateLastPhase();
    }

    // Corrutina para esperar a que el jugador quiera cerrar la pantalla del final del juego
    private IEnumerator WaitUntilPlayerCloseScreen()
    {
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0));
        yield return null;

        GameLogicManager.Instance.UIManager.AnotherTryPanel.SetActive(false);
        GameLogicManager.Instance.UIManager.BadEndingPanel.SetActive(false);
        GameLogicManager.Instance.UIManager.GoodEndingPanel.SetActive(false);
        
        PlayerEvents.FinishShowingInformation();
        
        theEndAudioSource.Stop();
        worldMusic.Play();
        
        GameStateManager.Instance.SaveData();
        
        waitCoroutine = StartCoroutine(WaitUntilPlayerStartTheEnd());
    }

    // Corrutina para esperar que suene el sonido del botón y se cierre el panel de final del juego
    private IEnumerator WaitForSoundAndClosePanel()
    {
        // Por si se quisiera esperar a que suene el audio del botón de cierre entero
        // yield return new WaitForSeconds(buttonsAudioSource.clip.length);
        yield return null;

        isAnySuspectsKnown = false;           
        GameLogicManager.Instance.UIManager.TheEndSection.SetActive(false);
        GameLogicManager.Instance.UIManager.TheEndPanel.SetActive(false);

        PlayerEvents.FinishShowingInformation();

        waitCoroutine = StartCoroutine(WaitUntilPlayerStartTheEnd());
    }

    // Método que se llama cuando un objeto entra en el área de colisión del trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {            
            theEndMark.SetActive(true);
            waitCoroutine = StartCoroutine(WaitUntilPlayerStartTheEnd());
        }
    }

    // Método que se llama cuando un objeto sale del área de colisión del trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            theEndMark.SetActive(false);
            StopAllCoroutines();

            if (waitCoroutine != null) waitCoroutine = null;
            if (closePanelCoroutine != null) closePanelCoroutine = null;
            if (closePanelSoundCoroutine != null) closePanelSoundCoroutine = null;
            if (sendGuiltyCoroutine != null) sendGuiltyCoroutine = null;
            if (sendGuiltySoundCoroutine != null) sendGuiltySoundCoroutine = null;
            if (closeScreenCoroutine != null) closeScreenCoroutine = null;
        }
    }
}
