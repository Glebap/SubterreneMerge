using DG.Tweening;
using Zenject;


public class HeroesPoolHandler
{
	private HeroesPool _heroesPool;
	private float _dropDuration;

	
	[Inject]
	private void Construct(HeroesPool heroesPool, HeroesHandleConfig heroesHandleConfig)
	{
		_heroesPool = heroesPool;
		_dropDuration = heroesHandleConfig.PoolDropDuration;
	}
	
	public void DropHeroFromPool(BoardUnit boardUnit, int column)
	{
		var hero = _heroesPool.GetHero(column);
		
		hero.transform.DOMove(boardUnit.transform.position, _dropDuration);
		boardUnit.SetHero(hero);
		
		_heroesPool.UpdatePool();
	}
	
	public void BackHeroToPool(HeroDropData heroDropData)
	{
		var column = heroDropData.DropColumn;
		var heroesData = heroDropData.HeroesData;
		var hero = heroDropData.DropBoardUnit.ContainedHero;
		var spawnPoint = _heroesPool.GetSpawnPointPosition(column);

		heroesData[column] = null;
		_heroesPool.RestorePool(heroesData);
		heroDropData.DropBoardUnit.Clear();

		_heroesPool.SetHero(hero, column);
		hero.transform.DOMove(spawnPoint, _dropDuration);
	}
}