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
            List<ITileAttribute> attributes = tile.GetAttributes();
            for (int i = 0; i < attributes.Count; i++)
            {
                tileAttributeRemover.Remove(attributes[i]);
            }
            if (tile.TileMainMaterial?.GetType() == typeof(GroundMainMaterial) || tile.TileMainMaterial == null)
            {
                tileBackgroundRemover.RemoveBackground<MazeTilePath>();
                tileBackgroundRemover.RemoveBackground<MazeTileBaseGround>();
            }

            tileBackgroundPlacer.PlaceBackground<MazeTileBaseWater>();
        }
    }

    public override void PlaceBackgroundVariation(EditorMazeTile tile)
    {
        Logger.Log("Background variations be implemented");
    }

    public override Sprite GetSprite()
    {
        return MazeSpriteManager.Instance.DefaultMazeTileWater[0];
    }
}