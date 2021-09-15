using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FerryRoute : MonoBehaviour, ITileAttribute
{
    public Tile Tile;
    public string ParentId;

    private int _sortingOrderBase;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    [SerializeField] private GameObject _dockingBeginGO;
    [SerializeField] private GameObject _dockingEndGO;
    [SerializeField] private SpriteRenderer _dockingBeginSpriteRenderer;
    [SerializeField] private SpriteRenderer _dockingEndSpriteRenderer;

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

        UpdateDockingEnd();
    }

    public void RemoveFerryRoutePoint(Tile tile)
    {
        FerryRoutePoint ferryRoutePoint = _ferryRoutePoints.FirstOrDefault(point => point.Tile.TileId == tile.TileId);

        if (ferryRoutePoint == null) return;

        _ferryRoutePoints.Remove(ferryRoutePoint);
        _editorFerryRouteLineRenderer?.UpdateLineRenderer(_ferryRoutePoints);

        UpdateDockingEnd();
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

    public void UpdateDockingEnd()
    {
        if(_ferryRoutePoints.Count < 2)
        {
            _dockingEndGO.SetActive(false);
            return;
        }
        
        _dockingEndGO.transform.position = _ferryRoutePoints[_ferryRoutePoints.Count - 1].Tile.transform.position;
        _dockingEndGO.SetActive(true);
        
        // TODO: ADD DOCKING SPRITES IN 4 DIRECTIONS
        //_dockingEndSpriteRenderer.sprite = MazeSpriteManager.Instance.DockingPointSprites[0];
    }
}
