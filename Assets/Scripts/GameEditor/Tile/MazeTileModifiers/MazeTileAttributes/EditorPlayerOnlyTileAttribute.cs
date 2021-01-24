﻿using System.Linq;
using UnityEngine;

public class EditorPlayerOnlyTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Player Only"; }

    public override void PlaceAttribute(EditorTile tile)
    {
        EditorTileAttributePlacer tileAttributePlacer = new EditorTileAttributePlacer(tile);
        TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);

        IMazeTileAttribute playerOnlyAttribute = (PlayerOnly)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerOnly);
        if (playerOnlyAttribute == null)
        {
            tileAttributeRemover.RemoveTileObstacle();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemoveEnemySpawnpoint();
            tileAttributeRemover.RemovePlayerSpawnpoint();

            Logger.Warning($"Now place player only attribute at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlacePlayerOnlyAttribute(PlayerOnlyType.Bush);
            return;
        }

        tileAttributeRemover.RemovePlayerOnlyAttribute();
    }

    public override Sprite GetSprite()
    {
        return EditorUIContainer.Instance.TileAttributeIcons[4];
    }
}