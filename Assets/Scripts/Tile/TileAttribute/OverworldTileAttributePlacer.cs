using UnityEngine;
public class OverworldTileAttributePlacer<T> : TileAttributePlacer<T> where T : OverworldTile
{
    public override T Tile { get; set; }

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
        mazeEntry.Tile = Tile;

        Tile.Walkable = true;
        Tile.TileAttributes.Add(mazeEntry);

        if(OverworldManager.Instance != null && OverworldManager.Instance.EditorOverworld != null)
        {
            OverworldManager.Instance.EditorOverworld.MazeEntries.Add(mazeEntry);
        }
    }
}
