﻿using System.Collections;
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
    public static Dictionary<Direction, float> PanLimits = new Dictionary<Direction, float> { };

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

        _cameraBoundsOffset = _camera.ScreenToWorldPoint(new Vector3(
            Screen.width - (Screen.width * (1 - _maxXPercentageBoundary / 100f)),
            Screen.height - (Screen.height * (1 - _maxYPercentageBoundary / 100f)),
            0));
    }

    public void Start()
    {
        if(GameManager.Instance.CurrentGameLevel != null)
            SetPanLimits(GameManager.Instance.CurrentGameLevel.LevelBounds);
    }

    public void SetZoomLevel(float zoomLevel)
    {
        _camera.orthographicSize = zoomLevel;
    }

    public void ResetCamera()
    {
        FocussedOnPlayer = false;

        Vector3 cameraPosition = new Vector3(0, 0, -10);

        if(!PanLimits.ContainsKey(Direction.Left) || !PanLimits.ContainsKey(Direction.Right) || !PanLimits.ContainsKey(Direction.Down) || !PanLimits.ContainsKey(Direction.Up))
        {
            return;
        }

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, PanLimits[Direction.Left], PanLimits[Direction.Right]);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, PanLimits[Direction.Down], PanLimits[Direction.Up]);

        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, cameraPosition.z);
    }

    public void SetPanLimits(GridLocation levelBounds)
    {
        PanLimits.Clear();
        //TODO. the - .. value is currently hardcoded but would not work with different screen sizes or zoom levels
        PanLimits.Add(Direction.Up, levelBounds.Y - 3f);  // should depend on the furthest upper edge of the maze level.Never less than 4.
        PanLimits.Add(Direction.Right, levelBounds.X - 7f);// should depend on the furthest right edge of the maze level  Never less than 8.
        PanLimits.Add(Direction.Down, 4f); // should (with this zoom level) always have 4 as lowest boundary down. Should always be => 4
        PanLimits.Add(Direction.Left, 8f); // should (with this zoom level) always have 8 as the left most boundary. Should always be => 8

        if (levelBounds.Y < 4) PanLimits[Direction.Up] = 4f;
        if (levelBounds.X < 8) PanLimits[Direction.Right] = 8f;
    }

    public void FocusOnPlayer()
    {
        FocussedOnPlayer = true;

        PlayerCharacter player = null;

        if (GameManager.CurrentSceneType == SceneType.Maze)
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
            //Logger.Warning($"cameraPosition.y {cameraPosition.y}");
            //if (cameraPosition.y == _player.position.y - _cameraBoundsOffset.y)
            //{
            //    Logger.Warning("Got you!");
            //}
            cameraPosition.y = _player.position.y + _cameraBoundsOffset.y;
        }

        // binding to the limits of the map
        //cameraPosition.x = Mathf.Clamp(cameraPosition.x, PanLimits[Direction.Left], PanLimits[Direction.Right]);
        //cameraPosition.y = Mathf.Clamp(cameraPosition.y, PanLimits[Direction.Down], PanLimits[Direction.Up]);

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
