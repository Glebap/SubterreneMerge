using UnityEngine;
using DG.Tweening;
using Zenject;


public class BoardHeroesMover
{
	[Inject] private readonly Board _board;


	public void DropHeroes(Vector2Int origin, float duration = 0.0f, float delay = 0.0f)
	{
		for (var row = origin.y + 1; row < _board.Height; row++)
		{
			var boardUnit = _board.TryGetBoardUnit(origin.x, row, out var unit) ? unit : null;

			if (boardUnit == null || boardUnit.IsEmpty)
				continue;

			var lowerBoardUnit = _board.TryGetBoardUnit(origin.x, row - 1, out var lowerUnit) ? lowerUnit : null;
			if (lowerBoardUnit == null)
				continue;

			MoveHero(boardUnit, lowerBoardUnit, duration).SetDelay(delay);
		}
	}

	public void RaiseHeroes(Vector2Int origin, float duration = 0.0f, float delay = 0.0f)
	{
		for (var row = _board.Height - 1; row > origin.y; row--)
		{
			var boardUnit = _board.TryGetBoardUnit(origin.x, row, out var unit) ? unit : null;

			if (boardUnit == null || !boardUnit.IsEmpty)
				continue;

			var lowerBoardUnit = _board.TryGetBoardUnit(origin.x, row - 1, out var lowerUnit) ? lowerUnit : null;
			if (lowerBoardUnit == null || lowerBoardUnit.IsEmpty)
				continue;

			MoveHero(lowerBoardUnit, boardUnit, duration).SetDelay(delay);
		}
	}

	private Tween MoveHero(BoardUnit origin, BoardUnit end, float duration)
	{
		var hero = origin.ContainedHero;
		origin.Clear();
		end.SetHero(hero);

		return hero.transform.DOMove(end.transform.position, duration);
	}
}