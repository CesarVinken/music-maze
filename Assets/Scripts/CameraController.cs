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
        { Direction.Up, 40f },
        { Direction.Right, 40f },
        { Direction.Down, -40f },
        { Direction.Left, -40f },
    };
    [SerializeField] private Camera _camera;

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

        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
    }

    public void Start()
    {
        Logger.Log("maze players {0}", CharacterManager.Instance.MazePlayers.Count);

    }

    void Update()
    {
        if (!FocussedOnPlayer) return;

        //Vector2 position = new Vector2(transform.position.x, transform.position.y);

        //// binding to the limits of the map
        //position.x = Mathf.Clamp(position.x, PanLimits[Direction.Left], PanLimits[Direction.Right]);
        //position.y = Mathf.Clamp(position.y, PanLimits[Direction.Down], PanLimits[Direction.Up]);

        //transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}
