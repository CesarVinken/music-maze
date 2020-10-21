using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    private List<GridLocation> _fingerPositions = new List<GridLocation>();
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private SpriteRenderer _currentTileSpriteRenderer;
    [SerializeField] private GameObject _currentTileSpriteGO;

    public bool IsDrawingPath = false;

    private GridLocation _selectedGridLocation;

    public void Awake()
    {
        if (_lineRenderer == null)
            Logger.Error(Logger.Initialisation, "Could not find _lineRenderer");

        if (_currentTileSpriteRenderer == null)
            Logger.Error(Logger.Initialisation, "Could not find _currentTileSpriteRenderer");

        Guard.CheckIsNull(_currentTileSpriteGO, "_currentTileSpriteGO");
    }

    public void Start()
    {
        this.enabled = false;
    }

    public void OnEnable()
    {
        IsDrawingPath = true;
        Logger.Log("PathDrawer enabled");
        _fingerPositions.Clear();
        _lineRenderer.positionCount = 0;
        
        CreateLine();

        _currentTileSpriteGO.SetActive(true);
    }

    public void OnDisable()
    {
        _currentTileSpriteGO.SetActive(false);

        // remove existing lines
        Logger.Log("PathDrawer disabled");
        _fingerPositions.Clear();
        _lineRenderer.positionCount = 0;

        IsDrawingPath = false;
    }

    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            UpdateLine();
        }
    }

    public void CreateLine()
    {
        Vector2 tempFingerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridLocation tempGridLocation = GridLocation.FindClosestGridTile(tempFingerPosition);
        Vector2 roundedTempGridLocation = GridLocation.GridToVector(tempGridLocation);

        _fingerPositions.Add(tempGridLocation);
        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, new Vector2(
            roundedTempGridLocation.x + GridLocation.OffsetToTileMiddle,
            roundedTempGridLocation.y + GridLocation.OffsetToTileMiddle
            ));

        _currentTileSpriteGO.transform.position = new Vector3(roundedTempGridLocation.x + GridLocation.OffsetToTileMiddle, roundedTempGridLocation.y + GridLocation.OffsetToTileMiddle, -8);
        _selectedGridLocation = tempGridLocation;
    }

    private void UpdateLine()
    {    
        Vector2 tempFingerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridLocation tempGridLocation = GridLocation.FindClosestGridTile(tempFingerPosition);

        if (!MazeLevelManager.Instance.Level.TilesByLocation.TryGetValue(tempGridLocation, out Tile tile)) return;

        if (!tile.Walkable)
        {
            GridLocation currentTileSpriteGOGridLocation = GridLocation.FindClosestGridTile(_currentTileSpriteGO.transform.position);
            if (tile.GridLocation.X == currentTileSpriteGOGridLocation.X && tile.GridLocation.Y == currentTileSpriteGOGridLocation.Y)
            {
                Logger.Log(Logger.Pathfinding, "The _currentTileSpriteGO is on an unaccessible tile. That probably means we moved the camera while drawing and the pointer moved onto unwalkable terrain. Let's clear out the path.");
                DisablePathDrawer(this);
            }
            return;
        }

        // only continue if we are selecting a different tile
        if (tempGridLocation.X == _fingerPositions[_fingerPositions.Count - 1].X &&
            tempGridLocation.Y == _fingerPositions[_fingerPositions.Count - 1].Y) return;

        if (!IsAdjacent(tempGridLocation, _selectedGridLocation)) return;

        Vector2 roundedTempGridLocation = GridLocation.GridToVector(tempGridLocation);

        // check if we are moving the pointer back on the already drawn path. In that case, remove the previous point from the path
        if (_fingerPositions.Count > 1 && 
            _fingerPositions[_fingerPositions.Count - 2].X == tempGridLocation.X &&
           _fingerPositions[_fingerPositions.Count - 2].Y == tempGridLocation.Y)
        {
            _fingerPositions.RemoveAt(_fingerPositions.Count - 1);
            _lineRenderer.positionCount = _lineRenderer.positionCount - 1;
        }
        else
        {
            _fingerPositions.Add(tempGridLocation);
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, new Vector2(
                roundedTempGridLocation.x + GridLocation.OffsetToTileMiddle,
                roundedTempGridLocation.y + GridLocation.OffsetToTileMiddle
                ));
        }

        _selectedGridLocation = tempGridLocation;
        _currentTileSpriteGO.transform.position = new Vector3(roundedTempGridLocation.x + GridLocation.OffsetToTileMiddle, roundedTempGridLocation.y + GridLocation.OffsetToTileMiddle, -8);
    }

    private bool IsAdjacent(GridLocation newLocation, GridLocation oldLocation)
    {
        if (GridLocation.IsOneAbove(newLocation, oldLocation)) return true;
        if (GridLocation.IsOneUnder(newLocation, oldLocation)) return true;
        if (GridLocation.IsOneLeft(newLocation, oldLocation)) return true;
        if (GridLocation.IsOneRight(newLocation, oldLocation)) return true;

        return false;
    }

    public List<GridLocation> GetDrawnPath()
    {
        return _fingerPositions;
    }

    // Invoked when the player character is moving and reaching new GridLocations while the player is drawing. The drawn path should be updated accordingly
    public void PlayerCurrentGridLocationUpdated(GridLocation playerCurrentGridLocation)
    {
        if (!isActiveAndEnabled)
            return;

        Logger.Warning("Player entered {0}, {1}, Update the drawn path", playerCurrentGridLocation.X, playerCurrentGridLocation.Y);

        // Make path longer if player character walks away from starting point that is being drawn.
        if (IsAdjacent(playerCurrentGridLocation, _fingerPositions[0]))
        {
            Logger.Warning("Add to path because the new player position is adjacent. Prepend {0},{1}", playerCurrentGridLocation.X, playerCurrentGridLocation.Y);

            List<GridLocation> tempFingerPositions = _fingerPositions.Prepend(playerCurrentGridLocation).ToList();
            _fingerPositions = tempFingerPositions;

            _lineRenderer.positionCount++;

            for (int i = _lineRenderer.positionCount - 1; i >= 1; i--)
            {
                Logger.Log("_lineRenderer.positionCount: {0}", _lineRenderer.positionCount);
                Logger.Log("set i {0} to {1},{2}", i, _lineRenderer.GetPosition(i - 1).x, _lineRenderer.GetPosition(i - 1).y);
                _lineRenderer.SetPosition(i, _lineRenderer.GetPosition(i - 1));
            }
            _lineRenderer.SetPosition(0, new Vector2(
                playerCurrentGridLocation.X + GridLocation.OffsetToTileMiddle,
                playerCurrentGridLocation.Y + GridLocation.OffsetToTileMiddle
                ));

            Logger.Log("set i 0 to {0},{1}", _lineRenderer.GetPosition(0).x, _lineRenderer.GetPosition(0).y);
        }
    }

    public static void EnablePathDrawer(PathDrawer pathDrawer)
    {
        pathDrawer.enabled = true;
    }

    public static void DisablePathDrawer(PathDrawer pathDrawer)
    {
        pathDrawer.enabled = false;
    }
}
