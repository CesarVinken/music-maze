using System.Collections;
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

    private float _panSpeed;
    public bool FocussedOnPlayer = false;
    public static Dictionary<Direction, float> PanLimits = new Dictionary<Direction, float>
    {
        { Direction.Up, 6f }, // should depend on the furthest upper edge of the maze level. Should always be => 4
        { Direction.Right, 12f }, // should depend on the furthest right edge of the maze level Should always be => 8
        { Direction.Down, 4f }, // should (with this zoom level) always have 4 as lowest boundary down. Never less than 4.
        { Direction.Left, 8f }, // should (with this zoom level) always have 8 as the left most boundary. Never less than 8.
    };
    [SerializeField] private Camera _camera;
    private Transform _player;

    public void Awake()
    {
        Instance = this;
        
        if (_camera == null)
            Logger.Error(Logger.Initialisation, "Could not find main camera");
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
        if (!FocussedOnPlayer) return;

        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        position.x = _player.position.x;
        position.y = _player.position.y;

        // binding to the limits of the map
        position.x = Mathf.Clamp(position.x, PanLimits[Direction.Left], PanLimits[Direction.Right]);
        position.y = Mathf.Clamp(position.y, PanLimits[Direction.Down], PanLimits[Direction.Up]);

        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}
