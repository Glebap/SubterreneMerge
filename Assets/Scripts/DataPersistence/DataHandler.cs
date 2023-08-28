#if UNITY_EDITOR
using NaughtyAttributes;
using UnityEngine;
using Zenject;


[ExecuteInEditMode]
public class DataHandler : MonoBehaviour
{
	[Inject] private DataPersistenceManager _dataPersistenceManager;
	
	
	[Button("NewGame")]
	public void NewGame()
	{
		_dataPersistenceManager.NewGame();
		PlayerPrefs.DeleteAll();
	}
    
	[Button("Save")]
	private void SaveGame()
	{
		_dataPersistenceManager.SaveGame();
	}
}

#endif