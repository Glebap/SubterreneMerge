using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;


public class TouchHandler : MonoBehaviour
{
	[SerializeField] private Camera _camera;
	[SerializeField] private TutorialManager _tutorialManager;
	[SerializeField, Min(0)] private float _dragSpeed = 36;
	[SerializeField, Range(0, 1)] private float _clickThreshold;
	[SerializeField, Range(0, 1)] private float _mergeThreshold;
	
	[Inject] private Board _board;
	[Inject] private HeroesHandler _heroesHandler;

	private HandleMode _mode = HandleMode.Default;
	private bool _isClick;
	
	private Vector2 _clickOffset;
	private Vector2 _touchStartPosition;
	private Vector2 _selectedBoardUnitPosition;
	
	private BoardUnit _selectedBoardUnit;
	private BoardUnit _nearBoardUnit;

	private Transform _selectedHeroTransform;
	private Transform _selectedBoardUnitTransform;

	public event Action<HeroAction> HeroActionDone;
	public event Action<BoardUnit> HeroPicked;

	private Vector2 GetTouchAsWorldPoint(Touch touch) 
		=> _camera.ScreenToWorldPoint(touch.position);

	private bool MergeIsNotAllowed => _mode == HandleMode.Tutorial &&
	                                    !_tutorialManager.IsMergeAllowed(_selectedBoardUnit, _nearBoardUnit);

	
	public void SetMode(HandleMode mode)
	{
		_mode = mode;
	}
	
	private void Update()
	{
		if (Input.touchCount <= 0) return;

		var touch = Input.GetTouch(0);
		if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
		{
			switch (touch.phase)
			{
				case TouchPhase.Began:
					OnTouchBegan(touch);
					break;
				case TouchPhase.Moved:
					OnTouchMoved(touch);
					break;
				case TouchPhase.Ended or TouchPhase.Canceled:
					OnTouchEnded(touch);
					return;
			}
		}

		if (_selectedBoardUnit is { IsEmpty: false })
			DragSelectedHero(GetTouchAsWorldPoint(touch));
	}

	private void OnTouchBegan(Touch touch)
    {
        _isClick = true;
        _touchStartPosition = GetTouchAsWorldPoint(touch);
        
        SelectBoardUnit(_touchStartPosition);
    }
    
    private void SelectBoardUnit(Vector2 touchPosition)
    {
	    if (!TrySelectBoardUnit(touchPosition, out _selectedBoardUnit)) return;
	    
	    _selectedBoardUnitPosition = _selectedBoardUnit.transform.position;
	    _clickOffset = _selectedBoardUnitPosition - _touchStartPosition;
	    _selectedHeroTransform = _selectedBoardUnit.IsEmpty 
		    ? null : _selectedBoardUnit.ContainedHero.transform;
    }
    
    private void OnTouchMoved(Touch touch)
    {
        Vector2 touchPosition = GetTouchAsWorldPoint(touch);

        bool TouchIsWithinClickThreshold() =>
	        Vector2.Distance(touchPosition, _touchStartPosition) < _clickThreshold;
        
        if (_isClick && TouchIsWithinClickThreshold()) return;
        
        _isClick = false;
        
        SearchForAnotherHero();
    }
    
    private Vector2 _startPocketPosition;

    private void DragSelectedHero(Vector2 touchPosition)
	{
	    if (_isClick) return;

	    var selectedHeroTransform = _selectedHeroTransform;
	    touchPosition += _clickOffset;

	    Vector2 boardUnitSize = _board.BoardUnitSize;
	    Vector2 goalPosition = _selectedBoardUnitPosition;

	    float deltaX = Mathf.Abs(touchPosition.x - goalPosition.x);
	    float deltaY = Mathf.Abs(touchPosition.y - goalPosition.y);

	    if (deltaX > deltaY)
	        goalPosition.x = Mathf.Clamp(touchPosition.x, goalPosition.x - boardUnitSize.x, goalPosition.x + boardUnitSize.x);
	    else
	        goalPosition.y = Mathf.Clamp(touchPosition.y, goalPosition.y - boardUnitSize.y, goalPosition.y + boardUnitSize.y);

	    float step = _dragSpeed * Time.deltaTime;
	    selectedHeroTransform.position = Vector2.MoveTowards(selectedHeroTransform.position, goalPosition, step);
	}

    
    private void OnTouchEnded(Touch touch)
    {
        if (_selectedBoardUnit == null) 
	        return;

        if (_isClick)
	        OnBoardUnitClicked();
        else
	        TryMergeSelectedHeroes();

        Reset();
    }

    private void TryMergeSelectedHeroes()
    {
	    if (_selectedBoardUnit.IsEmpty || _mode == HandleMode.HeroPick) return; 
	    
	    if (_nearBoardUnit == null || MergeIsNotAllowed)
	    {
		    BackToOriginPosition(_selectedBoardUnit);
	    }
	    else
	    {
		    _heroesHandler.MergeHeroes(_selectedBoardUnit, _nearBoardUnit);
		    HeroActionDone?.Invoke(HeroAction.Merged);
	    }
    }

    private void Reset()
    {
	    _selectedBoardUnit = null;
		_nearBoardUnit = null;
    }

    private void OnBoardUnitClicked()
    {
	    if (_mode == HandleMode.HeroPick)
	    {
		    if (!_selectedBoardUnit.IsEmpty)
			    HeroPicked?.Invoke(_selectedBoardUnit);
		    return;
	    }
	    
        int column = _selectedBoardUnit.GridPosition.x;

        if (_mode == HandleMode.Tutorial
            && !_tutorialManager.IsTapAllowed(column))
        {
	        BackToOriginPosition(_selectedBoardUnit);
	        return;
        }
        
        _heroesHandler.TryDropHeroFromPool(column);
        HeroActionDone?.Invoke(HeroAction.Dropped);
    }
    
    private void BackToOriginPosition(BoardUnit boardUnit)
    {
	    if (!boardUnit.IsEmpty)
		    boardUnit.ContainedHero.transform.DOMove(boardUnit.transform.position, 0.22f);
	    
	    HeroActionDone?.Invoke(HeroAction.Released);
    }
	
    private bool TrySelectBoardUnit(Vector2 touchPosition, out BoardUnit boardUnit)
    {
	    if (!_board.TryGetBoardUnit(touchPosition, out boardUnit))  
		    return false;

	    HeroActionDone?.Invoke(HeroAction.Selected);
	    return true;
    }

    private void SearchForAnotherHero()
    {
	    if (_selectedBoardUnit is null or { IsEmpty: true }) return;

	    var selectedHeroPosition = (Vector2)_selectedHeroTransform.position;
	    var direction = selectedHeroPosition - _selectedBoardUnitPosition;

	    if (!_board.TryGetBoardUnit(selectedHeroPosition, _mergeThreshold, direction, out var nearBoardUnit) 
	        || nearBoardUnit == _nearBoardUnit) return;
	    
	    if (nearBoardUnit == _selectedBoardUnit)
	    {
		    _nearBoardUnit = null;
		    return;
	    }

        if (_nearBoardUnit != null)
	        HighlightBoardUnit(_nearBoardUnit, false);
        
        if (!_selectedBoardUnit.ContainedHero.Equals(nearBoardUnit.ContainedHero)) return;
        
        _nearBoardUnit = nearBoardUnit;
        HighlightBoardUnit(_nearBoardUnit, true);
    }

    private void HighlightBoardUnit(BoardUnit boardUnit, bool value)
    {
	    
    }
}