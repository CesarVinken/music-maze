using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Right,
    Down,
    Left
}

public class CameraController : MonoBehaviour
{
    private float _panSpeed = 175f;

    private bool _focussedOnPlayer = false;
    //public static Dictionary<Direction, float> PanLimits = new Dictionary<Direction, float> { };

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _player;
    
    private float _maxXPercentageBoundary = 70f;
    private float _maxYPercentageBoundary = 70f;

    private Vector3 _dragOrigin;    //for camera dragging in editor
    public PlayerNumber PlayerNumberForCamera;
    public static ZoomAction CurrentZoom = ZoomAction.NoZoom;

    private CameraZoomHandler _cameraZoomHander;
    private Vector3 _velocity = Vector3.zero;

    private float _panLimitRight;
    private float _panLimitBottom;
    private float _panLimitLeft;
    private float _panLimitTop;

    public void Awake()
    {
        _camera = GetComponent<Camera>();

        //if (GameManager.Instance.CurrentGameLevel != null)
        //    SetPanLimits(GameManager.Instance.CurrentGameLevel.LevelBounds);
    }

    public void Start()
    {
        //if(GameManager.Instance.CurrentGameLevel != null)
        //    SetPanLimits(GameManager.Instance.CurrentGameLevel.LevelBounds);

        _cameraZoomHander = new CameraZoomHandler(_camera);
    }

    public void EnableCamera()
    {
        _camera.enabled = true;
    }

    public void DisableCamera()
    {
        _camera.enabled = false;
    }

    public void SetZoomLevel(float zoomLevel)
    {
        _camera.orthographicSize = zoomLevel;
    }

    public void ResetCamera()
    {
        _focussedOnPlayer = false;

        //if(!PanLimits.ContainsKey(Direction.Left) || !PanLimits.ContainsKey(Direction.Right) || !PanLimits.ContainsKey(Direction.Down) || !PanLimits.ContainsKey(Direction.Up))
        //{
        //    return;
        //}
    }

    public void SetPanLimits(GridLocation levelBounds)
    {
        Vector3 leftBottomCameraPos = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        //Vector3 rightTopCameraPos = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        float halfScreenWidth = leftBottomCameraPos.x;
        float halfScreenHeight = leftBottomCameraPos.y;
        float xPadding = 3f;
        float yPadding = 3f;
        float levelWidth = levelBounds.X;
        float levelHeight = levelBounds.Y;

        _panLimitRight = levelWidth - halfScreenWidth + xPadding;
        _panLimitBottom = halfScreenHeight - yPadding;
        _panLimitLeft = halfScreenWidth - xPadding;
        _panLimitTop = levelHeight - halfScreenHeight + yPadding;

        // Set minima for small levels
        //if (PanLimits[Direction.Up] < PanLimits[Direction.Down]) PanLimits[Direction.Up] = PanLimits[Direction.Down];
        if (_panLimitRight < _panLimitLeft)
        {
            _panLimitLeft = levelWidth / 2;
            _panLimitRight = levelWidth / 2;
        }
    }

