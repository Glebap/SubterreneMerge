using System;
using NaughtyAttributes;
using UnityEngine;


public class Hero : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private bool _showLevel;
    [SerializeField, ShowIf("_showLevel")] private LevelView _levelView;

    public Sprite Sprite => _spriteRenderer.sprite;
    
    public HeroRace Race { get; private set; }
    public int Level { get; private set; }


    public void Initialize(HeroData heroData)
    {
        Race = heroData.Race;
        _spriteRenderer.sprite = heroData.Sprite;
        Level = heroData.Level;

        if (_levelView == null) return;
        
        if (_showLevel) _levelView.Show(Level);
        else _levelView.Hide();
    }

    public override bool Equals(object obj)
    {
        if (obj is not Hero otherHero)
            return false;

        return Race == otherHero.Race && Level == otherHero.Level;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), _showLevel, _levelView, (int)Race, Level);
    }
}
