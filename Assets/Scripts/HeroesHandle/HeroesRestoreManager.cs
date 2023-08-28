using System;
using UnityEngine;
using Zenject;


public class HeroesRestoreManager : IDisposable
{
	private Board _board;
	private HeroesPool _heroesPool;
	private HeroesSpawner _heroesSpawner;

	public HeroRestoreData[] BoardHeroesRestoreData { get; private set; }
	public HeroRestoreData[] PoolHeroesRestoreData { get; private set; }

	[Inject]
	private void Construct(Board board, HeroesPool heroesPool, HeroesSpawner heroesSpawner)
	{
		_board = board;
		_heroesPool = heroesPool;
		_heroesSpawner = heroesSpawner;
		
		_heroesPool.PoolUpdated += OnPoolUpdated;
		_board.BoardCleared += OnBoardCleared;
	}
	
	public void Dispose()
	{
		_heroesPool.PoolUpdated -= OnPoolUpdated;
		_board.BoardCleared -= OnBoardCleared;
	}

	public void UpdateRestoreHeroData(Vector2Int origin)
	{
		if (!_board.TryGetBoardUnit(origin.x, origin.y, out var boardUnit)) 
			return;
		
		int index = origin.y * _board.Width + origin.x;

		BoardHeroesRestoreData[index] = boardUnit.IsEmpty
			? null : new HeroRestoreData(boardUnit.ContainedHero);
	}
	
	public void UpdatePoolRestoreData()
	{
		var data = _heroesPool.CurrentHeroesData;
		
		for (var index = 0; index < _board.Width; index++)
			PoolHeroesRestoreData[index] = new HeroRestoreData(data[index]);
	}

	public void UpdateRestoreDataColumn(int column)
	{
		for (int row = 0; row < _board.Height; row++)
			UpdateRestoreHeroData(new Vector2Int(column, row));
	}
	
	public void TryRestoreBoard(HeroRestoreData[] heroesRestoreData)
	{
		PoolHeroesRestoreData = new HeroRestoreData[_board.Width];
		
		if (heroesRestoreData is not { Length: > 0 })
		{
			ResetHeroesRestoreData();
			return;
		}

		for (var index = 0; index < heroesRestoreData.Length; index++)
		{
			if (heroesRestoreData[index].Level == 0) continue;
			
			int column = index % _board.Width;
			int row = index / _board.Width;

			if (!_board.TryGetBoardUnit(column, row, out var boardUnit)) 
				continue;
			
			var heroData = HeroesManager.GetHeroData(heroesRestoreData[index]);
			var hero = _heroesSpawner.SpawnHero(heroData, boardUnit.transform.position);
			
			boardUnit.SetHero(hero);
		}

		BoardHeroesRestoreData = heroesRestoreData;
	}
	
	public void TryRestorePool(HeroRestoreData[] heroesRestoreData)
	{
		PoolHeroesRestoreData = new HeroRestoreData[_board.Width];
		
		if (heroesRestoreData is not { Length: > 0 })
		{
			_heroesPool.UpdatePool();
			return;
		}
		
		_heroesPool.UpdatePool(heroesRestoreData);
	}
	
	private void ResetHeroesRestoreData()
	{
		BoardHeroesRestoreData = new HeroRestoreData[_board.Width * _board.Height];
		GameEventManager.SendLevelStartedEvent();
	}
	
	private void OnPoolUpdated()
	{
		UpdatePoolRestoreData();
	}

	private void OnBoardCleared()
	{
		ResetHeroesRestoreData();
	}
}