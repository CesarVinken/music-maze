using System.Linq;

public class EditorEnemySpawnpointTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Enemy Spawnpoint"; }

    public override void PlaceAttribute(Tile tile)
    {
        TileAttributePlacer tileAttributePlacer = new TileAttributePlacer(tile);
        TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);

        IMazeTileAttribute enemySpawnpoint = (EnemySpawnpoint)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is EnemySpawnpoint);
        if (enemySpawnpoint == null)
        {
            tileAttributeRemover.RemoveTileObstacle();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemovePlayerSpawnpoint();

            tileAttributePlacer.PlaceEnemySpawnpoint();
            return;
        }

        tileAttributeRemover.RemoveEnemySpawnpoint();
    }
}
