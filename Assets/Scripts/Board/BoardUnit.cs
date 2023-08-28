using UnityEngine;


public class BoardUnit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    public Vector2Int GridPosition { get; private set; }
    public Hero ContainedHero { get; private set; }

    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public bool IsEmpty => ContainedHero == null;

    
    public void Initialize(Vector2Int position)
    {
        GridPosition = position;
    }

    public void Clear()
    {
        ContainedHero = null;
    }

    public void SetHero(Hero hero)
    {
        ContainedHero = hero;
    }
}
