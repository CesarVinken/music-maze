using System.Collections.Generic;
using System.Linq;
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
    public static CameraController Instance;

    private float _panSpeed = 0.01f;
    public bool FocussedOnPlayer = false;
    public static Dictionary<Direction, float> PanLimits = new Dictionary<Direction, float> { };

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _player;
    
    private float _maxXPercentageBoundary = 70f;
    private float _maxYPercentageBoundary = 70f;

    private Vector3 _dragOrigin;    //for camera dragging in editor


    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(_camera, "_camera", gameObject);

        if (GameManager.Instance.CurrentGameLevel != null)
            SetPanLimits(GameManager.Instance.CurrentGameLevel.LevelBounds);

        CalculateCameraPosition();
    }

    public void Start()
    {
        if(GameManager.Instance.CurrentGameLevel != null)
            SetPanLimits(GameManager.Instance.CurrentGameLevel.LevelBounds);

        CalculateCameraPosition();
    }

    public void SetZoomLevel(float zoomLevel)
    {
        _camera.orthographicSize = zoomLevel;
    }

    public void ResetCamera()
    {
        FocussedOnPlayer = false;

        if(!PanLimits.ContainsKey(Direction.Left) || !PanLimits.ContainsKey(Direction.Right) || !PanLimits.ContainsKey(Direction.Down) || !PanLimits.ContainsKey(Direction.Up))
        {
            return;
        }

        CalculateCameraPosition();
    }

    public void SetPanLimits(GridLocation levelBounds)
    {
        PanLimits.Clear();
        //TODO. the - .. value is currently hardcoded but would not work with different screen sizes or zoom levels
        PanLimits.Add(Direction.Up, levelBounds.Y - 3f);  // should depend on the furthest upper edge of the maze level.Never less than 4.
        PanLimits.Add(Direction.Right, levelBounds.X - 7f);// should depend on the furthest right edge of the maze level  Never less than 8.
        PanLimits.Add(Direction.Down, 4f); // should (with this zoom level) always have 4 as lowest boundary down. Should always be => 4
        PanLimits.Add(Direction.Left, 8f); // should (with this zoom level) always have 8 as the left most boundary. Should always be => 8

        // Set minima for small levels
        if (PanLimits[Direction.Up] < PanLimits[Direction.Down]) PanLimits[Direction.Up] = PanLimits[Direction.Down];
        if (PanLimits[Direction.Right] < PanLimits[Direction.Left]) PanLimits[Direction.Right] = PanLimits[Direction.Left];
    }

    public void FocusOnPlayer()
    {
        FocussedOnPlayer = true;

        PlayerCharacter player = null;

        if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
        {
            if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
                player = GameManager.Instance.CharacterManager.GetPlayerCharacter<MazePlayerCharacter>(PlayerNumber.Player1);
            else
            {
                player = GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>().FirstOrDefault(p => p.Value.PhotonView.IsMine).Value;
            }
        }
        else
        {
            if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
                player = GameManager.Instance.CharacterManager.GetPlayerCharacter<OverworldPlayerCharacter>(PlayerNumber.Player1);
            else
            {
                player = GameManager.Instance.CharacterManager.GetPlayers<OverworldPlayerCharacter>().FirstOrDefault(p => p.Value.PhotonView.IsMine).Value;
            }
        }

        if (player == null)
        {
            Logger.Error("Could not find player character on client");
        }

        _player = player.transform;

        transform.position = new Vector3(_player.position.x, _player.position.y, -10f);
    }

    void Update()
    {
        if (EditorManager.InEditor)
        {
            HandleMiddleMousePanning();
        }

        if (!FocussedOnPlayer) return;

        CalculateCameraPosition();
    }

    // Camera is static with player in the middle. Once the player get to the screen edges (the outer 30%) the camera follows the player.
    // If the camera/player reach the edge of the level, the camera movement is clamped so it will not move further in that direction
    private void CalculateCameraPosition()
    {
        Vector2 cameraPosition = new Vector2(transform.position.x, transform.position.y);

        Vector3 playerWorldToScreenPos = _camera.WorldToScreenPoint(_player.position);
        float playerWidthPercentagePosOnScreen = (playerWorldToScreenPos.x / Screen.width) * 100f;
        float playerHeightPercentagePosOnScreen = (playerWorldToScreenPos.y / Screen.height) * 100f;

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
}
