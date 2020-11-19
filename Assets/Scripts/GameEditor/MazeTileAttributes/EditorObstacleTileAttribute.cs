using System.Linq;
using UnityEngine;

public class EditorObstacleTileAttribute : IEditorMazeTileAttribute
{
    public string Name { get => "Obstacle"; }
    public Sprite Sprite { get => null; }

    public EditorMazeTileAttributeType AttributeType { get => EditorMazeTileAttributeType.Obstacle; }

    public void PlaceAttribute(Tile tile)
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
