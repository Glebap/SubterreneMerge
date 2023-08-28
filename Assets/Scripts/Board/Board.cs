using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;


public class Board
{
    private readonly Color _firstColor;
    private readonly Color _secondColor;
    private readonly BoardUnit _boardUnitPrefab;
    
    private Vector2Int _boardSize;
    private Dictionary<(int column, int row), BoardUnit> _boardUnitsGridPositions;

    private int _capacity => Height * Width;
    
    public int Height => _boardSize.y;
    public int Width => _boardSize.x;
    public IEnumerable<BoardUnit> BoardUnits => _boardUnitsGridPositions.Values;
    
    public Vector2 BoardUnitSize { get; private set; }
    public Vector2 TrueSize { get; private set; }

    public readonly Transform Transform;
    
    public bool IsFull => BoardUnits.Count(unit => !unit.IsEmpty) == _capacity;

    public event Action BoardCleared;

    
    public Board(BoardConfig config, Transform boardTransform, BoardUnit boardUnitPrefab)
    {
        Transform = boardTransform;
        _firstColor = config.FirstColor;
        _secondColor = config.SecondColor;
        _boardSize = config.BoardSize;
        _boardUnitPrefab = boardUnitPrefab;
        
        SetUpBoardUnits();
    }
    
    public bool TryGetBoardUnit(int column, int row, out BoardUnit boardUnit)
    {
        boardUnit = default;
        if (column < 0 || row < 0 || column >= Width || row >= Height)
            return false;
        
        boardUnit = _boardUnitsGridPositions[(column, row)];
        return true;
    }
    
    public bool TryGetBoardUnit(Vector2 worldPosition, out BoardUnit boardUnit)
    {
        var localPosition = Transform.InverseTransformPoint(worldPosition);
        var column = Mathf.RoundToInt(localPosition.x / BoardUnitSize.x);
        var row = Mathf.RoundToInt(localPosition.y / BoardUnitSize.y);

        return TryGetBoardUnit(column, row, out boardUnit);
    }
    
    public bool TryGetBoardUnit(Vector2 worldPosition, float threshold, Vector2 direction, out BoardUnit boardUnit)
    {
        var widthOrHeight = direction.x == 0 ? BoardUnitSize.y : BoardUnitSize.x;
        var additionalRange = direction.normalized * ((0.5f - threshold) * widthOrHeight);
        
        worldPosition += additionalRange;
        
        return TryGetBoardUnit(worldPosition, out boardUnit);
    }

    public IEnumerable<Hero> GetHeroes()
    {
        foreach (var boardUnit in BoardUnits) 
            if (!boardUnit.IsEmpty) yield return boardUnit.ContainedHero;
    }

    public void ClearBoard()
    {
        foreach (var boardUnit in BoardUnits.Where(boardUnit => !boardUnit.IsEmpty))
            DestroyHeroAtBoardUnit(boardUnit);
        
        BoardCleared?.Invoke();
    }

    public void DestroyHeroAtBoardUnit(BoardUnit boardUnit)
    {
        boardUnit.ContainedHero.Destroy();
        boardUnit.Clear();
    }

    public IEnumerable<HeroData> GetColumnsTopHeroesData()
    {
        for (var column = 0; column < Width; column++)
        {
            Hero topHero = null;
            for (var row = Height; --row >= 0;)
                if ((topHero = _boardUnitsGridPositions[(column, row)].ContainedHero) != null) break;

            yield return topHero ? new HeroData(topHero) : null;
        }
    }

    public bool TryGetEmptyBoardUnitInColumn(int column, out BoardUnit boardUnit)
    {
        boardUnit = default;
        
        for (var row = 0; row < Height; row++)
        {
            boardUnit = _boardUnitsGridPositions[(column, row)];
            
            if (boardUnit.IsEmpty)
                return true;
        }

        return false;
    }

    private void SetUpBoardUnits()
    {
        BoardUnitSize = _boardUnitPrefab.SpriteRenderer.bounds.size;
        TrueSize = new Vector2(BoardUnitSize.x * Width, BoardUnitSize.y * Height);
            
        _boardUnitsGridPositions = new Dictionary<(int, int), BoardUnit>(Width * Height);
        
        Color color = _firstColor;
        for (var x = 0; x < Width; x++, color = x % 2 == 0 ? _firstColor : _secondColor)
            for (var y = 0; y < Height; y++)
            {
                var boardUnit = Object.Instantiate(_boardUnitPrefab, Transform);
                var position = new Vector3(x * BoardUnitSize.x, y * BoardUnitSize.y, 0);

                _boardUnitsGridPositions[(column: x, row: y)] = boardUnit;
                    
                boardUnit.transform.localPosition = position;
                boardUnit.Initialize(new Vector2Int(x, y));
                boardUnit.SpriteRenderer.color = color;
            }
    }
}
