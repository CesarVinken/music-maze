using System.Linq;
using UnityEngine;

public class EditorMazeTileBaseBackground : EditorMazeTileBackgroundModifier
{
    public override string Name => "Grass";

    public override void PlaceBackground(Tile tile)
    {
        TileBackgroundPlacer tileBackgroundPlacer = new TileBackgroundPlacer(tile);
        TileBackgroundRemover tileBackgroundRemover = new TileBackgroundRemover(tile);

        IMazeTileBackground mazeTileBaseBackground = (MazeTileBaseBackground)tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTileBaseBackground);
        if (mazeTileBaseBackground == null)
        {
            tileBackgroundPlacer.PlaceBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
        }

        tileBackgroundRemover.RemoveBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
    }

    public override void PlaceBackgroundVariation(Tile tile)
    {
        Logger.Log("Background variations be implemented");
    }

    public override Sprite GetSprite()
    {
        return SpriteManager.Instance.DefaultMazeTileBackground[0];
    }
}