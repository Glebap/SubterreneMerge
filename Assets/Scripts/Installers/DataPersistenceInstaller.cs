using UnityEngine;
using Zenject;


public class DataPersistenceInstaller : MonoInstaller
{
	[SerializeField] private DataPersistenceConfig _dataPersistenceConfig;
	
	
	public override void InstallBindings()
	{
		Container.Bind<DataPersistenceConfig>().FromInstance(_dataPersistenceConfig).AsSingle();
		Container.BindInterfacesAndSelfTo<DataPersistenceManager>().FromNew().AsSingle();
	}
}