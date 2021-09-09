using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryRoute : MonoBehaviour, ITileAttribute
{
    public Tile Tile;
    public string ParentId;

    private int _sortingOrderBase;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    [SerializeField] private GameObject _dockingBeginGO;
    [SerializeField] private GameObject _dockingEndGO;

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
}

public class FerryRoutePoint
{

}