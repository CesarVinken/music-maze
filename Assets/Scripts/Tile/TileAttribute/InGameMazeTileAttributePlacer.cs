public class InGameMazeTileAttributePlacer : MazeTileAttributePlacer<InGameMazeTile>
{
    private InGameMazeTile _tile;

    public override InGameMazeTile Tile { get => _tile; set => _tile = value; }

    public InGameMazeTileAttributePlacer(InGameMazeTile tile)
    {
        Tile = tile;
    }

    public void PlacePlayerSpawnpoint()
    {
        Logger.Log("TODO: only make not markable if there is a player for the spawnpoint");
        PlayerSpawnpoint playerSpawnpoint = (PlayerSpawnpoint)InstantiateTileAttributeGO<PlayerSpawnpoint>();

        Tile.Walkable = true;

        Tile.TileAttributes.Add(playerSpawnpoint);
    }
}