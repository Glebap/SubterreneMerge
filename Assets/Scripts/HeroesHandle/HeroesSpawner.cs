using DG.Tweening;
using UnityEngine;


public class HeroesSpawner
{
	private readonly Hero _heroPrefab;
	private readonly Transform _heroesTransform;


	public HeroesSpawner(Hero heroPrefab, Transform heroesTransform)
	{
		_heroPrefab = heroPrefab;
		_heroesTransform = heroesTransform;
	}

	public Hero SpawnHero(HeroData heroData)
	{
		var hero = Object.Instantiate(_heroPrefab, _heroesTransform);
		hero.Initialize(heroData);

		return hero;
	}
	
	public Hero SpawnHero(HeroData heroData, Vector2 position, float duration = 0.0f, float delay = 0.0f)
	{
		if (heroData == null) return null;
		
		var hero = SpawnHero(heroData);
		var heroTransform = hero.transform;
		
		heroTransform.localScale = Vector3.zero;
		heroTransform.localPosition = position;
		heroTransform.DOScale(Vector3.one, duration).SetDelay(delay);

		return hero;
	}
}