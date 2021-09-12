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

    private List<FerryRoutePoint> _ferryRoutePoints = new List<FerryRoutePoint>();

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
    }

    public void AddFerryRoutePoint(Tile tile)
    {
        _ferryRoutePoints.Add(new FerryRoutePoint(tile));
        Logger.Log($"Number of points : {_ferryRoutePoints.Count}");
    }

    public void RemoveFerryRoutePoint(Tile tile)
    {
        FerryRoutePoint ferryRoutePoint = _ferryRoutePoints.FirstOrDefault(point => point.Tile.TileId == tile.TileId);

        if (ferryRoutePoint == null) return;

        _ferryRoutePoints.Remove(ferryRoutePoint);
    }

    public List<FerryRoutePoint> GetFerryRoutePoints()
    {
        return _ferryRoutePoints;
    }
}
