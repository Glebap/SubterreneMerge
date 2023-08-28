using UnityEngine;
using Zenject;

public class ShuffleBooster : Booster
{
	[SerializeField] private bool _matchWithTopHero;
	
	[Inject] private Board _board;
	[Inject] private HeroesPool _heroesPool;
	
	public override string Name => "Shuffle";

	
	public override void Use()
	{
		OnBoosterUsed();
		
		if (_matchWithTopHero)
			_heroesPool.UpdatePool(_board.GetColumnsTopHeroesData());
		else
			_heroesPool.UpdatePool();
	} 
}