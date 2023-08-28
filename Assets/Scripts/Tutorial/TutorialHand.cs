using DG.Tweening;
using UnityEngine;


public class TutorialHand : MonoBehaviour
{
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private Transform _handTransform;
	
	[Header("Tween Settings")]
	[SerializeField] private float _moveDuration = 0.6f;
	[SerializeField] private float _rotationDuration = 0.3f;
	[SerializeField] private float _fadeDuration = 0.3f;
	[SerializeField] private float _scaleDuration = 0.4f;
	
	private Transform _transform;
	private bool _isHided = true;
	private Sequence _hintSequence;

	
	private void Awake()
	{
		_transform = transform;
		_spriteRenderer.DOFade(0.0f, 0.0f);
	}
	
	public void ShowHint(Vector2 startPoint, Vector2 endPoint)
	{
		HideAndReset(startPoint);
		
		_hintSequence.Append(_spriteRenderer.DOFade(1.0f, _fadeDuration))
					 .Append(_handTransform.DOLocalRotate(new Vector3(0, 0, 30), _rotationDuration).SetDelay(0.12f))
					 .Append(_transform.DOMove(endPoint, _moveDuration))
					 .Append(_handTransform.DOLocalRotate(Vector3.zero, _rotationDuration))
					 .Append(_spriteRenderer.DOFade(0.0f, _fadeDuration).OnComplete(()=>_transform.position = startPoint));

		_hintSequence.Play().SetLoops(-1, LoopType.Restart);
	}
	
	public void ShowHint(Vector2 point)
	{
		HideAndReset(point);
		_spriteRenderer.DOFade(1.0f, _fadeDuration);
		
		_hintSequence.Append(_transform.DOScale(0.7f, _scaleDuration));

		_hintSequence.Play().SetLoops(-1, LoopType.Yoyo);
	}

	private void HideAndReset(Vector2 point)
	{
		_hintSequence.Kill();
		_hintSequence = DOTween.Sequence();
		_isHided = false;
		_transform.position = point;
		_transform.localScale = Vector3.one;
		_handTransform.localRotation = Quaternion.Euler(Vector3.zero);
	}

	public void Hide()
	{
		if (_isHided) return;
		
		_hintSequence.Kill();
		_spriteRenderer.DOFade(0.0f, 0.3f);
		_isHided = true;
	}
}


