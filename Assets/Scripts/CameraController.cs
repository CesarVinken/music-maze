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

    public GameObject PathDrawerGO;
    public PathDrawer PathDrawer;

    private float _panSpeed;
    public bool FocussedOnPlayer = false;
    public static Dictionary<Direction, float> PanLimits = new Dictionary<Direction, float>
    {

    };
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _player;
    
    private float _maxXPercentageBoundary = 70f;
    private float _maxYPercentageBoundary = 70f;
    private Vector2 _cameraBoundsOffset; // with this offset the camera is calculated when player is past the edge margin by calculating cameraPosition = _player.position - offset. 

    private Vector3 _dragOrigin;    //for camera dragging in editor


    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(_camera, "_camera", gameObject);
        Guard.CheckIsNull(PathDrawer, "PathDrawer", gameObject);

        Guard.CheckIsNull(PathDrawerGO, "PathDrawerGO", gameObject);

        _cameraBoundsOffset = _camera.ScreenToWorldPoint(new Vector3(
            Screen.width - (Screen.width * (1 - _maxXPercentageBoundary / 100f)),
            Screen.height - (Screen.height * (1 - _maxYPercentageBoundary / 100f)),
            0));
    }

    public void Start()
    {
        SetPanLimits(MazeLevelManager.Instance.Level.LevelBounds);
    }

    public void ResetCamera()
    {
        FocussedOnPlayer = false;

        Vector3 cameraPosition = new Vector3(0, 0, -10);

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, PanLimits[Direction.Left], PanLimits[Direction.Right]);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, PanLimits[Direction.Down], PanLimits[Direction.Up]);

        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, cameraPosition.z);
    }

    public void SetPanLimits(GridLocation levelBounds)
    {
        PanLimits.Clear();
        //TODO. the - .. value is currently hardcoded but would not work with different screen sizes or zoom levels
        PanLimits.Add(Direction.Up, levelBounds.Y - 4f);  // should depend on the furthest upper edge of the maze level.Never less than 4.
        PanLimits.Add(Direction.Right, levelBounds.X - 7f);// should depend on the furthest right edge of the maze level  Never less than 8.
        PanLimits.Add(Direction.Down, 4f); // should (with this zoom level) always have 4 as lowest boundary down. Should always be => 4
        PanLimits.Add(Direction.Left, 8f); // should (with this zoom level) always have 8 as the left most boundary. Should always be => 8

        if (levelBounds.Y < 4) PanLimits[Direction.Up] = 4f;
        if (levelBounds.X < 8) PanLimits[Direction.Right] = 8f;
    }

    public void FocusOnPlayer()
    {
        FocussedOnPlayer = true;

        PlayerCharacter player = GameManager.Instance.GameType == GameType.SinglePlayer ? CharacterManager.Instance.MazePlayers[0] : CharacterManager.Instance.MazePlayers.FirstOrDefault(p => p.PhotonView.IsMine == true);

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

        Vector2 cameraPosition = new Vector2(transform.position.x, transform.position.y);

        // Check if the player has walked to far to the edge. If so, compensate by following the player in that direction with the camera.
        Vector3 playerWorldToScreenPos = _camera.WorldToScreenPoint(_player.position);
        float playerWidthPercentagePosOnScreen = (playerWorldToScreenPos.x / Screen.width) * 100f;
        float playerHeightPercentagePosOnScreen = (playerWorldToScreenPos.y / Screen.height) * 100f;

        if (playerWidthPercentagePosOnScreen >= _maxXPercentageBoundary)
        {
            cameraPosition.x = _player.position.x - _cameraBoundsOffset.x;
        }
        else if (playerWidthPercentagePosOnScreen <= 100 - _maxXPercentageBoundary)
        {
            cameraPosition.x = _player.position.x + _cameraBoundsOffset.x;
        }

        if (playerHeightPercentagePosOnScreen >= _maxYPercentageBoundary)
        {
            cameraPosition.y = _player.position.y - _cameraBoundsOffset.y;
        }
        else if (playerHeightPercentagePosOnScreen <= 100 - _maxYPercentageBoundary)
        {
            cameraPosition.y = _player.position.y + _cameraBoundsOffset.y;
        }

        // binding to the limits of the map
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, PanLimits[Direction.Left], PanLimits[Direction.Right]);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, PanLimits[Direction.Down], PanLimits[Direction.Up]);

        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
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
