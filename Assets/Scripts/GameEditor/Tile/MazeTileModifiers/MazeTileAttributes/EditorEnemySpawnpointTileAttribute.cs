using System.Linq;
using UnityEngine;

public class EditorEnemySpawnpointTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Enemy Spawnpoint"; }

    public override void PlaceAttribute(EditorTile tile)
    {
        EditorTileAttributePlacer tileAttributePlacer = new EditorTileAttributePlacer(tile);
        TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);

        IMazeTileAttribute enemySpawnpoint = (EnemySpawnpoint)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is EnemySpawnpoint);
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
        return EditorUIContainer.Instance.TileAttributeIcons[3];
    }
}
