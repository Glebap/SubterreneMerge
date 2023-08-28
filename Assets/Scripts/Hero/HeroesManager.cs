using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;


public class HeroesManager : IDataPersistence, IDisposable
{
	private HeroesHandler _heroesHandler;
	private HeroRacesUnlockData _heroRacesUnlockData;
	private ParticleManager _particleManager;

	private static Dictionary<HeroRace, HeroesTierList> _heroesTierLists;
	private static Dictionary<HeroRace, int> _racesMaxLevelHero;

	private HeroRestoreData[] _racesMaxLevelHeroRestoreData;

	public readonly PlayerPrefIntValue MaxHeroLevel = new (
		key : "MaxHeroLevel", 
		defaultValue: 1, 
		valuePreprocessors: v => Mathf.Max(1, v)
		);

	public event Action<HeroData> NewHeroUnlocked;
	public event Action NewMaxLevelReached;
	public event Action<HeroRace> RaceMaxLevelDecreased;
	
	
	[Inject]
	public void Construct(HeroesHandler heroesHandler, HeroRacesUnlockData heroRacesUnlockData, ParticleManager particleManager)
	{
		_heroesHandler = heroesHandler;
		_heroRacesUnlockData = heroRacesUnlockData;
		_particleManager = particleManager;
	
		InitializeHeroesTierLists();
		InitializeRacesMaxLevelHero();
		
		_heroesHandler.HeroesMerged += OnHeroesMerged;
	}

	public void Dispose()
	{
		_heroesHandler.HeroesMerged -= OnHeroesMerged;
	}

	public IEnumerable<HeroRaceUnlockData> GetAvailableHeroRaces()
	{
		return _heroRacesUnlockData.Where(heroRace => heroRace.UnlockLevel <= MaxHeroLevel);
	}

	public static HeroData GetHeroData(HeroRestoreData heroRestoreData)
	{
		return _heroesTierLists[(HeroRace)heroRestoreData.Race].GetHeroData(heroRestoreData.Level);
	}

	public static HeroData GetNextLevelHeroData(Hero hero)
	{
		return _heroesTierLists[hero.Race].GetHeroData(hero.Level + 1);
	}
	
	public static HeroData GetPreviousLevelHeroData(Hero hero)
	{
		return _heroesTierLists[hero.Race].GetHeroData(hero.Level - 1);
	}

	public IEnumerable<RaceUnlockedHeroesData> GetRacesUnlockedHeroesData()
	{
		return _heroRacesUnlockData.Select(heroRaceUnlockData 
			=> new RaceUnlockedHeroesData(heroRaceUnlockData, _racesMaxLevelHero[heroRaceUnlockData.HeroesTierList.HeroesRace]));
	}

	private void OnHeroesMerged(HeroesMergeData heroesMergeData)
	{
		var mergedHeroData = heroesMergeData.MergedHeroData;
		var position = heroesMergeData.MergedHero.transform.position;

		if (_racesMaxLevelHero[mergedHeroData.Race] >= mergedHeroData.Level)
		{
			_particleManager.ShowMergeParticle(position);
			return;
		}

		OnNewHeroUnlocked(mergedHeroData);
		_particleManager.ShowNewHeroParticle(position);

		if (MaxHeroLevel < mergedHeroData.Level)
			OnNewMaxLevelReached(mergedHeroData);
	}

	private void OnNewHeroUnlocked(HeroData heroData)
	{
		_racesMaxLevelHero[heroData.Race] = heroData.Level;
		
		UpdateRacesMaxLevelHeroRestoreData();
		GameEventManager.SendHeroUnlockedEvent(heroData);
		
		NewHeroUnlocked?.Invoke(heroData);
	}
	
	private void OnNewMaxLevelReached(HeroData heroData)
	{
		MaxHeroLevel.Value = heroData.Level;
		NewMaxLevelReached?.Invoke();
	}

	public void UndoHeroUnlock(HeroData heroData)
	{
		_racesMaxLevelHero[heroData.Race] = heroData.Level - 1;
		UpdateRacesMaxLevelHeroRestoreData();
		RaceMaxLevelDecreased?.Invoke(heroData.Race);
	}

	public void DecreaseMaxHeroLevel()
	{
		MaxHeroLevel.Value--;
		NewMaxLevelReached?.Invoke();
	}

	private void InitializeHeroesTierLists()
	{
		_heroesTierLists = new Dictionary<HeroRace, HeroesTierList>();
		
		foreach (var heroRacesUnlockData in _heroRacesUnlockData)
		{
			var heroesTierList = heroRacesUnlockData.HeroesTierList;
			_heroesTierLists[heroesTierList.HeroesRace] = heroRacesUnlockData.HeroesTierList;
		}
	}

	private void UpdateRacesMaxLevelHeroRestoreData()
	{
		_racesMaxLevelHeroRestoreData = _racesMaxLevelHero.Select(pair => new HeroRestoreData(pair.Key, pair.Value)).ToArray();
	}
	
	private void InitializeRacesMaxLevelHero()
	{
		_racesMaxLevelHero = new Dictionary<HeroRace, int>();
		
		foreach (var heroRacesUnlockData in _heroRacesUnlockData)
			_racesMaxLevelHero[heroRacesUnlockData.HeroesTierList.HeroesRace] = 1;
	}
	
	private void RestoreRacesMaxLevelHero(HeroRestoreData[] racesMaxLevelHeroRestoreData)
	{
		_racesMaxLevelHero = new Dictionary<HeroRace, int>();
		
		foreach (var raceMaxLevelHeroRestoreData in racesMaxLevelHeroRestoreData)
			_racesMaxLevelHero[(HeroRace)raceMaxLevelHeroRestoreData.Race] = raceMaxLevelHeroRestoreData.Level;
	}

	public void LoadData(GameData data)
	{
		_racesMaxLevelHeroRestoreData = data.RacesMaxLevelHeroRestoreData;
		if (_racesMaxLevelHeroRestoreData is { Length: > 0 })
			RestoreRacesMaxLevelHero(_racesMaxLevelHeroRestoreData);
		else
			InitializeRacesMaxLevelHero();
	}

	public void SaveData(GameData data)
	{
		data.RacesMaxLevelHeroRestoreData = _racesMaxLevelHeroRestoreData;
	}
}
