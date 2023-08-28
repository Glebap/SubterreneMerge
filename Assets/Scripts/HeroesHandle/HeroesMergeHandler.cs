using DG.Tweening;
using Zenject;


public class HeroesMergeHandler
{
	private HeroesSpawner _heroesSpawner;
	private float _mergeDuration;
	private float _halfMergeDuration;
	
	[Inject]
	private void Construct(HeroesSpawner heroesSpawner, HeroesHandleConfig heroesHandleConfig)
	{
		_heroesSpawner = heroesSpawner;
		_mergeDuration = heroesHandleConfig.MergeDuration;
		_halfMergeDuration = _mergeDuration * 0.5f;
	}
	
	public void MergeHeroes(BoardUnit selectedBoardUnit, BoardUnit nearBoardUnit, out HeroesMergeData mergeData)
	{
		var selectedHero = selectedBoardUnit.ContainedHero;
		var nearHero = nearBoardUnit.ContainedHero;
		var nextLevelHeroData = HeroesManager.GetNextLevelHeroData(nearHero);
		var goalPosition = nearHero.transform.position;

		mergeData = new HeroesMergeData(selectedBoardUnit, nearBoardUnit, nextLevelHeroData);

		selectedHero.transform.DOMove(goalPosition, _halfMergeDuration);
		selectedHero.Destroy(_halfMergeDuration);
		selectedBoardUnit.Clear();

		nearHero.Reinitialize(nextLevelHeroData, _mergeDuration);

	}

	public void UndoHeroesMerge(HeroesMergeData heroesMergeData)
	{
		var nearHero = heroesMergeData.NearBoardUnit.ContainedHero;
		var previousLevelHeroData = HeroesManager.GetPreviousLevelHeroData(nearHero);
		var spawnPoint = nearHero.transform.position;
		
		var selectedHero = _heroesSpawner.SpawnHero(previousLevelHeroData, spawnPoint, _halfMergeDuration, _halfMergeDuration);
		selectedHero.transform.DOMove(heroesMergeData.SelectedBoardUnit.transform.position, _halfMergeDuration).SetDelay(_halfMergeDuration);
		heroesMergeData.SelectedBoardUnit.SetHero(selectedHero);
			
		nearHero.Reinitialize(previousLevelHeroData, _mergeDuration);
		
	}
}