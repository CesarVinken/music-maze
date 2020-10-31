using System.Linq;
using UnityEngine;

public class EditorObstacleTileAttribute : IEditorMazeTileAttribute
{
    public string Name { get => "Obstacle"; }
    public Sprite Sprite { get => null; }
    public EditorMazeTileAttributeType AttributeType { get => EditorMazeTileAttributeType.Obstacle; }

    public void PlaceAttribute(Tile tile)
    {
        IMazeTileAttribute tileObstacle = (TileObstacle)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);
        if (tileObstacle == null)
        {
            tile.RemovePlayerExit();
            tile.RemoveEnemySpawnpoint();
            tile.RemovePlayerSpawnpoint();

            tile.PlaceTileObstacle();
            return;
        }

        // Tile is already blocked
        tile.RemoveTileObstacle();
    }
}
