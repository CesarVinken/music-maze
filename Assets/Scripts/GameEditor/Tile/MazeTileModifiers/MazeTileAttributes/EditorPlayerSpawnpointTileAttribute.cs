﻿using System.Linq;
using UnityEngine;

public class EditorPlayerSpawnpointTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Player Spawnpoint"; }

    public override void PlaceAttribute(EditorTile tile)
    {
        EditorTileAttributePlacer tileAttributePlacer = new EditorTileAttributePlacer(tile);
        TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);

        IMazeTileAttribute playerSpawnpoint = (PlayerSpawnpoint)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null)
        {
            tileAttributeRemover.RemoveEnemySpawnpoint();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemovePlayerOnlyAttribute();
            tileAttributeRemover.RemoveTileObstacle();

            tileAttributePlacer.PlacePlayerSpawnpoint();
            return;
        }

        tileAttributeRemover.RemovePlayerSpawnpoint();
    }

    public override Sprite GetSprite()
    {
        return EditorUIContainer.Instance.TileAttributeIcons[2];
    }
}