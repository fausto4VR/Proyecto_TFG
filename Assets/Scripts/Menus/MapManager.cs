using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject mapMark;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject audioSourcesManager;
    [SerializeField] private GameObject thirdOptionKey;
    [SerializeField] private GameObject thirdOptionPanel;

    private bool isPlayerInRange;
    private bool isMapOpen;
    private AudioSource mapAudioSource;
    private bool isThirdOptionActive;

    void Start()
    {        
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        mapAudioSource = audioSources[4];
    }
    
    void Update()
    {
        if(!string.IsNullOrEmpty(GameLogicManager.Instance.Clues[1]) && !isThirdOptionActive)
        {
            thirdOptionKey.SetActive(true);
            thirdOptionPanel.SetActive(true);
            isThirdOptionActive = true;
        }

        if(isPlayerInRange && Input.GetKeyDown(KeyCode.M))
        {
            DisplayMapPanel();
        }

        if(isMapOpen && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)))
        {
            GoToHomeScene();
        }

        if(isMapOpen && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)))
        {
            GoToMansionScene();
        }

        if(isMapOpen && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)))
        {
            GoToParkScene();
        }
    }

    public void DisplayMapPanel()
    {
        mapAudioSource.Play();

        if(mapPanel.activeInHierarchy == true)
        {
            player.GetComponent<PlayerMovement>().isPlayerInspecting = false;
            isMapOpen = false;
            mapPanel.SetActive(false);
        }
        else if(mapPanel.activeInHierarchy == false)
        {
            player.GetComponent<PlayerMovement>().isPlayerInspecting = true;
            isMapOpen = true;
            mapPanel.SetActive(true);
        }
    }

    public void GoToHomeScene()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void GoToMansionScene()
    {
        SceneManager.LoadScene("MansionScene");
    }

    public void GoToParkScene()
    {
        if(!string.IsNullOrEmpty(GameLogicManager.Instance.Clues[1]))
        {
            SceneManager.LoadScene("ParkScene");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            mapMark.SetActive(true);
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(mapMark != null)
            {
                mapMark.SetActive(false);                
            }
            isPlayerInRange = false;
        }
    }
}
