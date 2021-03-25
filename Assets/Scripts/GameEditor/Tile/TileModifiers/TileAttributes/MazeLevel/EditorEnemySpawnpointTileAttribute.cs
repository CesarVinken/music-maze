﻿using System.Linq;
using UnityEngine;

public class EditorEnemySpawnpointTileAttribute : EditorMazeTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Enemy Spawnpoint"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        if (tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }
        
        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute enemySpawnpoint = (EnemySpawnpoint)tile.GetAttributes().FirstOrDefault(attribute => attribute is EnemySpawnpoint);
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
