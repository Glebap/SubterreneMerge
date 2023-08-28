using UnityEngine;

public class TargetFrameRateSetter : MonoBehaviour
{
    [SerializeField, Min(0)] private int _framerate = 60;
    
    private void Awake() => Application.targetFrameRate = _framerate;
}
