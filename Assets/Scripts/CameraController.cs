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
    private float _panSpeed = 0.01f;
    private bool _focussedOnPlayer = false;
    public static Dictionary<Direction, float> PanLimits = new Dictionary<Direction, float> { };

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _player;
    
    private float _maxXPercentageBoundary = 70f;
    private float _maxYPercentageBoundary = 70f;

    private Vector3 _dragOrigin;    //for camera dragging in editor
    public PlayerNumber PlayerNumberForCamera;

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

        //CalculateCameraPosition();
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

        //CalculateCameraPosition();
    }

    public void SetPanLimits(GridLocation levelBounds)
    {
        PanLimits.Clear();

        //TODO. the - .. value is currently hardcoded but would not work with different screen sizes or zoom levels
        if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiPlayer)
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
            PanLimits.Add(Direction.Left, 11f); // should (with this zoom level) always have 8 as the left most boundary. Should always be => 8
        }

        // Set minima for small levels
        if (PanLimits[Direction.Up] < PanLimits[Direction.Down]) PanLimits[Direction.Up] = PanLimits[Direction.Down];
        if (PanLimits[Direction.Right] < PanLimits[Direction.Left]) PanLimits[Direction.Right] = PanLimits[Direction.Left];
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
