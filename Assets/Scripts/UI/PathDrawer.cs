using System.Collections.Generic;
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
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, new Vector2(
            roundedTempGridLocation.x + GridLocation.OffsetToTileMiddle,
            roundedTempGridLocation.y + GridLocation.OffsetToTileMiddle
            ));
        _lineRenderer.SetPosition(1, new Vector2(
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

        if (!tile.Walkable) return;


        // only continue if we are selecting a different tile
        if (tempGridLocation.X == _fingerPositions[_fingerPositions.Count - 1].X &&
            tempGridLocation.Y == _fingerPositions[_fingerPositions.Count - 1].Y) return;

        if (!IsAdjacent(tempGridLocation, _selectedGridLocation)) return;

        Vector2 roundedTempGridLocation = GridLocation.GridToVector(tempGridLocation);

        // check if we are moving the pointer back on the path. In that case, remove the previous point from the path
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
        //Logger.Log("Added {0},{1}", tempGridLocation.X, tempGridLocation.Y);

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
}
