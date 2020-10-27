using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlocker : MonoBehaviour
{
    public Tile Tile;
    public string ParentId;

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
    }
    
}
