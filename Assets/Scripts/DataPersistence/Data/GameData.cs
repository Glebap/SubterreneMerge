[System.Serializable]
public class GameData
{
	public HeroRestoreData[] BoardHeroesRestoreData;
	public HeroRestoreData[] PoolHeroesRestoreData;
	public HeroRestoreData[] RacesMaxLevelHeroRestoreData;
	

	public GameData()
	{
		BoardHeroesRestoreData = null;
		PoolHeroesRestoreData = null;
		RacesMaxLevelHeroRestoreData = null;
	}
}	
