using System;
using System.Collections.Generic;
using UnityEngine;

public enum ZoomType
{
    ZoomIn,
    ZoomOut
}
public enum Direction
{
    Up,
    Right,
    Down,
    Left
}

public class CameraController : MonoBehaviour
{
    private float _panSpeed = 0.011f;
    private float _zoomSpeed;
    private float _zoomMin;
    private float _zoomMax;

    private bool _focussedOnPlayer = false;
    public static Dictionary<Direction, float> PanLimits = new Dictionary<Direction, float> { };

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _player;
    
    private float _maxXPercentageBoundary = 70f;
    private float _maxYPercentageBoundary = 70f;

    private Vector3 _dragOrigin;    //for camera dragging in editor
    private float _desiredZoomLevel;
    public PlayerNumber PlayerNumberForCamera;
    public static bool IsZooming = false;
    private float _zoomInCooldownTime = 4f; // after how much time do we start zooming in back to the default level
    private bool _isCooldownFromZoomToDefault = false;

    public void Awake()
    {
        _camera = GetComponent<Camera>();

        if (GameManager.Instance.CurrentGameLevel != null)
            SetPanLimits(GameManager.Instance.CurrentGameLevel.LevelBounds);
    }

    public void Start()
    {
        if(GameManager.Instance.CurrentGameLevel != null)
            SetPanLimits(GameManager.Instance.CurrentGameLevel.LevelBounds);

        _zoomMin = GameManager.Instance.Configuration.DefaultCameraZoomLevel;
        _zoomMax = GameManager.Instance.Configuration.MaximumCameraZoomLevel;
        _zoomSpeed = GameManager.Instance.Configuration.ZoomSpeed;

        _desiredZoomLevel = _camera.orthographicSize;
        Logger.Warning($"_desiredZoomLevel = {_desiredZoomLevel}");
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

        if(!PanLimits.ContainsKey(Direction.Left) || !PanLimits.ContainsKey(Direction.Right) || !PanLimits.ContainsKey(Direction.Down) || !PanLimits.ContainsKey(Direction.Up))
        {
            return;
        }
    }

    public void SetPanLimits(GridLocation levelBounds)
    {
        PanLimits.Clear();

        //TODO. the - .. value is currently hardcoded but would not work with different screen sizes or zoom levels
        if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            PanLimits.Add(Direction.Up, levelBounds.Y);  // should depend on the furthest upper edge of the maze level.Never less than 4.
            PanLimits.Add(Direction.Right, levelBounds.X - 3f);// should depend on the furthest right edge of the maze level  Never less than 8.
            PanLimits.Add(Direction.Down, 1f); // should (with this zoom level) always have 4 as lowest boundary down. Should always be => 4
            PanLimits.Add(Direction.Left, 3f); // should (with this zoom level) always have 8 as the left most boundary. Should always be => 8
        }
        else
        {
            PanLimits.Add(Direction.Up, levelBounds.Y - 3f);  // should depend on the furthest upper edge of the maze level.Never less than 4.
            PanLimits.Add(Direction.Right, levelBounds.X - 6f);// should depend on the furthest right edge of the maze level  Never less than 8.
            PanLimits.Add(Direction.Down, 4f); // should (with this zoom level) always have 4 as lowest boundary down. Should always be => 4
            PanLimits.Add(Direction.Left, 7f); // should (with this zoom level) always have 8 as the left most boundary. Should always be => 8
        }

