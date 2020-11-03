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
        IMazeTileAttribute tileObstacle = (TileObstacle)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);
        if (tileObstacle == null)
        {
            tileAttributePlacer.RemovePlayerExit();
            tileAttributePlacer.RemoveEnemySpawnpoint();
            tileAttributePlacer.RemovePlayerSpawnpoint();

            tileAttributePlacer.PlaceTileObstacle(ObstacleType.Wall);
            return;
        }

        // Tile is already blocked
        tileAttributePlacer.RemoveTileObstacle();
    }
}
