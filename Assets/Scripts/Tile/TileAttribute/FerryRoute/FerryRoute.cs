using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FerryRoute : MonoBehaviour, ITileAttribute
{
    public Tile Tile;
    public string ParentId;

    private int _sortingOrderBase;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    [SerializeField] private Ferry _ferry;
    [SerializeField] private FerryDocking _ferryDockingBegin;
    [SerializeField] private FerryDocking _ferryDockingEnd;

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

    public void Initialise(int dockingStartDirection, int dockingEndDirection)
    {
        _ferryDockingBegin.Initialise(this, FerryDockingType.DockingStart, dockingStartDirection);
        _ferryDockingEnd.Initialise(this, FerryDockingType.DockingEnd, dockingEndDirection);

        Direction dockingDirectionBegin = _ferryDockingBegin.GetDockingDirection();
        if (dockingDirectionBegin == Direction.Right || dockingDirectionBegin == Direction.Left)
        {
            _ferry.SetDirection(FerryDirection.Horizontal);
        }
        else
        {
            _ferry.SetDirection(FerryDirection.Vertical);
        }

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

    public List<FerryRoutePoint> GetFerryRoutePoints()
    {
        return _ferryRoutePoints;
    }

    // called in editor
    public void UpdateDocking()
    {
        if(_ferryRoutePoints.Count < 2)
        {
            _ferryDockingBegin.UpdateDockingDirection();
            _ferryDockingBegin.UpdateDockingSprite();
            _ferryDockingEnd.SetActive(false);

            Direction ferryDockingBeginDirection = _ferryDockingBegin.GetDockingDirection();
            if (ferryDockingBeginDirection == Direction.Right || ferryDockingBeginDirection == Direction.Left)
            {
                _ferry.SetDirection(FerryDirection.Horizontal);
            }
            else
            {
                _ferry.SetDirection(FerryDirection.Vertical);
            }
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
}
