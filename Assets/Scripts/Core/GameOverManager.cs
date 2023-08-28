using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameOverManager : MonoBehaviour
{
	[SerializeField] private Button _undoButton;
	[SerializeField] private Button _hammerBoosterButton;
	[SerializeField] private Button _resetButton;
	[SerializeField] private PopUp _gameOverWindow;

	[Inject] private Board _board;
	[Inject] private HeroesHandler _heroesHandler;
	
	private Sequence _highlightSequence;

	private void OnEnable()
	{
		_heroesHandler.HeroesMerged += OnHeroesMerged;
		_heroesHandler.HeroDropped += OnHeroDropped;
		_heroesHandler.HeroesRestored += TryCheckForAvailableMoves;
		_undoButton.onClick.AddListener(OnUndoButtonClicked);
		_hammerBoosterButton.onClick.AddListener(OnHammerBoosterButtonClicked);
		_resetButton.onClick.AddListener(Disable);
	}

	private void OnDisable()
	{
		_heroesHandler.HeroesMerged -= OnHeroesMerged;
		_heroesHandler.HeroDropped -= OnHeroDropped;
		_heroesHandler.HeroesRestored -= TryCheckForAvailableMoves;
		_undoButton.onClick.RemoveListener(OnUndoButtonClicked);
		_hammerBoosterButton.onClick.RemoveListener(OnHammerBoosterButtonClicked);
		_resetButton.onClick.RemoveListener(Disable); 
	}

	private void OnHeroDropped(BoardUnit boardUnit)
	{
		TryCheckForAvailableMoves();
	}

	private void OnHeroesMerged(HeroesMergeData heroesMergeData)
	{
		TryCheckForAvailableMoves();
	}

	private void TryCheckForAvailableMoves()
	{
		if (_board.IsFull)
			CheckForAvailableMoves();
	}

	private void CheckForAvailableMoves()
	{
		foreach (var boardUnit in _board.BoardUnits)
		{
			var hero = boardUnit.ContainedHero;
			var column = boardUnit.GridPosition.x;
			var row = boardUnit.GridPosition.y;

			if (HasMatchingHero(column, row + 1, hero)
			    || HasMatchingHero(column + 1, row, hero)) return;
		}
		
		OnGameOver();
	}

	private bool HasMatchingHero(int column, int row, Hero heroToMatch)
	{
		return _board.TryGetBoardUnit(column, row, out var boardUnit) 
		       && boardUnit.ContainedHero.Equals(heroToMatch);
	}

	private void OnGameOver()
	{
		_gameOverWindow.Show();
		EnableButtonsHighlight(true);
		GameEventManager.SendLevelFailedEvent();
	}

	private void OnUndoButtonClicked()
	{
		if (!_gameOverWindow.IsShowing) return;

		Disable();
	}
	
	private void OnHammerBoosterButtonClicked()
	{
		if (!_gameOverWindow.IsShowing) return;

		Disable();
	}

	private void Disable()
	{
		EnableButtonsHighlight(false);
		_gameOverWindow.Hide();
	}

	private void EnableButtonsHighlight(bool enable)
	{
		if (enable)
		{
			_highlightSequence = DOTween.Sequence()
				.Join(_undoButton.transform.DOScale(1.1f, 0.36f))
				.Join(_hammerBoosterButton.transform.DOScale(1.1f, 0.36f))
				.SetLoops(-1, LoopType.Yoyo)
				.Play();
		}
		else
		{
			_highlightSequence.Kill();
			_undoButton.transform.DOScale(1.0f, 0.18f);
			_hammerBoosterButton.transform.DOScale(1.0f, 0.18f);
		}
	}
}