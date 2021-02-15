using System.Linq;
using UnityEngine;

public class EditorMazeTileBaseBackground : EditorMazeTileBackgroundModifier
{
    public override string Name => "Base Background";

    public override void PlaceBackground(EditorTile tile)
    {
        EditorTileBackgroundPlacer tileBackgroundPlacer = new EditorTileBackgroundPlacer(tile);
        TileBackgroundRemover tileBackgroundRemover = new TileBackgroundRemover(tile);

        IMazeTileBackground mazeTileBaseBackground = (MazeTileBaseBackground)tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTileBaseBackground);
        if (mazeTileBaseBackground == null)
        {
            tileBackgroundPlacer.PlaceBaseBackground(new MazeLevelDefaultBaseBackgroundType());
        }

        tileBackgroundRemover.RemoveBaseBackground(new MazeLevelDefaultBaseBackgroundType());
    }

    public override void PlaceBackgroundVariation(EditorTile tile)
    {
        Logger.Log("Background variations be implemented");
    }

    public override Sprite GetSprite()
    {
        return SpriteManager.Instance.DefaultMazeTileBackground[0];
    }
}