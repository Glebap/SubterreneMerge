using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UnlockedHeroesView : MonoBehaviour
{
    [SerializeField] private Transform _scrollViewContent;
    [SerializeField] private Image _heroImagePrefab; 
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _locked;
    [SerializeField] private bool _showLevel;
    [SerializeField, ShowIf("_showLevel")] private LevelView _levelViewPrefab;

    public bool Locked { get; private set; }

    public void Initialize(RaceUnlockedHeroesData raceUnlockedHeroesData)
    {
        _title.text = raceUnlockedHeroesData.Race.ToString();
        _locked.enabled = false;
        Locked = false;
        
        var level = 1;
        foreach (var heroSprite in raceUnlockedHeroesData.HeroesSprites)
            AddNewHero(heroSprite, level++);
    }
    
    public void Initialize(HeroRace race, int levelToUnlock)
    {
        _title.text = race.ToString();
        _locked.enabled = true;
        Locked = true;
        _locked.text = $"Reach lvl {levelToUnlock} of any race to unlock";
    }

    public void AddNewHero(HeroData heroData)
    {
        AddNewHero(heroData.Sprite, heroData.Level);
    }

    private void AddNewHero(Sprite heroSprite, int level)
    {
        _heroImagePrefab.sprite = heroSprite;
        var newHero = Instantiate(_heroImagePrefab, _scrollViewContent);
        
        if (_showLevel)
            Instantiate(_levelViewPrefab, newHero.transform).Show(level);
    }

    public void RemoveLastHero()
    {
        Destroy(_scrollViewContent.GetChild(_scrollViewContent.childCount - 1).gameObject);
    }
}
