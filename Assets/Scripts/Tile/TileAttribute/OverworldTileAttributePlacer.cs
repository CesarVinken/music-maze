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

        Tile.SetWalkable(true);
        Tile.AddAttribute(playerSpawnpoint);
    }

    public virtual MazeLevelEntry PlaceMazeLevelEntry(string mazeLevelName = "")
    {
        MazeLevelEntry mazeLevelEntry = (MazeLevelEntry)InstantiateTileAttributeGO<MazeLevelEntry>();
        mazeLevelEntry.Tile = Tile;
        mazeLevelEntry.MazeLevelName = mazeLevelName;

        Tile.SetWalkable(true);
        Tile.AddAttribute(mazeLevelEntry);

        return mazeLevelEntry;
    }
}
