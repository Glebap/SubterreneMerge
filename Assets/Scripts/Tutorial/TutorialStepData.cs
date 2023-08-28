using UnityEngine;


[System.Serializable]
public class TutorialStepData
{
	[SerializeField] private TutorialStepActionType _actionType;
	[SerializeField] private Vector2Int _firstItem;
	[SerializeField] private Vector2Int _secondItem;
	[SerializeField] private int _column;
	
	
	public TutorialStepActionType ActionType => _actionType;
	public Vector2Int FirstItem => _firstItem;
	public Vector2Int SecondItem => _secondItem;
	public int Column => _column;

}