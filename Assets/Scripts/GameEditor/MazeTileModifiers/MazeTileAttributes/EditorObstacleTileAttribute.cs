using System.Linq;

public class EditorObstacleTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Obstacle"; }

    public override void PlaceAttribute(Tile tile)
    {
        TileAttributePlacer tileAttributePlacer = new TileAttributePlacer(tile);
        TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);

        IMazeTileAttribute tileObstacle = (TileObstacle)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);
        if (tileObstacle == null)
        {
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemoveEnemySpawnpoint();
            tileAttributeRemover.RemovePlayerSpawnpoint();

            tileAttributePlacer.PlaceTileObstacle(ObstacleType.Wall);
            return;
        }

        // Tile is already blocked
        tileAttributeRemover.RemoveTileObstacle();
    }
}
