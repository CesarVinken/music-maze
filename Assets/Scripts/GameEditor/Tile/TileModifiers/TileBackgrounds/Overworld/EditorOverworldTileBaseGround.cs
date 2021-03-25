using System;
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

        Type oldMainMaterial = tile.TileMainMaterial?.GetType(); // old material before updating it
        ITileBackground overworldTileBaseGround = (OverworldTileBaseGround)tile.GetBackgrounds().FirstOrDefault(background => background is OverworldTileBaseGround);

        if (oldMainMaterial != typeof(GroundMainMaterial))
        {
            List<ITileAttribute> attributes = tile.GetAttributes();
            for (int i = 0; i < attributes.Count; i++)
            {
                tileAttributeRemover.Remove(attributes[i]);
            }

            if (oldMainMaterial == typeof(WaterMainMaterial) || tile.TileMainMaterial == null)
            {
                tileBackgroundRemover.RemoveBackground<OverworldTileBaseWater>();
            }

            if(overworldTileBaseGround == null)
            {
                tileBackgroundPlacer.PlaceBackground<OverworldTileBaseGround>();
            }
            else
            {
                tileBackgroundPlacer.UpdateGroundConnectionsOnNeighbours(new MazeLevelDefaultGroundType());
            }
        }
    }

    public override void PlaceBackgroundVariation(EditorOverworldTile tile)
    {
        Logger.Log("Try place background variation");
        ITileBackground overworldTileGround = (OverworldTileBaseGround)tile.GetBackgrounds().FirstOrDefault(background => background is OverworldTileBaseGround);

        if (overworldTileGround == null) return;

        EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(tile);
        tileBackgroundPlacer.PlaceGroundVariation((OverworldTileBaseGround)overworldTileGround);
    }

    public override Sprite GetSprite()
    {
        return OverworldSpriteManager.Instance.DefaultOverworldTileBackground[15];
    }
}