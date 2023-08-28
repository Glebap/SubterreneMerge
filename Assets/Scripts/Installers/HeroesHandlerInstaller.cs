using UnityEngine;
using Zenject;

public class HeroesHandlerInstaller : MonoInstaller
{
	[SerializeField] private HeroesHandleConfig _heroesHandleConfig;
	
	
	public override void InstallBindings()
	{
		Container.Bind<HeroesHandleConfig>().FromInstance(_heroesHandleConfig).AsSingle();
		Container.BindInterfacesAndSelfTo<HeroesRestoreManager>().FromNew().AsSingle();
		Container.BindInterfacesAndSelfTo<HeroesHandler>().FromNew().AsSingle();
	}
}