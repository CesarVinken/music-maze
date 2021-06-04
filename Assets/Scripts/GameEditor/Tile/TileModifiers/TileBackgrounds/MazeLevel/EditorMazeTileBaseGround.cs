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

        MazeTileBaseGround oldMazeTileBaseGround = (MazeTileBaseGround)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseGround);
        if ((oldMainMaterial != typeof(GroundMainMaterial)))
        {
            List<ITileAttribute> attributes = tile.GetAttributes();
            for (int i = 0; i < attributes.Count; i++)
            {
                tileAttributeRemover.Remove(attributes[i]);
            }

            //if (oldMainMaterial == typeof(WaterMainMaterial) || tile.TileMainMaterial == null)
            //{
            //    tileBackgroundRemover.RemoveBackground<MazeTileBaseWater>();
            //}

            // Remove the old land background, because we are going to fully cover it with a new land background
            if(oldMazeTileBaseGround != null && oldMazeTileBaseGround.ConnectionScore != 16)
            {
                tileBackgroundRemover.RemoveBackground<MazeTileBaseGround>();
            }

            MazeTileBaseGround newMazeTileBaseGround = tileBackgroundPlacer.PlaceBackground<MazeTileBaseGround>();
            // Remove water from the tile that is fully covered by land
            if (newMazeTileBaseGround.ConnectionScore == 16)
            {
                tileBackgroundRemover.RemoveBackground<MazeTileBaseWater>();
            }
        }
    }

    public override void PlaceBackgroundVariation(EditorMazeTile tile)
    {
        Logger.Log("Try place background variation");
        ITileBackground mazeTileGround = (MazeTileBaseGround)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseGround);

        if (mazeTileGround == null) return;

        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);
        tileBackgroundPlacer.PlaceGroundVariation((MazeTileBaseGround)mazeTileGround);
    }

    public override Sprite GetSprite()
    {
        return MazeSpriteManager.Instance.DefaultMazeTileGround[15];
    }
}