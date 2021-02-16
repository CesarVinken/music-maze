﻿using System.Linq;
using UnityEngine;

public class EditorObstacleTileAttribute : EditorMazeTileAttributeModifier<EditorMazeTile>
{
    public override string Name { get => "Obstacle"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        EditorTileAttributePlacer tileAttributePlacer = new EditorTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute tileObstacle = (TileObstacle)tile.TileAttributes.FirstOrDefault(attribute => (attribute is TileObstacle && !(attribute is PlayerExit)));
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
        ITileAttribute tileObstacle = (TileObstacle)tile.TileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);

        if (tileObstacle == null) return; // only place variation if there is already an obstacle

        EditorTileAttributePlacer tileAttributePlacer = new EditorTileAttributePlacer(tile);
        tileAttributePlacer.PlaceTileObstacleVariation((TileObstacle)tileObstacle);
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[0];
    }
}