        // Set minima for small levels
        //if (PanLimits[Direction.Up] < PanLimits[Direction.Down]) PanLimits[Direction.Up] = PanLimits[Direction.Down];
        if (PanLimits[Direction.Right] < PanLimits[Direction.Left])
        {
            PanLimits[Direction.Left] = levelBounds.X / 2;
            PanLimits[Direction.Right] = levelBounds.X / 2;
        }
    }

    public void FocusOnPlayer(PlayerCharacter player)
    {
        _focussedOnPlayer = true;

        _player = player.transform;
        PlayerNumberForCamera = player.PlayerNumber;

        transform.position = new Vector3(_player.position.x, _player.position.y, -10f);
    }

    void Update()
    {
        if (EditorManager.InEditor)
        {
            HandleMiddleMousePanning();
        }

        if (!_focussedOnPlayer) return;

        if(GameRules.GamePlayerType != GamePlayerType.SplitScreenMultiplayer)
        {
            HandleZoom();
        }

        CalculateCameraPosition();
    }

    // Camera is static with player in the middle. Once the player get to the screen edges (the outer 30%) the camera follows the player.
    // If the camera/player reach the edge of the level, the camera movement is clamped so it will not move further in that direction
    private void CalculateCameraPosition()
    {
        Vector2 cameraPosition = new Vector2(transform.position.x, transform.position.y);

        Vector3 playerWorldToScreenPos = _camera.WorldToScreenPoint(_player.position);
        float playerWidthPercentagePosOnScreen = (playerWorldToScreenPos.x / CameraManager.Instance.ScreenWidth) * 100f;
        float playerHeightPercentagePosOnScreen = (playerWorldToScreenPos.y / CameraManager.Instance.ScreenHeight) * 100f;

        if (playerWidthPercentagePosOnScreen >= _maxXPercentageBoundary)
        {
            cameraPosition = Vector3.Lerp(transform.position, new Vector3(_player.position.x, _player.position.y, -10), _panSpeed);
        }
        else if (playerWidthPercentagePosOnScreen <= 100 - _maxXPercentageBoundary)
        {
            cameraPosition = Vector3.Lerp(transform.position, new Vector3(_player.position.x, _player.position.y, -10), _panSpeed);
        }

        if (playerHeightPercentagePosOnScreen >= _maxYPercentageBoundary)
        {
            cameraPosition = Vector3.Lerp(transform.position, new Vector3(_player.position.x, _player.position.y, -10), _panSpeed);
        }
        else if (playerHeightPercentagePosOnScreen <= 100 - _maxYPercentageBoundary)
        {
            cameraPosition = Vector3.Lerp(transform.position, new Vector3(_player.position.x, _player.position.y, -10), _panSpeed);
        }

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, PanLimits[Direction.Left], PanLimits[Direction.Right]);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, PanLimits[Direction.Down], PanLimits[Direction.Up]);

        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, -10);
    }

    public void HandleZoom()
    {
        float currentZoomLevel = _camera.orthographicSize;

        // Stop zooming if the difference with the desired zoom level is neglectible 
        if(Math.Abs(currentZoomLevel - _desiredZoomLevel) < 0.01f){
            _desiredZoomLevel = currentZoomLevel;
        }

        if(_desiredZoomLevel != _camera.orthographicSize)
        {
            float newZoomLevel = Mathf.Lerp(currentZoomLevel, _desiredZoomLevel, _zoomSpeed * Time.deltaTime);
            _camera.orthographicSize = Mathf.Clamp(newZoomLevel, _zoomMin, _zoomMax);
        }

        if(PersistentGameManager.CurrentPlatform == Platform.PC)
        {
            // Zoom out
            if(Input.GetKey(KeyCode.O) || Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                Zoom(ZoomType.ZoomOut);
            }
            else if(Input.GetKey(KeyCode.P) || Input.GetAxis("Mouse ScrollWheel") < 0f) // Zoom in
            {
                Zoom(ZoomType.ZoomIn);
            }
        }
        else // ANDROID
        {
            if (Input.touchCount == 2)
            {
                // get current touch positions
                Touch touchOne = Input.GetTouch(0);
                Touch touchTwo = Input.GetTouch(1);

                // get touch position from the previous frame
                Vector2 touchOnePrevious = touchOne.position - touchOne.deltaPosition;
                Vector2 touchTwoPrevious = touchTwo.position - touchTwo.deltaPosition;

                float oldTouchDistance = Vector2.Distance (touchOnePrevious, touchTwoPrevious);
                float currentTouchDistance = Vector2.Distance (touchOne.position, touchTwo.position);

                // get offset value
                float deltaDistance = oldTouchDistance - currentTouchDistance;
                if(deltaDistance > 1)
                {
                    Zoom (ZoomType.ZoomOut);
                }
                else 
                {
                    Zoom (ZoomType.ZoomIn);
                }
            }
            else {
                IsZooming = false; // TODO Should not be checked every frame
            }
        }
    }

    private void Zoom(ZoomType zoomType)
    {
        IsZooming = true;
        float currentZoomLevel = _camera.orthographicSize;

        if(zoomType == ZoomType.ZoomOut) // zoom out
        { 
            _desiredZoomLevel = _desiredZoomLevel - 0.4f;
        }
        else if(zoomType == ZoomType.ZoomIn) // zoom in
        { 
            _desiredZoomLevel = _desiredZoomLevel + 0.4f;
        }

        if(_desiredZoomLevel > currentZoomLevel + 1f){
            _desiredZoomLevel = currentZoomLevel + 1f;
        }
        else if(_desiredZoomLevel < currentZoomLevel - 1f){
           _desiredZoomLevel = currentZoomLevel - 1f;
        }

        if(_desiredZoomLevel < _zoomMin){
            _desiredZoomLevel = _zoomMin;
        }
        else if(_desiredZoomLevel > _zoomMax){
            _desiredZoomLevel = _zoomMax;
        }
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

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, PanLimits[Direction.Left], PanLimits[Direction.Right]);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, PanLimits[Direction.Down], PanLimits[Direction.Up]);

        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
    }

    public Camera GetCamera()
    {
        return _camera;
    }
}
