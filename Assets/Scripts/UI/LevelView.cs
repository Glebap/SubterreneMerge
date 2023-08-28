using TMPro;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    [SerializeField] private TMP_Text _countView;

    public void Show(int value)
    {
        _countView.text = $"{value}";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
