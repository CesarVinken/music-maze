using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorMazeTileBaseGround : EditorMazeTileBackgroundModifier, IGroundMaterialModifier
{
    public override string Name => "Grass Ground";

    public override void PlaceBackground(EditorMazeTile tile)
    {
        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);
        MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        Type oldMainMaterial = tile.TileMainMaterial?.GetType(); // old material before updating it

        ITileBackground mazeTileBaseGround = (MazeTileBaseGround)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseGround);
        if ((oldMainMaterial != typeof(GroundMainMaterial)))
        {
            List<ITileAttribute> attributes = tile.GetAttributes();
            for (int i = 0; i < attributes.Count; i++)
            {
                tileAttributeRemover.Remove(attributes[i]);
            }

            if (oldMainMaterial == typeof(WaterMainMaterial) || tile.TileMainMaterial == null)
            {
                tileBackgroundRemover.RemoveBackground<MazeTileBaseWater>();
            }

            if (mazeTileBaseGround == null)
            {
                tileBackgroundPlacer.PlaceBackground<MazeTileBaseGround>();
            }
            else
            {
                tileBackgroundPlacer.UpdateWaterConnectionsOnNeighbours(new MazeLevelDefaultWaterType());
            }
        }
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