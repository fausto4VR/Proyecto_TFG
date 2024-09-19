using UnityEngine;

public class PlayerLogicManager : MonoBehaviour
{
    public Sprite playerImage;
    public string playerName = "Player";

    void Update()
    { 
        if(Input.GetKeyDown(KeyCode.G))
        {
            GameStateManager.Instance.SaveData();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            GameStateManager.Instance.LoadData();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            GameStateManager.Instance.ResetData();
        }
    }
}
