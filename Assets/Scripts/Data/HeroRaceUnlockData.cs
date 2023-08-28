using UnityEngine;


[System.Serializable]
public class HeroRaceUnlockData
{
	[SerializeField] private HeroesTierList _heroesTierList;
	[SerializeField] private int _unlockLevel;
	
	public HeroesTierList HeroesTierList => _heroesTierList;
	public int UnlockLevel => _unlockLevel;
}