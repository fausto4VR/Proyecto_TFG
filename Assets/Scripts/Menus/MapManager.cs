using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject mapMark;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject mapPanel;

    private bool isPlayerInRange;
    private bool isMapOpen;
    
    void Update()
    {
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
        SceneManager.LoadScene("ParkScene");
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
