using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class BoosterButton : MonoBehaviour
{
    [SerializeField] private Button _addForRv;
    [SerializeField] private GameObject _noVideoView;
    [SerializeField] private GameObject _videoView;
    [SerializeField] private TMP_Text _amountView;

    private Button _button;
    private int _amount;

    public Button ShowAdButton => _addForRv;

    public bool Interactable
    {
        set => _button.interactable = value && _amount != 0;
    }

    
    private void Awake() 
        => _button = GetComponent<Button>();

    public void ToggleVideoAvailabilityView(bool isAvailable)
    {
        _addForRv.gameObject.SetActive(_amount == 0);
        
        _videoView.SetActive(isAvailable);
        _noVideoView.SetActive(!isAvailable);
    }

    public void UpdateAmountView(int value)
    {
        _amount = value;
        Interactable = _amount != 0;
        _amountView.text = $"{_amount}";
        _addForRv.gameObject.SetActive(_amount == 0);
    }
    
    public void AddListener(UnityAction call)
    {
        _button.onClick.AddListener(call);
    }
}
