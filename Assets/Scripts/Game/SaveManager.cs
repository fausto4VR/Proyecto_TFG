using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static void SavePlayerData(GameObject player)
    {
        PlayerData playerData = new PlayerData(player);
        string playerDataPath = Application.persistentDataPath + "/player.save";
        FileStream fileStream = new FileStream(playerDataPath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, playerData);
        fileStream.Close(); 
    }

    public static PlayerData LoadPlayerData()
    {
        string playerDataPath = Application.persistentDataPath + "/player.save";

        if(File.Exists(playerDataPath))
        {
            FileStream fileStream = new FileStream(playerDataPath, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            PlayerData playerData = (PlayerData) binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return playerData; 
        }
        else
        {
            Debug.LogError("No se encontró el archivo de jugador guardado.");
            return null;
        }
    }

    public static void ResetPlayerData()
    {
        PlayerData playerData = new PlayerData();
        string playerDataPath = Application.persistentDataPath + "/player.save";
        FileStream fileStream = new FileStream(playerDataPath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, playerData);
        fileStream.Close(); 
    }

    public static void SaveGameData(string sceneName, string guilty, string firstClue, string secondClue, string thirdClue, 
        int storyPhase, string lastPuzzleComplete, bool[] knownSuspects, bool[] knownTutorials, bool[] knownDialogues)
    {
        GameData gameData = new GameData(sceneName, guilty, firstClue, secondClue, thirdClue, storyPhase, lastPuzzleComplete, 
            knownSuspects, knownTutorials, knownDialogues);
        string gameDataPath = Application.persistentDataPath + "/game.save";
        FileStream fileStream = new FileStream(gameDataPath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, gameData);
        fileStream.Close(); 
    }

    public static GameData LoadGameData()
    {
        string gameDataPath = Application.persistentDataPath + "/game.save";

        if(File.Exists(gameDataPath))
        {
            FileStream fileStream = new FileStream(gameDataPath, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            GameData gameData = (GameData) binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return gameData; 
        }
        else
        {
            Debug.LogError("No se encontró el archivo de estado del juego guardado.");
            return null;
        }
    }

    public static void ResetGameData()
    {
        GameData gameData = new GameData();
        string gameDataPath = Application.persistentDataPath + "/game.save";
        FileStream fileStream = new FileStream(gameDataPath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, gameData);
        fileStream.Close();
    }
}
