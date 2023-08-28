using UnityEngine;

[CreateAssetMenu(menuName = "Data Persistence Config", fileName = "DataPersistenceConfig")]
public class DataPersistenceConfig : ScriptableObject
{
	[field: SerializeField] public string FileName { get; private set; }
	[field: SerializeField] public bool UseEncryption { get; private set; }
}