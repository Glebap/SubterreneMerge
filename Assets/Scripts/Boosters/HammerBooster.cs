using UnityEngine;
using Zenject;

public class HammerBooster : Booster
{
	[SerializeField] private TouchHandler _touchHandler;
	
	[Inject] private HeroesHandler _heroesHandler;

	public override string Name => "Hammer";
	
	private bool _pickModeEnabled;
	
	
	private void OnEnable() 
		=> _touchHandler.HeroPicked += OnHeroPicked;

	private void OnDisable()
		=> _touchHandler.HeroPicked -= OnHeroPicked;

	public override void Use()
	{
		TogglePickMode(!_pickModeEnabled);
	}

	private void TogglePickMode(bool state)
	{
		var mode = state ? HandleMode.HeroPick : HandleMode.Default;
		
		_heroesHandler.ToggleAllHeroesHighlight(state);
		_touchHandler.SetMode(mode);
		_pickModeEnabled = state;
	}

	private void OnHeroPicked(BoardUnit boardUnit)
	{
		TogglePickMode(false);
		_heroesHandler.EliminateHero(boardUnit);
		
		OnBoosterUsed();
	}
}