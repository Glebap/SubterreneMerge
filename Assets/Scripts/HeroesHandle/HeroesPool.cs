using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


public class HeroesPool : IDisposable
{
    private Board _board;
    private HeroesManager _heroesManager;
    private HeroesSpawner _heroesSpawner;
    
    private float _spawnDuration;
    private float _halfSpawnDuration => _spawnDuration * 0.5f;

    private Dictionary<HeroRace, HeroData> _initialRacesHeroData = new();
    private HeroData[] _currentHeroesData;
    private Hero[] _currentHeroes;
    private Vector2[] _spawnPoints;

    public int Capacity => _board.Width;
    public HeroData[] CurrentHeroesData => _currentHeroesData;

    public event Action PoolUpdated;

    
    [Inject]
    private void Construct(
        Board board, 
        HeroesManager heroesManager,
        HeroesSpawner heroesSpawner,
        HeroesHandleConfig heroesHandleConfig)
    {
        _board = board;
        _heroesSpawner = heroesSpawner;
        _heroesManager = heroesManager;
        _spawnDuration = heroesHandleConfig.PoolSpawnDuration;
        
        SetUpSpawnPoints();
        UpdateInitialRacesHeroData();
        
        _heroesManager.NewMaxLevelReached += OnNewMaxLevelReached;
    }
    
    public void Dispose()
    {
        _heroesManager.NewMaxLevelReached -= OnNewMaxLevelReached;
    }

    public void UpdatePool(HeroData[] heroesData = null)
    {
        SetHeroesToPool(heroesData);
        
        PoolUpdated?.Invoke();
    }

    public void UpdatePool(IEnumerable<HeroRestoreData> heroesRestoreData)
    {
        UpdatePool(heroesRestoreData.Select(HeroesManager.GetHeroData));
    }
    
    public void UpdatePool(IEnumerable<HeroData> heroesRestoreData)
    {
        UpdatePool(heroesRestoreData.ToArray());
    }

    public void RestorePool(HeroData[] heroesData)
    {
        SetHeroesToPool(heroesData, false);
    }

    public Hero GetHero(int row)
    {
        var hero = _currentHeroes[row];
        _currentHeroes[row] = null;

        return hero;
    }
    
    public void SetHero(Hero hero, int row)
    {
        _currentHeroes[row] = hero;
        _currentHeroesData[row] = new HeroData(hero);
    }

    public Vector2 GetSpawnPointPosition(int column)
    {
        return _spawnPoints[column];
    }

    private void OnNewMaxLevelReached()
    {
        UpdateInitialRacesHeroData();
        UpdatePool();
    }

    private void SetUpSpawnPoints()
    {
        _currentHeroes = new Hero[Capacity];
        _spawnPoints = new Vector2[Capacity];

        var boardPosition = _board.Transform.position;
        var positionY = boardPosition.y + _board.TrueSize.y;
        var positionX = boardPosition.x;

        for (var count = 0; count < Capacity; count++)
        {
            _spawnPoints[count] = new Vector2(positionX, positionY);
            positionX += _board.BoardUnitSize.x;
        }
    }

    private void UpdateInitialRacesHeroData()
    {
        const int level = 1;
        var initialRacesHeroData = new Dictionary<HeroRace, HeroData>();

        foreach (var heroesRaceData in _heroesManager.GetAvailableHeroRaces())
        {
            var heroTierList = heroesRaceData.HeroesTierList;
            initialRacesHeroData[heroTierList.HeroesRace] = heroTierList.GetHeroData(level);
        }
        
        if (initialRacesHeroData.Count == _initialRacesHeroData.Count) return;
        
        _initialRacesHeroData = initialRacesHeroData;
    }

    private void SetHeroesToPool(HeroData[] heroesData, bool fillEmpty = true)
    {
        _currentHeroesData = heroesData == null 
            ? GetRandomHeroesData(Capacity).ToArray() 
            : GetInitialHeroesData(heroesData, fillEmpty).ToArray();
        
        for (var index = 0; index < Capacity; index++)
        {
            var currentHeroData = _currentHeroesData[index];

            if (currentHeroData == null)
            {
                _currentHeroes[index].Destroy(_halfSpawnDuration);
                continue;
            }

            SpawnHero(currentHeroData, index);
        }
    }

    private void SpawnHero(HeroData heroData, int spawnPointIndex)
    {
        ref var currentHero = ref _currentHeroes[spawnPointIndex];
        
        if (currentHero == null)
            currentHero = _heroesSpawner.SpawnHero(heroData, _spawnPoints[spawnPointIndex], _halfSpawnDuration, _halfSpawnDuration);
        else
            currentHero.Reinitialize(heroData, _spawnDuration);
    }

    private IEnumerable<HeroData> GetRandomHeroesData(int count)
    {
        for (var n = 0; n < Mathf.Max(count, 0); n++)
            yield return GetRandomHeroData();
    }
    
    private HeroData GetRandomHeroData()
    {
        return _initialRacesHeroData[(HeroRace)Random.Range(0, _initialRacesHeroData.Count)];
    }

    private IEnumerable<HeroData> GetInitialHeroesData(HeroData[] heroesData, bool fillEmpty = true)
    {
        return heroesData.Select(heroData => heroData?.Race != null 
            ? _initialRacesHeroData[heroData.Race] 
            : fillEmpty ? GetRandomHeroData() : null);
    }
}
