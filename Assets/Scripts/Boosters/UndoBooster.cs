using UnityEngine;

public class UndoBooster : Booster
{
	[SerializeField] private UndoManager _undoManager;
	
	public override string Name => "Undo";
	
	public override void Use()
	{
		OnBoosterUsed();
		_undoManager.Undo();
	}
}
