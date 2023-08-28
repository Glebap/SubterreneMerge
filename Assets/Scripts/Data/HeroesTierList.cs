using UnityEngine;


[CreateAssetMenu(menuName = "Heroes Tier List")]
public class HeroesTierList : ScriptableObject
{
	[SerializeField] private HeroRace _heroesRace;
	[SerializeField] private Sprite[] _sprites;

	public HeroRace HeroesRace => _heroesRace;
	public Sprite[] Sprites => _sprites;

	
	public HeroData GetHeroData(int level)
	{
		var levelIndex = Mathf.Clamp(level, 1, _sprites.Length) - 1;
		
		return new HeroData(_heroesRace, _sprites[levelIndex], level);
	}
}