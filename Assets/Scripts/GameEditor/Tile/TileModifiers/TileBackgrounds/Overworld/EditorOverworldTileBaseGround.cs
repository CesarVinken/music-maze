using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorOverworldTileBaseGround : EditorOverworldTileBackgroundModifier, IGroundMaterialModifier
{
    public override string Name => "Grass Ground";

    public override void PlaceBackground(EditorOverworldTile tile)
    {
        EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(tile);
        OverworldTileBackgroundRemover tileBackgroundRemover = new OverworldTileBackgroundRemover(tile);
        OverworldTileAttributeRemover tileAttributeRemover = new OverworldTileAttributeRemover(tile);

        ITileBackground overworldTileBaseGround = (OverworldTileBaseGround)tile.GetBackgrounds().FirstOrDefault(background => background is OverworldTileBaseGround);
        if (overworldTileBaseGround == null)
        {
            List<ITileAttribute> attributes = tile.GetAttributes();
            for (int i = 0; i < attributes.Count; i++)
            {
                tileAttributeRemover.Remove(attributes[i]);
            }

            if (tile.TileMainMaterial?.GetType() == typeof(WaterMainMaterial) || tile.TileMainMaterial == null)
            {
                tileBackgroundRemover.RemoveBackground<MazeTileBaseWater>();
            }

            tileBackgroundPlacer.PlaceBackground<OverworldTileBaseGround>();
        }

        //tileBackgroundRemover.RemoveBaseBackground(new MazeLevelDefaultBaseBackgroundType());
    }

    public override void PlaceBackgroundVariation(EditorOverworldTile tile)
    {
        Logger.Log("Background variations be implemented");
    }

    public override Sprite GetSprite()
    {
        return OverworldSpriteManager.Instance.DefaultOverworldTileBackground[0];
    }
}