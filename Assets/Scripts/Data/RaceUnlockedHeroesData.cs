using System.Collections.Generic;
using UnityEngine;


public struct RaceUnlockedHeroesData
{
	public int UnlockLevel { get; }
	public HeroRace Race { get; }
	public IEnumerable<Sprite> HeroesSprites { get; }

	
	public RaceUnlockedHeroesData(HeroRaceUnlockData heroRaceUnlockData, int maxLevel)
	{
		UnlockLevel = heroRaceUnlockData.UnlockLevel;
		Race = heroRaceUnlockData.HeroesTierList.HeroesRace;
		HeroesSprites = heroRaceUnlockData.HeroesTierList.Sprites[..maxLevel];
	}
} 