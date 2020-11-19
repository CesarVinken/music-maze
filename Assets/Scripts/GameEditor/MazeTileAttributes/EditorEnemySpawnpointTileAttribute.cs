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
