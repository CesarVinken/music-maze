using System.Linq;
using UnityEngine;

public class EditorEnemySpawnpointTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Enemy Spawnpoint"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        EditorTileAttributePlacer tileAttributePlacer = new EditorTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute enemySpawnpoint = (EnemySpawnpoint)tile.TileAttributes.FirstOrDefault(attribute => attribute is EnemySpawnpoint);
        if (enemySpawnpoint == null)
        {
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemovePlayerOnlyAttribute();
            tileAttributeRemover.RemovePlayerSpawnpoint();
            tileAttributeRemover.RemoveTileObstacle();

            tileAttributePlacer.PlaceEnemySpawnpoint();
            return;
        }

        tileAttributeRemover.RemoveEnemySpawnpoint();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[3];
    }
}
