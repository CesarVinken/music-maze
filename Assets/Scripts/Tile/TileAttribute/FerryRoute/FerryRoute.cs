using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FerryRoute : MonoBehaviour, ITileAttribute
{
    public Tile Tile;
    public string ParentId;

    private int _sortingOrderBase;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

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

        EditorMazeLevel editorMazeLevel = GameManager.Instance.CurrentEditorLevel as EditorMazeLevel;
        if (editorMazeLevel != null)
        {
            editorMazeLevel.FerryRoutes.Add(this);
        }

        Tile = tile;
        ParentId = tile.TileId;

        _ferryDockingBegin.Initialise(this, FerryDockingType.DockingStart);
        _ferryDockingEnd.Initialise(this, FerryDockingType.DockingEnd);
    }

    public void AddRouteLineRenderer()
    {
        EditorFerryRouteLineRenderer editorFerryRouteLineRenderer = gameObject.AddComponent<EditorFerryRouteLineRenderer>();
        _editorFerryRouteLineRenderer = editorFerryRouteLineRenderer;
        _editorFerryRouteLineRenderer.Initialise(this);
    }

    public void AddFerryRoutePoint(Tile tile)
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

    public void UpdateDocking()
    {
        if(_ferryRoutePoints.Count < 2)
        {
            _ferryDockingBegin.UpdateDockingSprite();
            _ferryDockingEnd.SetActive(false);
        }
        else
        {
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

}
