using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOverworldTileBackgroundPlacer : OverworldTileBackgroundPlacer<EditorOverworldTile>
{
    private EditorOverworldTile _tile;

    public override EditorOverworldTile Tile { get => _tile; set => _tile = value; }

    public EditorOverworldTileBackgroundPlacer(EditorOverworldTile tile)
    {
        Tile = tile;
    }

    
}
