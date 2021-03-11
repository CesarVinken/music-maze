﻿using UnityEngine;

public class GroundMainMaterial : MonoBehaviour, ITileMainMaterial
{
    public Tile Tile;
    public string ParentId;

    public void SetTile(Tile tile)
    {
        Tile = tile;
        ParentId = tile.TileId;
    }
}
