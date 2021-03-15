﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorMazeTileBaseGround : EditorMazeTileBackgroundModifier, IGroundMaterialModifier
{
    public override string Name => "Grass Ground";

    public override void PlaceBackground(EditorMazeTile tile)
    {
        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);
        MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(tile);

        ITileBackground mazeTileBaseGround = (MazeTileBaseGround)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseGround);
        if (mazeTileBaseGround == null)
        {
            List<ITileAttribute> attributes = tile.GetAttributes();
            for (int i = 0; i < attributes.Count; i++)
            {
                tile.RemoveAttribute(attributes[i]);
            }

            if(tile.TileMainMaterial?.GetType() == typeof(WaterMainMaterial) || tile.TileMainMaterial == null)
            {
                tileBackgroundRemover.RemoveBackground<MazeTileBaseWater>();
            }

            tileBackgroundPlacer.PlaceBackground<MazeTileBaseGround>();
            Logger.Log("TODO: Remove Water");
        }

        //tileBackgroundRemover.RemoveBaseBackground(new MazeLevelDefaultBaseBackgroundType());
    }

    public override void PlaceBackgroundVariation(EditorMazeTile tile)
    {
        Logger.Log("Background variations be implemented");
    }

    public override Sprite GetSprite()
    {
        return MazeSpriteManager.Instance.DefaultMazeTileGround[0];
    }
}