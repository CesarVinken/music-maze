﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorMazeTileBaseWater : EditorMazeTileBackgroundModifier, IWaterMaterialModifier
{
    public override string Name => "Water";

    public override void PlaceBackground(EditorMazeTile tile)
    {
        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);
        MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        List<ITileBackground> backgrounds = tile.GetBackgrounds();
        ITileBackground mazeTileBaseWater = backgrounds.FirstOrDefault(background => background is MazeTileBaseWater);

        // Only act if there is no water
        if (mazeTileBaseWater == null)
        {
            Type oldMainMaterial = tile.TileMainMaterial?.GetType(); // old material before updating it

            if (oldMainMaterial == null || oldMainMaterial == typeof(GroundMainMaterial))
            {
                tileBackgroundRemover.RemoveBackground<MazeTilePath>();
            }
             
            MazeTileBaseWater water = tileBackgroundPlacer.PlaceBackground<MazeTileBaseWater>();

            List<ITileAttribute> attributes = tile.GetAttributes();
            for (int i = 0; i < attributes.Count; i++)
            {
                tileAttributeRemover.Remove(attributes[i]);
            }
            if (oldMainMaterial == null || oldMainMaterial == typeof(GroundMainMaterial))
            {
                if(water.ConnectionScore == 16) // remove background if we completely covered the tile with water
                {
                    tileBackgroundRemover.RemoveBackground<MazeTileBaseGround>();
                }
            }

        }
    }

    public override void PlaceBackgroundVariation(EditorMazeTile tile)
    {
        Logger.Log("Background variations to be implemented");
        ITileBackground mazeTileWater = (MazeTileBaseWater)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseWater);

        if (mazeTileWater == null) return; // only place variation if there is already a path

        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);
        tileBackgroundPlacer.PlaceWaterVariation((MazeTileBaseWater)mazeTileWater);
    }

    public override Sprite GetSprite()
    {
        return MazeSpriteManager.Instance.DefaultMazeTileWater[0];
    }
}