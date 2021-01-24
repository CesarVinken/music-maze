using UnityEngine;

public class InGameTileAttributePlacer : TileAttributePlacer<InGameTile>
{
    private InGameTile _tile;

    public override InGameTile Tile { get => _tile; set => _tile = value; }

    public InGameTileAttributePlacer(InGameTile tile)
    {
        Tile = tile;
    }

    public override IMazeTileAttribute InstantiateTileAttributeGO<U>()
    {
        GameObject tileAttributeGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileAttributePrefab<U>(), Tile.transform);
        return tileAttributeGO.GetComponent<U>();
    }

    public void PlacePlayerSpawnpoint()
    {
        Logger.Log("TODO: only make not markable if there is a player for the spawnpoint");
        PlayerSpawnpoint playerSpawnpoint = (PlayerSpawnpoint)InstantiateTileAttributeGO<PlayerSpawnpoint>();

        Tile.Walkable = true;
        //Tile.TryMakeMarkable(false);

        Tile.MazeTileAttributes.Add(playerSpawnpoint);
    }
}