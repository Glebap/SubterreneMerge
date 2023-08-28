using UnityEngine;

public class HeroData
{
	public HeroRace Race { get; }
	public Sprite Sprite { get; }
	public int Level { get; }

	public HeroData(HeroRace race, Sprite sprite, int level)
	{
		Race = race;
		Sprite = sprite;
		Level = level;
	}
	
	public HeroData(Hero hero)
		: this(hero.Race, hero.Sprite, hero.Level) { }
}