using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] position = new float[3];

    public PlayerData(GameObject player)
    {
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }

    public PlayerData()
    {
        position[0] = 0f;
        position[1] = 0f;
        position[2] = 0f;
    }
}
