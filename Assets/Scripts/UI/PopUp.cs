using DG.Tweening;
using UnityEngine;


public class PopUp : MonoBehaviour
{
    [SerializeField] private TouchBlock _touchBlock;
    [SerializeField] private float _showSpeed;
    [SerializeField] private float _showTime;
    
    private Sequence _showSequence;

    public bool IsShowing { get; private set; }

    public void Show()
    {
        _showSequence.Kill();
        _showSequence = DOTween.Sequence();
        EnableTouchBlock();

        _showSequence
            .Append(transform.DOScale(Vector3.one, _showSpeed))
            .AppendInterval(_showTime)
            .Append(transform.DOScale(Vector3.zero, _showSpeed));
        
        _showSequence.Play().OnComplete(DisableTouchBlock);
        
        IsShowing = true;
    }

    public void Hide()
    {
        _showSequence.Kill();
        _showSequence = DOTween.Sequence();
        
        _showSequence.Append(transform.DOScale(Vector3.zero, _showSpeed)).Play().OnComplete(DisableTouchBlock);

        IsShowing = false;
    }

    private void DisableTouchBlock()
    {
        if (_touchBlock != null) _touchBlock.Disable();
    }
    
    private void EnableTouchBlock()
    {
        if (_touchBlock != null) _touchBlock.Enable();
    }
}
