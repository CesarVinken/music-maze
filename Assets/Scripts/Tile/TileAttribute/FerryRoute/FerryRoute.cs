using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FerryRoute : MonoBehaviour, ITileAttribute
{
    public Tile Tile;
    public string ParentId;
    public string Id;

    private int _sortingOrderBase;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    [SerializeField] private Ferry _ferry;
    [SerializeField] private FerryDocking _ferryDockingBegin;
    [SerializeField] private FerryDocking _ferryDockingEnd;

    public FerryRouteDirection FerryRouteDirection;
    public GameObject FerryRoutePointSpritePrefab;

    private List<FerryRoutePoint> _ferryRoutePoints = new List<FerryRoutePoint>();
    private EditorFerryRouteLineRenderer _editorFerryRouteLineRenderer;

    public void Remove()
    {
        EditorMazeLevel editorMazeLevel = GameManager.Instance.CurrentEditorLevel as EditorMazeLevel;
        if (editorMazeLevel != null)
        {
            editorMazeLevel.FerryRoutes.Remove(this);
        }

        Destroy(this);
        Destroy(gameObject);
    }

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        if (EditorManager.InEditor)
        {
            GameManager.Instance.CurrentEditorLevel.FerryRoutes.Add(this);
        }
        else 
        {
            GameManager.Instance.CurrentGameLevel.FerryRoutes.Add(this);
        }


        Tile = tile;
        ParentId = tile.TileId;
    }

    public void SetDirection(int ferryRouteDirection)
    {
        if(ferryRouteDirection == 0 || ferryRouteDirection == 2)
        {
            FerryRouteDirection = FerryRouteDirection.Horizontal;
        }
        else
        {
            FerryRouteDirection = FerryRouteDirection.Vertical;
        }
    }

    public void Initialise(string id, int startDirection)
    {
        Id = id;
        SetDirection(startDirection);

        _ferryDockingBegin.Initialise(this, FerryDockingType.DockingStart, startDirection);
        _ferryDockingEnd.Initialise(this, FerryDockingType.DockingEnd, startDirection);

        _ferry.SetDirection(FerryRouteDirection);

        if (!EditorManager.InEditor)
        {
            _ferry.Initialise(this);
        }
    }

    public void AddRouteLineRenderer()
    {
        EditorFerryRouteLineRenderer editorFerryRouteLineRenderer = gameObject.AddComponent<EditorFerryRouteLineRenderer>();
        _editorFerryRouteLineRenderer = editorFerryRouteLineRenderer;
        _editorFerryRouteLineRenderer.Initialise(this);
    }

    public void AddFerryRoutePointInGame(Tile tile)
    {
        _ferryRoutePoints.Add(new FerryRoutePoint(tile));
    }

    public void AddFerryRoutePointInEditor(Tile tile)
    {
        _ferryRoutePoints.Add(new FerryRoutePoint(tile));
        _editorFerryRouteLineRenderer?.UpdateLineRenderer(_ferryRoutePoints);
        UpdateDocking();
    }

    public void RemoveFerryRoutePoint(Tile tile)
    {
        FerryRoutePoint ferryRoutePoint = _ferryRoutePoints.FirstOrDefault(point => point.Tile.TileId == tile.TileId);

        if (ferryRoutePoint == null) return;

        _ferryRoutePoints.Remove(ferryRoutePoint);
        _editorFerryRouteLineRenderer?.UpdateLineRenderer(_ferryRoutePoints);

        UpdateDocking();
    }

    // remove all tiles from the route up to the given tile
    public void RemoveFerryRoutePoints(FerryRoutePoint newLastPoint)
    {
        if(_ferryRoutePoints.Count < 1)
        {
            return;
        }

        EditorTileSelector.Instance?.ResetColouredTiles();

        for (int i = _ferryRoutePoints.Count - 1; i >= 0; i--)
        {
            if(newLastPoint == _ferryRoutePoints[i])
            {
                _editorFerryRouteLineRenderer?.UpdateLineRenderer(_ferryRoutePoints);
                FerryRouteDrawingModeAccessor.Instance?.ColourAddableTiles();
                break;
            }
            RemoveFerryRoutePoint(_ferryRoutePoints[i].Tile);
        }
    }

    public Ferry GetFerry()
    {
        return _ferry;
    }

    public List<FerryRoutePoint> GetFerryRoutePoints()
    {
        return _ferryRoutePoints;
    }

    public FerryRoutePoint GetNextFerryRoutePoint(FerryRoutePoint currentFerryRoutePoint)
    {
        int indexCurrentFerryRoutePoint = _ferryRoutePoints.IndexOf(currentFerryRoutePoint);

        if(indexCurrentFerryRoutePoint == -1 || indexCurrentFerryRoutePoint == _ferryRoutePoints.Count - 1)
        {
            return null;
        }
        return _ferryRoutePoints[indexCurrentFerryRoutePoint + 1];
    }

    public FerryRoutePoint GetPreviousFerryRoutePoint(FerryRoutePoint currentFerryRoutePoint)
    {
        int indexCurrentFerryRoutePoint = _ferryRoutePoints.IndexOf(currentFerryRoutePoint);

        if (indexCurrentFerryRoutePoint == -1 || indexCurrentFerryRoutePoint == 0)
        {
            return null;
        }
        return _ferryRoutePoints[indexCurrentFerryRoutePoint - 1];
    }

    public FerryRoutePoint GetFerryRoutePointForPlayerLocation(GridLocation playerCharacterLocation, FerryRoutePoint currentFerryRoutePoint)
    {
        FerryRoutePoint oldFerryRoutePoint = currentFerryRoutePoint;
        List<FerryRoutePoint> ferryRoutePoints = _ferryRoutePoints;
        int indexOldFerryRoutePoint = ferryRoutePoints.IndexOf(oldFerryRoutePoint);
        if (indexOldFerryRoutePoint == 0) // we're at the beginning
        {
            FerryRoutePoint nextPointOnRoute = ferryRoutePoints[indexOldFerryRoutePoint + 1];
            if (nextPointOnRoute.Tile.GridLocation.X == playerCharacterLocation.X &&
                nextPointOnRoute.Tile.GridLocation.Y == playerCharacterLocation.Y)
            {
                // We moved to the next point on the route
                return nextPointOnRoute;
            }
        }
        else if (indexOldFerryRoutePoint == ferryRoutePoints.Count - 1) // we're at the end
        {
            FerryRoutePoint previousPointOnRoute = ferryRoutePoints[indexOldFerryRoutePoint - 1];
            if (previousPointOnRoute.Tile.GridLocation.X == playerCharacterLocation.X &&
                previousPointOnRoute.Tile.GridLocation.Y == playerCharacterLocation.Y)
            {
                // We moved to the previous point on the route
                return previousPointOnRoute;
            }
        }
        else // we're in the middle
        {
            if (ferryRoutePoints[indexOldFerryRoutePoint + 1].Tile.GridLocation.X == playerCharacterLocation.X &&
                ferryRoutePoints[indexOldFerryRoutePoint + 1].Tile.GridLocation.Y == playerCharacterLocation.Y)
            {
                // We moved to the next point on the route
                return ferryRoutePoints[indexOldFerryRoutePoint + 1];
            }
            else if (ferryRoutePoints[indexOldFerryRoutePoint - 1].Tile.GridLocation.X == playerCharacterLocation.X &&
                ferryRoutePoints[indexOldFerryRoutePoint - 1].Tile.GridLocation.Y == playerCharacterLocation.Y)
            {
                // We moved to the previous point on the route
                return ferryRoutePoints[indexOldFerryRoutePoint - 1];
            }
        }
        return null;
    }

    // called in editor
    public void UpdateDocking()
    {
        if (_ferryRoutePoints.Count < 2)
        {
            _ferryDockingBegin.UpdateDockingDirection();
            _ferryDockingBegin.UpdateDockingSprite();
            _ferryDockingEnd.SetActive(false);

            FerryRouteDirection = FerryRouteDirection.Horizontal;
            _ferry.SetDirection(FerryRouteDirection);

        }
        else if (_ferryRoutePoints.Count == 2) // Two points is the minimum to have a direction. If there is a change in ferry route direction, it happens at two points
        {
            _ferryDockingBegin.UpdateDockingDirection();
            _ferryDockingBegin.UpdateDockingSprite();

            _ferryDockingEnd.UpdateDockingDirection();
            _ferryDockingEnd.UpdateDockingSprite();
            _ferryDockingEnd.gameObject.transform.position = _ferryRoutePoints[_ferryRoutePoints.Count - 1].Tile.transform.position;
            _ferryDockingEnd.SetActive(true);


            Direction dockingEndDirection = _ferryDockingEnd.GetDockingDirection();
            Logger.Log($"dockingEndDirection is {dockingEndDirection}");
            if (dockingEndDirection == Direction.Right || dockingEndDirection == Direction.Left)
            {
                FerryRouteDirection = FerryRouteDirection.Horizontal;
            }
            else
            {
                FerryRouteDirection = FerryRouteDirection.Vertical;
            }
            _ferry.SetDirection(FerryRouteDirection);
        }
        else
        {
            _ferryDockingEnd.UpdateDockingDirection();
            _ferryDockingEnd.UpdateDockingSprite();
            _ferryDockingEnd.gameObject.transform.position = _ferryRoutePoints[_ferryRoutePoints.Count - 1].Tile.transform.position;
            _ferryDockingEnd.SetActive(true);
        }    
    }

    public void TryTurnDocking(FerryDockingType ferryDockingType)
    {
        if (ferryDockingType == FerryDockingType.DockingStart)
        {
            _ferryDockingBegin.TryTurn();
        }
        else
        {
            _ferryDockingEnd.TryTurn();
        }
    }

    public FerryDocking GetFerryDocking(FerryDockingType ferryDockingType)
    {
        switch (ferryDockingType)
        {
            case FerryDockingType.DockingStart:
                return _ferryDockingBegin;
            case FerryDockingType.DockingEnd:
                return _ferryDockingEnd;
            default:
                Logger.Error($"Requested unknown ferryDockingType '{ferryDockingType}'");
                return _ferryDockingBegin;
        }
    }

    public FerryRoutePoint GetFerryRoutePointByLocation(GridLocation gridLocation)
    {
        List<FerryRoutePoint> ferryRoutePoints = GetFerryRoutePoints();
        for (int i = 0; i < ferryRoutePoints.Count; i++)
        {
            if (ferryRoutePoints[i].Tile.GridLocation.X == gridLocation.X &&
                ferryRoutePoints[i].Tile.GridLocation.Y == gridLocation.Y)
            {
                return ferryRoutePoints[i];
            }
        }
        return null;
    }
}
