using UnityEngine;
using Zenject;


public class HeroesManagerInstaller : MonoInstaller
{
	[SerializeField] private HeroRacesUnlockData _heroRacesUnlockData;
	
	
	public override void InstallBindings()
	{
		Container.Bind<HeroRacesUnlockData>().FromInstance(_heroRacesUnlockData).AsSingle();
		Container.BindInterfacesAndSelfTo<HeroesManager>().FromNew().AsSingle();
	}
}