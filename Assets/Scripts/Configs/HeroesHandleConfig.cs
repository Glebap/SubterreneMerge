using UnityEngine;

[CreateAssetMenu(menuName = "Heroes Handler Config")]
public class HeroesHandleConfig : ScriptableObject
{
	[field: SerializeField, Min(0)] public float PoolSpawnDuration { get; private set; } = 0.22f;
	[field: SerializeField, Min(0)] public float PoolDropDuration { get; private set; } = 0.36f;
	[field: SerializeField, Min(0)] public float BoardDropDuration { get; private set; } = 0.12f;
	[field: SerializeField, Min(0)] public float MergeDuration { get; private set; } = 0.24f;
}