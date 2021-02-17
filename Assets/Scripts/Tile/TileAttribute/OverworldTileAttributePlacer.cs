using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OverworldTileAttributePlacer<T> : TileAttributePlacer<T> where T : OverworldTile
{
    public override T Tile { get; set; }

    public override ITileAttribute InstantiateTileAttributeGO<U>()
    {
        GameObject tileAttributeGO = GameObject.Instantiate(OverworldManager.Instance.GetTileAttributePrefab<U>(), Tile.transform);
        return tileAttributeGO.GetComponent<U>();
    }
}
