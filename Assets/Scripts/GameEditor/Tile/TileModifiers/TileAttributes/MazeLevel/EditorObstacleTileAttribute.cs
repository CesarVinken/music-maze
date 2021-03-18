﻿using System.Linq;
using UnityEngine;

public class EditorObstacleTileAttribute : EditorMazeTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Obstacle"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute tileObstacle = (TileObstacle)tile.GetAttributes().FirstOrDefault(attribute => (attribute is TileObstacle && !(attribute is PlayerExit)));
        if (tileObstacle == null)
        {
            tileAttributeRemover.RemoveEnemySpawnpoint();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemovePlayerOnlyAttribute();
            tileAttributeRemover.RemovePlayerSpawnpoint();

            MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(tile);
            tileBackgroundRemover.RemovePath();

            tileAttributePlacer.CreateTileObstacle(ObstacleType.Bush);
            return;
        }

        // Tile is already blocked
        tileAttributeRemover.RemoveTileObstacle();
    }

    public override void PlaceAttributeVariation(EditorMazeTile tile)
    {
        ITileAttribute tileObstacle = (TileObstacle)tile.GetAttributes().FirstOrDefault(attribute => attribute is TileObstacle);

        if (tileObstacle == null) return; // only place variation if there is already an obstacle

        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        tileAttributePlacer.PlaceTileObstacleVariation((TileObstacle)tileObstacle);
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[0];
    }
}