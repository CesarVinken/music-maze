using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldTileBackgroundPlacer<T> : TileBackgroundPlacer<T> where T : OverworldTile
{
    public override T Tile { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public override void PlacePath(IPathType tilePathType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        throw new System.NotImplementedException();
    }
}
