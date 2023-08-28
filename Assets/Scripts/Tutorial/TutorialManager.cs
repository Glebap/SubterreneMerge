using System;
using UnityEngine;
using Zenject;


[DefaultExecutionOrder(0)]
public sealed class TutorialManager: MonoBehaviour
{
	[SerializeField] private TouchHandler _touchHandler;
	[SerializeField] private TutorialHand _tutorialHandPrefab;
	[SerializeField] private TutorialStepData[] _tutorialStepsData;
	
	[Inject] private Board _board;

	private TutorialHand _tutorialHand;

	private int CurrentTutorialStepIndex
	{
		get => PlayerPrefs.GetInt("TutorialStepIndex", 0);
		set => PlayerPrefs.SetInt("TutorialStepIndex", value);
	}
	
	public static bool IsCompleted => PlayerPrefs.GetInt("TutorialIsCompleted", 0) == 1;
	
	public TutorialStepData CurrentTutorialStep => _tutorialStepsData[CurrentTutorialStepIndex];

	public event Action TutorialCompleted;

	private void Start()
	{
		if (IsCompleted) return;
		
		_touchHandler.SetMode(HandleMode.Tutorial);
		_tutorialHand = Instantiate(_tutorialHandPrefab);
		ShowCurrentTutorialStep();
	}

	private void OnEnable()
	{
		_touchHandler.HeroActionDone += OnHeroActionDone;	
	}

	private void OnDisable()
	{
		_touchHandler.HeroActionDone -= OnHeroActionDone;
	}

	public bool IsMergeAllowed(BoardUnit selectedBoardUnit, BoardUnit nearBoardUnit)
	{
		var step = CurrentTutorialStep;
		var isMergeAction = step.ActionType is TutorialStepActionType.Merge;

		var itemsMatch = step.FirstItem == selectedBoardUnit.GridPosition && step.SecondItem == nearBoardUnit.GridPosition;
		var reverseItemsMatch = step.FirstItem == nearBoardUnit.GridPosition && step.SecondItem == selectedBoardUnit.GridPosition;

		return step.FirstItem.x == step.SecondItem.x
			? isMergeAction && (itemsMatch || reverseItemsMatch)
			: itemsMatch;
	}
 
	public bool IsTapAllowed(int column)
	{
		return CurrentTutorialStep.ActionType == TutorialStepActionType.Tap 
		       && CurrentTutorialStep.Column == column;
	}

	private Vector3 GetBoardUnitPosition(int column, int row)
    {
        var boardPosition = _board.Transform.position;
        if (_board.TryGetBoardUnit(column, row, out var boardUnit))
            return boardPosition + boardUnit.transform.position;
        return Vector3.zero;
    }
    
    private void ShowCurrentTutorialStep()
    {
        if (IsCompleted) return;
    
        var requiredAction = CurrentTutorialStep.ActionType;
    
        var tapColumn = GetBoardUnitPosition(CurrentTutorialStep.Column, 2);
        var startPoint = GetBoardUnitPosition(CurrentTutorialStep.FirstItem.x, CurrentTutorialStep.FirstItem.y);
        var endPoint = GetBoardUnitPosition(CurrentTutorialStep.SecondItem.x, CurrentTutorialStep.SecondItem.y);
    
        if (requiredAction == TutorialStepActionType.Tap)
            _tutorialHand.ShowHint(tapColumn);
        else if (requiredAction == TutorialStepActionType.Merge)
            _tutorialHand.ShowHint(startPoint, endPoint);
    }

	public void HideCurrentTutorialStep()
	{
		if (IsCompleted) return;
		
		_tutorialHand.Hide();
	}
	
	private void ShowNextTutorialStep()
	{
		if (IsCompleted) return;
		
		if (CurrentTutorialStepIndex == _tutorialStepsData.Length - 1)
		{
			OnTutorialCompleted();
			return;
		}

		CurrentTutorialStepIndex++;
		ShowCurrentTutorialStep();
	}

	private void OnTutorialCompleted()
	{
		_touchHandler.SetMode(HandleMode.Default);
		TutorialCompleted?.Invoke();
		PlayerPrefs.SetInt("TutorialIsCompleted", 1);
	}

	private void OnItemDropped()
	{
		if (CurrentTutorialStep.ActionType == TutorialStepActionType.Tap)
			OnStepCompleted();
		else
			OnStepFailed();
	}

	private void OnItemMerged()
	{
		OnStepCompleted();
	}
	
	private void OnItemReleased()
	{
		OnStepFailed();
	}

	private void OnItemSelected()
	{
		if (_tutorialHand != null) 
			_tutorialHand.Hide();
	}

	private void OnHeroActionDone(HeroAction action)
	{
		if (action == HeroAction.Selected)
			OnItemSelected();
		else if (action == HeroAction.Released)
			OnItemReleased();
		else if (action == HeroAction.Dropped)
			OnItemDropped();
		else if (action == HeroAction.Merged)
			OnItemMerged();
		else
			Debug.LogError("Unknown action");
	}

	private void OnStepCompleted()
	{
		ShowNextTutorialStep();
	}

	private void OnStepFailed()
	{
		ShowCurrentTutorialStep();
	}
}