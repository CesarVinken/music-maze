﻿using System.Linq;
using UnityEngine;

public class EditorMazeTilePath : EditorMazeTileBackgroundModifier, IGroundMaterialModifier
{
    public override string Name => "Path";

    public override void PlaceBackground(EditorMazeTile tile)
    {
        if (tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }

        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);
        MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(tile);

        ITileBackground mazeTilePath = (MazeTilePath)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTilePath);
        if (mazeTilePath == null)
        {
            MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);
            tileAttributeRemover.Remove<TileObstacle>();

            tileBackgroundPlacer.PlacePath(new MazeLevelDefaultPathType());
            return;
        }

        // This path already exists on this tile, so remove it
        tileBackgroundRemover.RemovePath();
    }

    public override void PlaceBackgroundVariation(EditorMazeTile tile)
    {
        ITileBackground mazeTilePath = (MazeTilePath)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTilePath);
        
        if (mazeTilePath == null) return; // only place variation if there is already a path

        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);
        tileBackgroundPlacer.PlacePathVariation((MazeTilePath)mazeTilePath);
    }

    public override Sprite GetSprite()
    {
        return MazeSpriteManager.Instance.DefaultPath[15];
    }
}
