using UnityEngine;
using System;
using System.IO;


public class FileDataHandler
{
    private readonly string _dataFilePath;
    private readonly bool _useEncryption;
    private const string EncryptionCodeWord = "corrupted";

    
    public FileDataHandler(string dataFileName, bool useEncryption)
    {
        _dataFilePath = Path.Combine(Application.persistentDataPath, dataFileName);
        _useEncryption = useEncryption;
        //Debug.Log(_dataFilePath);
    }

    public GameData Load()
    {
        if (!File.Exists(_dataFilePath)) return null;

        try
        {
            var dataToLoad = File.ReadAllText(_dataFilePath);

            if (_useEncryption)
                dataToLoad = EncryptDecrypt(dataToLoad);

            return JsonUtility.FromJson<GameData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading data from {_dataFilePath}: {e}");
            return null;
        }
    }

    public void Save(GameData data)
    {
        try
        {
            var dataToStore = JsonUtility.ToJson(data, true);

            if (_useEncryption)
                dataToStore = EncryptDecrypt(dataToStore);

            Directory.CreateDirectory(Path.GetDirectoryName(_dataFilePath));
            
            File.WriteAllText(_dataFilePath, dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving data to {_dataFilePath}: {e}");
        }
    }

    private string EncryptDecrypt(string data)
    {
        var modifiedData = "";
        for (var i = 0; i < data.Length; i++)
            modifiedData += (char)(data[i] ^ EncryptionCodeWord[i % EncryptionCodeWord.Length]);

        return modifiedData;
    }
}