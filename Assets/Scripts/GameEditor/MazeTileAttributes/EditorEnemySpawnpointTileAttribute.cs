using System.Linq;
using UnityEngine;

public class EditorEnemySpawnpointTileAttribute : IEditorMazeTileAttribute
{
    public string Name { get => "Enemy Spawnpoint"; }

    public Sprite Sprite { get => null; }

    public EditorMazeTileAttributeType AttributeType { get => EditorMazeTileAttributeType.EnemySpawnpoint; }

    public void PlaceAttribute(Tile tile)
    {
        TileAttributePlacer tileAttributePlacer = new TileAttributePlacer(tile);
        IMazeTileAttribute enemySpawnpoint = (EnemySpawnpoint)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is EnemySpawnpoint);
        if (enemySpawnpoint == null)
        {
            tileAttributePlacer.RemoveTileObstacle();
            tileAttributePlacer.RemovePlayerExit();
            tileAttributePlacer.RemovePlayerSpawnpoint();

            tileAttributePlacer.PlaceEnemySpawnpoint();
            return;
        }

        tileAttributePlacer.RemoveEnemySpawnpoint();
    }
}
