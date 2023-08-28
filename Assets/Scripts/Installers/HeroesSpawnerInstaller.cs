using UnityEngine;
using Zenject;


public class HeroesSpawnerInstaller : MonoInstaller
{
	[SerializeField] private Transform _heroesTransform;
	[SerializeField] private Hero _heroPrefab;
	
	
	public override void InstallBindings()
	{
		var heroesSpawner = new HeroesSpawner(_heroPrefab, _heroesTransform);
		Container.Bind<HeroesSpawner>().FromInstance(heroesSpawner).AsSingle();
		Container.BindInterfacesAndSelfTo<HeroesMergeHandler>().FromNew().AsSingle();
	}
}