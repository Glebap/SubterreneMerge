using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameAnalyticsSDK;
using Zenject;


public class DataPersistenceManager : IInitializable
{
    [Inject] private DiContainer _container;
    [Inject] private DataPersistenceConfig _config;
    
    private GameData _gameData;
    private IEnumerable<IDataPersistence> _dataPersistenceObjects;
    private FileDataHandler _fileDataHandler;
    
    
    public void Initialize()
    {
        _dataPersistenceObjects = new List<IDataPersistence>();
        _fileDataHandler = new FileDataHandler(_config.FileName, _config.UseEncryption);
        _dataPersistenceObjects = _container.ResolveAll<IDataPersistence>();

        if (!Application.isPlaying) return;
        
        GameAnalytics.Initialize();
        LoadGame();
    }
    
    public void NewGame()
    {
        _gameData = new GameData();
        _fileDataHandler.Save(_gameData);
    }

    private void LoadGame()
    {
        _gameData = _fileDataHandler.Load();
        
        if (_gameData == null)
        {
            Debug.Log("No data found");
            NewGame();
        }

        foreach (var dataPersistenceObj in _dataPersistenceObjects)
            dataPersistenceObj.LoadData(_gameData);
    }
    
    public void SaveGame()
    {
        foreach (var dataPersistenceObj in _dataPersistenceObjects)
            dataPersistenceObj.SaveData(_gameData);
        
        _fileDataHandler.Save(_gameData);
    }
}
