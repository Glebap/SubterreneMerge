using UnityEngine;
using Zenject;


public class ParticleManagerInstaller : MonoInstaller
{
	[SerializeField] private ParticleManager _particleManager;
	
	
	public override void InstallBindings()
	{
		Container.Bind<ParticleManager>().FromInstance(_particleManager).AsSingle();
	}
}