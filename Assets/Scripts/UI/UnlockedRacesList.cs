using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UnlockedRacesList : MonoBehaviour
{
    [SerializeField] private Transform _unlockedRacesListContent;
    [SerializeField] private UnlockedHeroesView _unlockedHeroesViewPrefab;
    [SerializeField] private PopUp _newLevelUnlockedPopUp;

    [Inject] private HeroesManager _heroesManager;
    
    private IEnumerable<RaceUnlockedHeroesData> _racesUnlockedHeroesData;
    private Dictionary<HeroRace, UnlockedHeroesView> _unlockedHeroesViews;

    private void OnEnable()
    {
        _heroesManager.NewHeroUnlocked += OnNewHeroUnlocked;
        _heroesManager.NewMaxLevelReached += OnNewMaxLevelReached;
        _heroesManager.RaceMaxLevelDecreased += OnRaceMaxLevelDecreased;
    }

    private void OnDisable()
    {
        _heroesManager.NewHeroUnlocked -= OnNewHeroUnlocked;
        _heroesManager.NewMaxLevelReached -= OnNewMaxLevelReached;
        _heroesManager.RaceMaxLevelDecreased -= OnRaceMaxLevelDecreased;
    }
    
    private void Start()
    {
        _racesUnlockedHeroesData = _heroesManager.GetRacesUnlockedHeroesData();
        Initialize();
    }

    private void Initialize()
    {
        var maxHeroLevel = _heroesManager.MaxHeroLevel;
        
        _unlockedHeroesViews = new Dictionary<HeroRace, UnlockedHeroesView>();
        foreach (var raceUnlockedHeroesData in _racesUnlockedHeroesData)
        {
            var unlockedHeroesView = Instantiate(_unlockedHeroesViewPrefab, _unlockedRacesListContent);
            var race = raceUnlockedHeroesData.Race;

            if (raceUnlockedHeroesData.UnlockLevel <= maxHeroLevel)
                unlockedHeroesView.Initialize(raceUnlockedHeroesData);
            else
                unlockedHeroesView.Initialize(race, raceUnlockedHeroesData.UnlockLevel);
            
            _unlockedHeroesViews[race] = unlockedHeroesView;
        }
    }

    private void UpdateUnlockedRaces()
    {
        var maxHeroLevel = _heroesManager.MaxHeroLevel;
        foreach (var raceUnlockedHeroesData in _racesUnlockedHeroesData)
        {
            if (raceUnlockedHeroesData.UnlockLevel <= maxHeroLevel && _unlockedHeroesViews[raceUnlockedHeroesData.Race].Locked)
                _unlockedHeroesViews[raceUnlockedHeroesData.Race].Initialize(raceUnlockedHeroesData);
        }
    }

    private void OnNewHeroUnlocked(HeroData heroData)
    {
        _unlockedHeroesViews[heroData.Race].AddNewHero(heroData);
        _newLevelUnlockedPopUp.Show();
    }

    private void OnRaceMaxLevelDecreased(HeroRace race)
    {
        LockLastHeroInRace(race);
        _newLevelUnlockedPopUp.Hide();
    }
    
    private void LockLastHeroInRace(HeroRace race)
    {
        _unlockedHeroesViews[race].RemoveLastHero();
    }

    private void OnNewMaxLevelReached()
    {
        UpdateUnlockedRaces();
    }

    public void ToggleUnlockedRacesList()
    {
        transform.localScale = transform.localScale == Vector3.zero ? Vector3.one : Vector3.zero;
    }
}
