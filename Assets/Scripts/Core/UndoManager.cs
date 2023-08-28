using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class UndoManager : MonoBehaviour
{
	[SerializeField] private Button _undoButton;
	[SerializeField] private BoostersManager _boostersManager;
	
	[Inject] private HeroesPool _heroesPool;
	[Inject] private HeroesHandler _heroesHandler;
	[Inject] private HeroesManager _heroesManager;
	[Inject] private DataPersistenceManager _dataPersistenceManager;
	
	private LastMoveType _lastMove = LastMoveType.Undo;
	private HeroData[] _lastHeroesData;
	private HeroData[] _currentHeroesData;
	private HeroesMergeData _lastHeroesMergeData;
	private BoardUnit _lastDroppedHeroBoardUnit;
	private HeroEliminateData _heroEliminateData;
	
	private bool _newMaxLevelWasReached;
	private bool _newHeroWasUnlocked;

	private HeroDropData LastHeroDropData => new (_lastHeroesData, _lastDroppedHeroBoardUnit);

	
	private void Awake()
	{
		_currentHeroesData = new HeroData[_heroesPool.Capacity];
		_lastHeroesData = new HeroData[_heroesPool.Capacity];
		
		SetUndoButtonActive(false);
	}

	private void OnEnable()
	{
		_heroesPool.PoolUpdated += OnPoolUpdated;
		_heroesHandler.HeroesMerged += OnHeroesMerged;
		_heroesHandler.HeroDropped += OnHeroDropped;
		_heroesHandler.HeroEliminated += OnHeroEliminated;
		_boostersManager.BoostedUsed += OnBoosterUsed;
		_heroesManager.NewHeroUnlocked += OnNewHeroUnlocked;
		_heroesManager.NewMaxLevelReached += OnNewMaxLevelReached;
	}

	private void OnHeroEliminated(HeroEliminateData eliminateData)
	{
		_heroEliminateData = eliminateData;
	}

	private void OnHeroesMerged(HeroesMergeData heroesMergeData)
	{
		_lastHeroesMergeData = heroesMergeData;
		_lastMove = LastMoveType.Merge;
		
		Reset();
		SetUndoButtonActive(true);
	}
	
	private void OnPoolUpdated()
	{
		Array.Copy(_currentHeroesData, _lastHeroesData, _heroesPool.Capacity);
		_currentHeroesData = _heroesPool.CurrentHeroesData;
	}

	private void OnHeroDropped(BoardUnit boardUnit)
	{
		_lastDroppedHeroBoardUnit = boardUnit;
		_lastMove = LastMoveType.Drop;
		
		Reset();
		SetUndoButtonActive(true);
	}
	
	private void OnBoosterUsed(Booster booster)
	{
		_lastMove = booster switch
		{
			ShuffleBooster => LastMoveType.Shuffle,
			HammerBooster => LastMoveType.Hammer,
			_ => _lastMove
		};
		
		Reset();
		SetUndoButtonActive(true);
	}

	private void OnNewHeroUnlocked(HeroData heroData)
	{
		_newHeroWasUnlocked = true;
	}

	private void OnNewMaxLevelReached()
	{
		_newMaxLevelWasReached = true;
	}
	
	private void Reset()
	{
		_newMaxLevelWasReached = false;
		_newHeroWasUnlocked = false;
	}

	public void Undo()
	{
		switch (_lastMove)
		{
			case LastMoveType.Drop: UndoDrop(); 
				break;
			case LastMoveType.Merge: UndoHeroesMerge(); 
				break;
			case LastMoveType.Hammer: UndoHammerBooster(); 
				break;
			case LastMoveType.Shuffle: UndoShuffleBooster(); 
				break;
		}

		SetUndoButtonActive(false);
		_lastMove = LastMoveType.Undo;
	}

	private void UndoHeroesMerge()
	{
		_heroesHandler.UndoHeroesMerge(_lastHeroesMergeData);
			
		if (_newHeroWasUnlocked)
			_heroesManager.UndoHeroUnlock(_lastHeroesMergeData.MergedHeroData);

		if (_newMaxLevelWasReached)
			_heroesManager.DecreaseMaxHeroLevel();
		
		_dataPersistenceManager.SaveGame();
	}

	private void UndoDrop()
	{
		Array.Copy(_lastHeroesData, _currentHeroesData, _heroesPool.Capacity);
		_heroesHandler.TryBackHeroToPool(LastHeroDropData);
	}
	
	private void UndoHammerBooster()
	{
		_heroesHandler.RespawnHero(_heroEliminateData);
	}
	
	private void UndoShuffleBooster()
	{
		Array.Copy(_lastHeroesData, _currentHeroesData, _heroesPool.Capacity);
		_heroesPool.RestorePool(_lastHeroesData);
	}

	private void SetUndoButtonActive(bool isActive)
	{
		if (isActive && !TutorialManager.IsCompleted) return;
		
		_undoButton.interactable = isActive;
	}
}

