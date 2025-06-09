using UnityEngine;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

public static class SaveManager
{
    private static readonly string keyDataPath = Application.persistentDataPath + "/configuration.dat";
    private static string encryptionKey;
    private static readonly int fixedSize = 128;

    // Método para guardar los datos del jugador
    public static void SavePlayerData(PlayerData playerData)
    {
        try
        {
            string json = JsonUtility.ToJson(playerData);
            string encryptedJson = Encrypt(json, encryptionKey);
            string playerDataPath = Application.persistentDataPath + "/player.sav";
            File.WriteAllText(playerDataPath, encryptedJson);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar los datos del jugador: " + ex.Message);
        }
    }

    // Método para cargar los datos del jugador
    public static PlayerData LoadPlayerData()
    {
        string playerDataPath = Application.persistentDataPath + "/player.sav";

        try
        {
            if(File.Exists(playerDataPath))
            {
                string encryptedJson = File.ReadAllText(playerDataPath);
                string json = Decrypt(encryptedJson, encryptionKey);
                return JsonUtility.FromJson<PlayerData>(json);
            }
            else
            {
                Debug.Log("No existe el archivo del estado del jugador.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al cargar los datos del jugador: " + ex.Message);
            return null;
        }
    }

    // Método para reiniciar los datos del jugador
    public static void ResetPlayerData()
    {
        try
        {
            PlayerData playerData = new PlayerData();
            string json = JsonUtility.ToJson(playerData);
            string encryptedJson = Encrypt(json, encryptionKey);
            string playerDataPath = Application.persistentDataPath + "/player.sav";
            File.WriteAllText(playerDataPath, encryptedJson);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al reiniciar los datos del jugador: " + ex.Message);
        }
    }

    // Método para guardar los datos del juego
    public static void SaveGameData(GameData gameData)
    {
        try
        {
            string json = JsonUtility.ToJson(gameData);
            string encryptedJson = Encrypt(json, encryptionKey);
            string gameDataPath = Application.persistentDataPath + "/game.sav";
            File.WriteAllText(gameDataPath, encryptedJson);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar los datos del juego: " + ex.Message);
        }
    }

    // Método para cargar los datos del juego
    public static GameData LoadGameData()
    {
        string gameDataPath = Application.persistentDataPath + "/game.sav";

        try
        {
            if(File.Exists(gameDataPath))
            {
                string encryptedJson = File.ReadAllText(gameDataPath);
                string json = Decrypt(encryptedJson, encryptionKey);
                return JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                Debug.Log("No existe el archivo del estado del juego.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al cargar los datos del juego: " + ex.Message);
            return null;
        }
    }

    // Método para reiniciar los datos del juego
    public static void ResetGameData()
    {
        try
        {
            GameData gameData = new GameData();
            string json = JsonUtility.ToJson(gameData);
            string encryptedJson = Encrypt(json, encryptionKey);
            string gameDataPath = Application.persistentDataPath + "/game.sav";
            File.WriteAllText(gameDataPath, encryptedJson);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al reiniciar los datos del juego: " + ex.Message);
        }
    }

    // Método para guardar los datos de los puzles
    public static void SavePuzzleData(PuzzleData puzzleData)
    {
        try
        {
            string json = JsonUtility.ToJson(puzzleData);
            string encryptedJson = Encrypt(json, encryptionKey);
            string puzzleDataPath = Application.persistentDataPath + "/puzzle.sav";
            File.WriteAllText(puzzleDataPath, encryptedJson);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar los datos de los puzles: " + ex.Message);
        }
    }

    // Método para cargar los datos de los puzles
    public static PuzzleData LoadPuzzleData()
    {
        string puzzleDataPath = Application.persistentDataPath + "/puzzle.sav";

        try
        {
            if(File.Exists(puzzleDataPath))
            {
                string encryptedJson = File.ReadAllText(puzzleDataPath);
                string json = Decrypt(encryptedJson, encryptionKey);
                return JsonUtility.FromJson<PuzzleData>(json);
            }
            else
            {
                Debug.Log("No existe el archivo del estado de los puzles.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al cargar los datos de los puzles: " + ex.Message);
            return null;
        }
    }

    // Método para reiniciar los datos de los puzles
    public static void ResetPuzzleData()
    {
        try
        {
            PuzzleData puzzleData = new PuzzleData();
            string json = JsonUtility.ToJson(puzzleData);
            string encryptedJson = Encrypt(json, encryptionKey);
            string puzzleDataPath = Application.persistentDataPath + "/puzzle.sav";
            File.WriteAllText(puzzleDataPath, encryptedJson);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar los datos de los puzles: " + ex.Message);
        }
    }

    // Método para generar una clave de encriptación al comenzar el juego
    public static void GenerateKey()
    {
        string masterKey = GetMasterKey();
        GetOrCreateKey(masterKey);        
    }

    // Método para generar la clave maestra a partir del identificador del dispositivo
    private static string GetMasterKey()
    {
        // Se obtiene el deviceUniqueIdentifier (UUID) proporcionado por Unity
        string deviceUUID = SystemInfo.deviceUniqueIdentifier;

        // Se convierte el UUID en una clave segura usando SHA-256
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(deviceUUID));

            byte[] aesKey = new byte[16];
            Array.Copy(hashBytes, aesKey, 16);

            return Convert.ToBase64String(aesKey);
        }
    }

    // Método para generar o recuperar la clave de encriptación
    private static void GetOrCreateKey(string masterKey)
    {
        try
        {
            if (File.Exists(keyDataPath))
            {
                string encryptedKey = File.ReadAllText(keyDataPath);
                encryptionKey = Decrypt(encryptedKey, masterKey);
            }
            else
            {
                encryptionKey = GenerateRandomKey();
                string encryptedKey = Encrypt(encryptionKey, masterKey);
                File.WriteAllText(keyDataPath, encryptedKey);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al manejar la clave de encriptación: " + ex.Message);
        }
    }

    // Método para generar una clave aleatoria de 16 bytes
    private static string GenerateRandomKey()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] keyBytes = new byte[16];
            rng.GetBytes(keyBytes);
            return Convert.ToBase64String(keyBytes);
        }
    }

    // Método para cifrar un texto usando el algoritmo AES
    private static string Encrypt(string text, string key)
    {
        using (Aes aes = Aes.Create())
        {
            // Se convierte la clave a bytes
            aes.Key = Encoding.UTF8.GetBytes(key);

            // Genera un vector de inicialización (IV) aleatorio
            aes.GenerateIV();

            // Se crea el cifrador
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            
            // Se convierte el texto a bytes
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            
            // Se obtiene el tamaño del texto
            int inputSize = inputBytes.Length;

            // Si el mensaje es menor a un tamaño establecido se añade relleno y un marcador. 
            // Si es mayor, solo se añade un marcador al principio
            if (inputSize >= fixedSize)
            {
                byte[] markedMessage = Encoding.UTF8.GetBytes("[NO_PADDING]").Concat(inputBytes).ToArray();
                inputBytes = markedMessage;
            }
            else
            {
                int paddingSize = fixedSize - inputSize - 3;
                byte[] padding = new byte[paddingSize];
                new RNGCryptoServiceProvider().GetBytes(padding);
                inputBytes = padding.Concat(Encoding.UTF8.GetBytes("END")).Concat(inputBytes).ToArray();        
            }

            // Se cifra los datos de entrada
            byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            // Se concatena el IV con los datos cifrados
            byte[] result = new byte[aes.IV.Length + encryptedBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

            // Se convierte el resultado a Base64 para transferirlo
            return Convert.ToBase64String(result); 
        }
    }

    // Método para descifrar un texto usando el algoritmo AES
    private static string Decrypt(string encryptedText, string key)
    {
        // Se convierte el texto Base64 en bytes
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText); 

        using (Aes aes = Aes.Create())
        {
            // Se emplea la misma clave que se usa para cifrar
            aes.Key = Encoding.UTF8.GetBytes(key);

            // Se extrae el IV de los primeros 16 bytes del arreglo cifrado
            byte[] iv = new byte[16];
            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);

            // Se extrae los datos cifrados (sin el IV) del arreglo cifrado
            byte[] cipherText = new byte[encryptedBytes.Length - iv.Length];
            Buffer.BlockCopy(encryptedBytes, iv.Length, cipherText, 0, cipherText.Length);

            // Se crea el descifrador con la clave y el IV extraídos
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);

            // Se descifra los datos cifrados
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

            // Se convierte los bytes descifrados a una cadena de texto
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

            // Si el mensaje comienza con el marcador [NO_PADDING], se elimina ese marcador
            if (decryptedText.StartsWith("[NO_PADDING]"))
            {
                return decryptedText.Substring(12);
            }

            // Si el mensaje no coiemza con el marcador [NO_PADDING], se busca el marcador END y se quita el texto anterior
            int endIndex = decryptedText.IndexOf("END");
            if (endIndex != -1)
            {
                return decryptedText.Substring(endIndex + 3);
            }

            return decryptedText; 
        }
    }
}
