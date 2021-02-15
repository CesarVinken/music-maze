using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldTile : Tile
{
    public override TileObstacle TryGetTileObstacle()
    {
        throw new System.NotImplementedException();
    }

    public override TilePath TryGetTilePath()
    {
        throw new System.NotImplementedException();
    }
}
