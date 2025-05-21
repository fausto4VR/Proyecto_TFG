[System.Serializable]
public class PlayerData
{
    public float[] position = new float[3];

    public PlayerData(float positionX, float positionY, float positionZ)
    {
        position[0] = positionX;
        position[1] = positionY;
        position[2] = positionZ;
    }

    public PlayerData()
    {
        position[0] = 0f;
        position[1] = 0f;
        position[2] = 0f;
    }
}
