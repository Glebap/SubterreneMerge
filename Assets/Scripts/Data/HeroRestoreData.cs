[System.Serializable]
public class HeroRestoreData
{
	public int Race;
	public int Level;

	
	public HeroRestoreData(HeroRace race, int level)
	{
		Race = (int)race;
		Level = level;
	}

	public HeroRestoreData(Hero hero) 
		: this(hero.Race, hero.Level) { }

	public HeroRestoreData(HeroData heroData) 
		: this(heroData.Race, heroData.Level) { }
}