using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Hero Races Unlock Data", fileName = "HeroRacesUnlockData")]
public class HeroRacesUnlockData : ScriptableObject, IEnumerable<HeroRaceUnlockData>
{
	[SerializeField] private HeroRaceUnlockData[] _data;

	
	public IEnumerator<HeroRaceUnlockData> GetEnumerator()
	{
		return ((IEnumerable<HeroRaceUnlockData>)_data).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _data.GetEnumerator();
	}
}