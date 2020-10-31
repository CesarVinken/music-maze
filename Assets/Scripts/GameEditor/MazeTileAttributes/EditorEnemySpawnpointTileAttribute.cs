using System.Linq;
using UnityEngine;

public class EditorEnemySpawnpointTileAttribute : IEditorMazeTileAttribute
{
    public string Name { get => "Enemy Spawnpoint"; }

    public Sprite Sprite { get => null; }

    public EditorMazeTileAttributeType AttributeType { get => EditorMazeTileAttributeType.EnemySpawnpoint; }

    public void PlaceAttribute(Tile tile)
    {
        IMazeTileAttribute enemySpawnpoint = (EnemySpawnpoint)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is EnemySpawnpoint);
        if (enemySpawnpoint == null)
        {
            tile.RemoveTileObstacle();
            tile.RemovePlayerExit();
            tile.RemovePlayerSpawnpoint();

            tile.PlaceEnemySpawnpoint();
            return;
        }

        tile.RemoveEnemySpawnpoint();
    }
}
