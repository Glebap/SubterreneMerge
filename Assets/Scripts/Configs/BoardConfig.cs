using UnityEngine;

[CreateAssetMenu(menuName = "Board Config")]
public class BoardConfig : ScriptableObject
{
	[field: SerializeField] public Color FirstColor { get; private set; }
	[field: SerializeField] public Color SecondColor { get; private set; }
	[field: SerializeField] public Vector2Int BoardSize { get; private set; }
}