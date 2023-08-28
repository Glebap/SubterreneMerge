using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TouchBlock : MonoBehaviour
{
    private Image _image;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Enable()
    {
        _image.raycastTarget = true;
    }

    public void Disable()
    {
        _image.raycastTarget = false;
    }
}
