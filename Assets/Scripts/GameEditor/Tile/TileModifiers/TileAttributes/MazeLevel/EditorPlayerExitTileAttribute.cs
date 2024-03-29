﻿using System.Linq;
using UnityEngine;

public class EditorPlayerExitTileAttribute : EditorMazeTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Player Exit"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        if (tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }
        
        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute playerExit = (PlayerExit)tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerExit);
        if (playerExit == null)
        {
            tileAttributeRemover.Remove<EnemySpawnpoint>();
            tileAttributeRemover.Remove<PlayerOnly>();
            tileAttributeRemover.Remove<PlayerSpawnpoint>();
            tileAttributeRemover.Remove<MusicInstrumentCase>();
            tileAttributeRemover.Remove<TileObstacle>();
            tileAttributeRemover.Remove<Sheetmusic>();

            Logger.Warning(Logger.Editor, $"Now place player exit at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlacePlayerExit(ObstacleType.Bush); 
            return;
        }

        tileAttributeRemover.Remove<PlayerExit>();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[1];
    }
}
