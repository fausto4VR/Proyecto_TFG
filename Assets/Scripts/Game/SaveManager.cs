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
            Debug.Log("No se encontró el archivo de jugador guardado.");
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
        int storyPhase, string lastPuzzleComplete, bool[] knownSuspects, bool[] knownTutorials, bool[] knownDialogues, bool isBadEnding)
    {
        GameData gameData = new GameData(sceneName, guilty, firstClue, secondClue, thirdClue, storyPhase, lastPuzzleComplete, 
            knownSuspects, knownTutorials, knownDialogues, isBadEnding);
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
            Debug.Log("No se encontró el archivo de estado del juego guardado.");
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

    public static void SavePuzzleData(bool[] puzzle1Supports, int puzzle1Points, bool[] puzzle2Supports, int puzzle2Points,
        bool[] puzzle3Supports, int puzzle3Points, bool[] puzzle4Supports, int puzzle4Points, 
        bool[] puzzle5Supports, int puzzle5Points, bool[] puzzle6Supports, int puzzle6Points,
        bool[] puzzle7Supports, int puzzle7Points, bool[] puzzle8Supports, int puzzle8Points)
    {
        PuzzleData puzzleData = new PuzzleData(puzzle1Supports, puzzle1Points, puzzle2Supports, puzzle2Points, 
            puzzle3Supports, puzzle3Points, puzzle4Supports, puzzle4Points, puzzle5Supports, puzzle5Points, 
            puzzle6Supports, puzzle6Points, puzzle7Supports, puzzle7Points, puzzle8Supports, puzzle8Points);
        string puzzleDataPath = Application.persistentDataPath + "/puzzle.save";
        FileStream fileStream = new FileStream(puzzleDataPath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, puzzleData);
        fileStream.Close(); 
    }

    public static PuzzleData LoadPuzzleData()
    {
        string puzzleDataPath = Application.persistentDataPath + "/puzzle.save";

        if(File.Exists(puzzleDataPath))
        {
            FileStream fileStream = new FileStream(puzzleDataPath, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            PuzzleData puzzleData = (PuzzleData) binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return puzzleData; 
        }
        else
        {
            Debug.Log("No se encontró el archivo de estado de los puzzles guardado.");
            return null;
        }
    }

    public static void ResetPuzzleData()
    {
        PuzzleData puzzleData = new PuzzleData();
        string puzzleDataPath = Application.persistentDataPath + "/puzzle.save";
        FileStream fileStream = new FileStream(puzzleDataPath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, puzzleData);
        fileStream.Close();
    }
}
