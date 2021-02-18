using System.Collections.Generic;
using UnityEngine;

public class EditorOverworldTileAttributePlacer : OverworldTileAttributePlacer<EditorOverworldTile>
{
    private EditorOverworldTile _tile;

    public override EditorOverworldTile Tile { get => _tile; set => _tile = value; }

    public EditorOverworldTileAttributePlacer(EditorOverworldTile tile)
    {
        Tile = tile;
    }

    public override ITileAttribute InstantiateTileAttributeGO<U>()
    {
        GameObject tileAttributeGO = GameObject.Instantiate(OverworldManager.Instance.GetTileAttributePrefab<U>(), Tile.transform);
        return tileAttributeGO.GetComponent<U>();
    }

    public void PlacePlayerSpawnpoint()
    {
        PlayerSpawnpoint playerSpawnpoint = (PlayerSpawnpoint)InstantiateTileAttributeGO<PlayerSpawnpoint>();

        Tile.Walkable = true;
        Tile.TileAttributes.Add(playerSpawnpoint);
    }

    public void PlaceMazeEntry()
    {
        MazeEntry mazeEntry = (MazeEntry)InstantiateTileAttributeGO<MazeEntry>();

        Tile.Walkable = true;
        Tile.TileAttributes.Add(mazeEntry);
    }
}
