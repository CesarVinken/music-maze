using System.Linq;

public class EditorPlayerSpawnpointTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Player Spawnpoint"; }

    public override void PlaceAttribute(Tile tile)
    {
        TileAttributePlacer tileAttributePlacer = new TileAttributePlacer(tile);
        TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);

        IMazeTileAttribute playerSpawnpoint = (PlayerSpawnpoint)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null)
        {
            tileAttributeRemover.RemoveTileObstacle();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemoveEnemySpawnpoint();

            tileAttributePlacer.PlacePlayerSpawnpoint();
            return;
        }

        tileAttributeRemover.RemovePlayerSpawnpoint();
    }
}
