using UnityEngine;
using Zenject;


public class BoardInstaller : MonoInstaller
{
	[SerializeField] private BoardConfig _boardConfig;
	[SerializeField] private Transform _boardTransform;
	[SerializeField] private BoardUnit _boardUnitPrefab;

	
	public override void InstallBindings()
	{
		var board = new Board(_boardConfig, _boardTransform, _boardUnitPrefab);
		Container.BindInterfacesAndSelfTo<Board>().FromInstance(board).AsSingle();
		Container.BindInterfacesAndSelfTo<BoardHeroesMover>().FromNew().AsSingle();
	}
}
