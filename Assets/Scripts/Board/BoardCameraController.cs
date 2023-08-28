using UnityEngine;
using Zenject;


[RequireComponent(typeof(Camera))]
public class BoardCameraController : MonoBehaviour
{
	[SerializeField] private Vector2Int _defaultResolution = new (720, 1280);
	[SerializeField, Range(0f, 1f)] public float _widthOrHeight = 0;
	[SerializeField, Range(0f, 1f)] private float _sideMargin;
	
	[Inject] private Board _board;

	private Camera _camera;
	

	private void Awake()
	{
		_camera = GetComponent<Camera>();
		SetUpCamera();
	}

	private void SetUpCamera()
	{
		var targetAspect = (float)_defaultResolution.x / _defaultResolution.y;
		var _cameraTransform = _camera.transform;
		var boardSize = _board.TrueSize;

		_camera.orthographicSize = (boardSize.x * 0.5f + _sideMargin) / targetAspect;

		_cameraTransform.position = new Vector3(
			x: (boardSize.x - _board.BoardUnitSize.x) * 0.5f, 
			y: (boardSize.y - _board.BoardUnitSize.y) * 0.5f,
			z: _cameraTransform.position.z);
	}
}
