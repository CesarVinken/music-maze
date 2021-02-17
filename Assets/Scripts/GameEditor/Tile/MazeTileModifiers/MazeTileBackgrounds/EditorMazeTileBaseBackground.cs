using System.Linq;
using UnityEngine;

public class EditorMazeTileBaseBackground : EditorMazeTileBackgroundModifier
{
    public override string Name => "Base Background";

    public override void PlaceBackground(EditorMazeTile tile)
    {
        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);
        MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(tile);

        ITileBackground mazeTileBaseBackground = (MazeTileBaseBackground)tile.TileBackgrounds.FirstOrDefault(background => background is MazeTileBaseBackground);
        if (mazeTileBaseBackground == null)
        {
            tileBackgroundPlacer.PlaceBaseBackground(new MazeLevelDefaultBaseBackgroundType());
        }

        tileBackgroundRemover.RemoveBaseBackground(new MazeLevelDefaultBaseBackgroundType());
    }

    public override void PlaceBackgroundVariation(EditorMazeTile tile)
    {
        Logger.Log("Background variations be implemented");
    }

    public override Sprite GetSprite()
    {
        return MazeSpriteManager.Instance.DefaultMazeTileBackground[0];
    }
}