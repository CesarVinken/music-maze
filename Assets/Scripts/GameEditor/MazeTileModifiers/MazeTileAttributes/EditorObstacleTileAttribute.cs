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
            tileAttributeRemover.RemoveEnemySpawnpoint();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemovePlayerOnlyAttribute();
            tileAttributeRemover.RemovePlayerSpawnpoint();

            TileBackgroundRemover tileBackgroundRemover = new TileBackgroundRemover(tile);
            tileBackgroundRemover.RemovePath();

            tileAttributePlacer.PlaceTileObstacle(ObstacleType.Wall);
            return;
        }

        // Tile is already blocked
        tileAttributeRemover.RemoveTileObstacle();
    }
}
