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

        ITileBackground overworldTileBaseWater = (OverworldTileBaseWater)tile.GetBackgrounds().FirstOrDefault(background => background is OverworldTileBaseWater);
        if (overworldTileBaseWater == null)
        {
            List<ITileAttribute> attributes = tile.GetAttributes();
            for (int i = 0; i < attributes.Count; i++)
            {
                tileAttributeRemover.Remove(attributes[i]);
            }

            if (tile.TileMainMaterial?.GetType() == typeof(GroundMainMaterial) || tile.TileMainMaterial == null)
            {
                tileBackgroundRemover.RemoveBackground<OverworldTilePath>();
                tileBackgroundRemover.RemoveBackground<OverworldTileBaseGround>();
            }

            tileBackgroundPlacer.PlaceBackground<OverworldTileBaseWater>();
        }
    }

    public override void PlaceBackgroundVariation(EditorOverworldTile tile)
    {
        Logger.Log("Background variations be implemented");
    }

    public override Sprite GetSprite()
    {
        return OverworldSpriteManager.Instance.DefaultOverworldTileWater[0];
    }
}