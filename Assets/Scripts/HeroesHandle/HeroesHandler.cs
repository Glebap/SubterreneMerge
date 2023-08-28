using System;
using DG.Tweening;
using UnityEngine;
using Zenject;


public class HeroesHandler : IDataPersistence
{
	[Inject] private Board _board;
	[Inject] private HeroesSpawner _heroesSpawner;
	[Inject] private HeroesPoolHandler _heroesPoolHandler;
	[Inject] private HeroesMergeHandler _heroesMergeHandler;
	[Inject] private HeroesHandleConfig _heroesHandleConfig;
	[Inject] private HeroesRestoreManager _heroesRestoreManager;
	[Inject] private DataPersistenceManager _dataPersistenceManager;
	[Inject] private BoardHeroesMover _boardHeroesMover;

	private Sequence _highlightSequence;

	public event Action HeroesRestored;
	public event Action<BoardUnit> HeroDropped;
	public event Action<HeroesMergeData> HeroesMerged;
	public event Action<HeroEliminateData> HeroEliminated;
	

	public void TryDropHeroFromPool(int column)
	{
		if (!_board.TryGetEmptyBoardUnitInColumn(column, out var boardUnit)) return;

		_heroesPoolHandler.DropHeroFromPool(boardUnit, column);

		HeroDropped?.Invoke(boardUnit);
		
		_heroesRestoreManager.UpdateRestoreHeroData(boardUnit.GridPosition);
		_dataPersistenceManager.SaveGame();
	}
	
	public void TryBackHeroToPool(HeroDropData heroDropData)
	{
		_heroesPoolHandler.BackHeroToPool(heroDropData);
		
		_heroesRestoreManager.UpdateRestoreHeroData(heroDropData.DropBoardUnit.GridPosition);
		_heroesRestoreManager.UpdatePoolRestoreData();
		_dataPersistenceManager.SaveGame();
	}

	public void MergeHeroes(BoardUnit selectedBoardUnit, BoardUnit nearBoardUnit)
	{
		var dropDuration = _heroesHandleConfig.BoardDropDuration;
		var dropDelay = _heroesHandleConfig.MergeDuration * 0.5f;
		
		_heroesMergeHandler.MergeHeroes(selectedBoardUnit, nearBoardUnit, out var mergeData);
		_boardHeroesMover.DropHeroes(selectedBoardUnit.GridPosition, dropDuration, dropDelay);
		
		HeroesMerged?.Invoke(mergeData);
		
		_heroesRestoreManager.UpdateRestoreDataColumn(selectedBoardUnit.GridPosition.x);
		_heroesRestoreManager.UpdateRestoreHeroData(nearBoardUnit.GridPosition);
		_dataPersistenceManager.SaveGame();
	}
	
	public void UndoHeroesMerge(HeroesMergeData heroesMergeData)
	{
		var origin = heroesMergeData.SelectedBoardUnit.GridPosition;
		var raiseDuration = _heroesHandleConfig.BoardDropDuration * 0.5f;
		
		_boardHeroesMover.RaiseHeroes(origin, raiseDuration);
		_heroesMergeHandler.UndoHeroesMerge(heroesMergeData);
		
		_heroesRestoreManager.UpdateRestoreDataColumn(origin.x);
		_heroesRestoreManager.UpdateRestoreHeroData(heroesMergeData.NearBoardUnit.GridPosition);
		_dataPersistenceManager.SaveGame();
	}

	public void ToggleAllHeroesHighlight(bool value)
	{
		float scaleValue = value ? 1.12f : 1.0f;
		float scaleDuration = value ? 0.36f : 0.12f;
		int loops = value ? -1 : 0;
		
		_highlightSequence.Kill();
		_highlightSequence = DOTween.Sequence();

		foreach (var hero in _board.GetHeroes())
			_highlightSequence.Join(hero.transform.DOScale(Vector3.one * scaleValue, scaleDuration));
		
		_highlightSequence.Play().SetLoops(loops, LoopType.Yoyo);
	}

	public void EliminateHero(BoardUnit boardUnit)
	{
		if (boardUnit.IsEmpty) return;

		var heroEliminateData = new HeroEliminateData(boardUnit);
		
		_board.DestroyHeroAtBoardUnit(boardUnit);
		_boardHeroesMover.DropHeroes(boardUnit.GridPosition, _heroesHandleConfig.BoardDropDuration, 0.12f);

		HeroEliminated?.Invoke(heroEliminateData);
		
		_heroesRestoreManager.UpdateRestoreDataColumn(boardUnit.GridPosition.x);
		_dataPersistenceManager.SaveGame();
	}
	
	public void RespawnHero(HeroEliminateData heroEliminateData)
	{
		var boardUnit = heroEliminateData.BoardUnit;
		var spawnPoint = (Vector2)boardUnit.transform.position;
		var hero = _heroesSpawner.SpawnHero(heroEliminateData.HeroData, spawnPoint, 0.12f, 0.12f);
		
		_boardHeroesMover.RaiseHeroes(boardUnit.GridPosition, 0.12f);
		boardUnit.SetHero(hero);

		_heroesRestoreManager.UpdateRestoreDataColumn(boardUnit.GridPosition.x);
		_dataPersistenceManager.SaveGame();
	}


	public void LoadData(GameData data)
	{
		_heroesRestoreManager.TryRestoreBoard(data.BoardHeroesRestoreData);
		_heroesRestoreManager.TryRestorePool(data.PoolHeroesRestoreData);

		HeroesRestored?.Invoke();
	}

	public void SaveData(GameData data)
	{
		data.BoardHeroesRestoreData = _heroesRestoreManager.BoardHeroesRestoreData;
		data.PoolHeroesRestoreData = _heroesRestoreManager.PoolHeroesRestoreData;
	}
}