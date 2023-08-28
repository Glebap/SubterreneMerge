using UnityEngine;
using Zenject;

public class HeroesPoolInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container.BindInterfacesAndSelfTo<HeroesPool>().FromNew().AsSingle();
		Container.BindInterfacesAndSelfTo<HeroesPoolHandler>().FromNew().AsSingle();
	}
}