    public void FocusOnPlayer(PlayerCharacter player)
    {
        _focussedOnPlayer = true;

        _player = player.transform;
        PlayerNumberForCamera = player.PlayerNumber;

        Vector2 desiredCameraPosition = new Vector3(_player.position.x, _player.position.y, -10f);

        SetPanLimits(MazeLevelGameplayManager.Instance.Level.LevelBounds);
        //Vector3 leftBottomCameraPos = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        ////Vector3 rightTopCameraPos = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        //float halfScreenWidth = leftBottomCameraPos.x;
        //float halfScreenHeight = leftBottomCameraPos.y;
        //float xPadding = 3f;
        //float yPadding = 3f;
        //float levelWidth = MazeLevelGameplayManager.Instance.Level.LevelBounds.X;
        //float levelHeight = MazeLevelGameplayManager.Instance.Level.LevelBounds.Y;

        //_panLimitRight = levelWidth - halfScreenWidth + xPadding;
        //_panLimitBottom = halfScreenHeight - yPadding;
        //_panLimitLeft = halfScreenWidth - xPadding;
        //_panLimitTop = levelHeight - halfScreenHeight + yPadding;

        //// Set minima for small levels
        ////if (PanLimits[Direction.Up] < PanLimits[Direction.Down]) PanLimits[Direction.Up] = PanLimits[Direction.Down];
        //if (_panLimitRight < _panLimitLeft)
        //{
        //    _panLimitLeft = levelWidth / 2;
        //    _panLimitRight = levelWidth / 2;
        //}

        //PanLimits.Add(Direction.Up, levelHeight - halfScreenHeight + yPadding);  // should depend on the furthest upper edge of the maze level.Never less than 4.
        //PanLimits.Add(Direction.Right, levelWidth - halfScreenWidth + xPadding);// should depend on the furthest right edge of the maze level  Never less than 8.
        //PanLimits.Add(Direction.Down, halfScreenHeight - yPadding); // should (with this zoom level) always have 4 as lowest boundary down. Should always be => 4
        //PanLimits.Add(Direction.Left, halfScreenWidth - xPadding); // should (with this zoom level) always have 8 as the left most boundary. Should always be => 8

        desiredCameraPosition.x = Mathf.Clamp(desiredCameraPosition.x, _panLimitLeft, _panLimitRight);
        desiredCameraPosition.y = Mathf.Clamp(desiredCameraPosition.y, _panLimitBottom, _panLimitTop);

        transform.position = new Vector3(desiredCameraPosition.x, desiredCameraPosition.y, -10f);
    }

    void Update()
    {
        if (EditorManager.InEditor)
        {
            HandleMiddleMousePanning();
        }

        if (!_focussedOnPlayer) return;

        if (GameRules.GamePlayerType != GamePlayerType.SplitScreenMultiplayer)
        {
            _cameraZoomHander.HandleZoom();
        }

        CalculateCameraPosition();
    }


    // Camera is static with player in the middle. Once the player get to the screen edges (the outer 30%) the camera follows the player.
    // If the camera/player reach the edge of the level, the camera movement is clamped so it will not move further in that direction
    private void CalculateCameraPosition()
    {
        Vector2 desiredCameraPosition = new Vector2(transform.position.x, transform.position.y);
        Vector3 playerWorldToScreenPos = _camera.WorldToScreenPoint(_player.position);
        float playerWidthPercentagePosOnScreen = (playerWorldToScreenPos.x / CameraManager.Instance.ScreenWidth) * 100f;
        float playerHeightPercentagePosOnScreen = (playerWorldToScreenPos.y / CameraManager.Instance.ScreenHeight) * 100f;

        if (playerWidthPercentagePosOnScreen >= _maxXPercentageBoundary ||
            playerWidthPercentagePosOnScreen <= 100 - _maxXPercentageBoundary ||
            playerHeightPercentagePosOnScreen >= _maxYPercentageBoundary ||
            playerHeightPercentagePosOnScreen <= 100 - _maxYPercentageBoundary
        )
        {
            desiredCameraPosition = new Vector3(_player.position.x, _player.position.y, -10);
        }

        desiredCameraPosition.x = Mathf.Clamp(desiredCameraPosition.x, _panLimitLeft, _panLimitRight);
        desiredCameraPosition.y = Mathf.Clamp(desiredCameraPosition.y, _panLimitBottom, _panLimitTop);

        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(desiredCameraPosition.x, desiredCameraPosition.y, -10), ref _velocity, 1f, _panSpeed * Time.deltaTime);
    }

    public void HandleMiddleMousePanning()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            _dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return;
        }

        if (!Input.GetKey(KeyCode.Mouse2)) return;

        Vector3 mouseCurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 distance = mouseCurrentPos - _dragOrigin;
        Vector3 cameraPosition = transform.position;
        cameraPosition += new Vector3(-distance.x, -distance.y, 0);

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, _panLimitLeft, _panLimitRight);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, _panLimitBottom, _panLimitTop);

        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
    }

    public Camera GetCamera()
    {
        return _camera;
    }
}
