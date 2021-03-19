using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorOverworldTileBaseWater : EditorOverworldTileBackgroundModifier, IWaterMaterialModifier
{
    public override string Name => "Water";

    public override void PlaceBackground(EditorOverworldTile tile)
    {
        EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(tile);
        OverworldTileBackgroundRemover tileBackgroundRemover = new OverworldTileBackgroundRemover(tile);
        OverworldTileAttributeRemover tileAttributeRemover = new OverworldTileAttributeRemover(tile);

        List<ITileBackground> backgrounds = tile.GetBackgrounds();
        ITileBackground overworldTileBaseWater = (OverworldTileBaseWater)tile.GetBackgrounds().FirstOrDefault(background => background is OverworldTileBaseWater);
        
        if (overworldTileBaseWater == null)
        {
            Type oldMainMaterial = tile.TileMainMaterial?.GetType(); // old material before updating it

            if (oldMainMaterial == null || oldMainMaterial == typeof(GroundMainMaterial))
            {
                tileBackgroundRemover.RemoveBackground<OverworldTilePath>();
            }
            OverworldTileBaseWater water = tileBackgroundPlacer.PlaceBackground<OverworldTileBaseWater>();

            List<ITileAttribute> attributes = tile.GetAttributes();
            for (int i = 0; i < attributes.Count; i++)
            {
                tileAttributeRemover.Remove(attributes[i]);
            }

            if (oldMainMaterial == null || oldMainMaterial == typeof(GroundMainMaterial))
            {
                if (water.ConnectionScore == 16) // remove background if we completely covered the tile with water
                {
                    tileBackgroundRemover.RemoveBackground<OverworldTileBaseGround>();
                }
            }
        }
    }

    public override void PlaceBackgroundVariation(EditorOverworldTile tile)
    {
        Logger.Log("Try place background variation");
        ITileBackground overworldTileWater = (OverworldTileBaseWater)tile.GetBackgrounds().FirstOrDefault(background => background is OverworldTileBaseWater);

        if (overworldTileWater == null) return;

        EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(tile);
        tileBackgroundPlacer.PlaceWaterVariation((OverworldTileBaseWater)overworldTileWater);
    }

    public override Sprite GetSprite()
    {
        return OverworldSpriteManager.Instance.DefaultOverworldTileWater[0];
    }
}