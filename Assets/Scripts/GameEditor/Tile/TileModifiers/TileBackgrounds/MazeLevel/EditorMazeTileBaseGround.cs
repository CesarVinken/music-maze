using System.Linq;
using UnityEngine;

public class EditorMazeTileBaseGround : EditorMazeTileBackgroundModifier, IGroundMaterialModifier
{
    public override string Name => "Grass Ground";

    public override void PlaceBackground(EditorMazeTile tile)
    {
        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);
        MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(tile);

        ITileBackground mazeTileBaseBackground = (MazeTileBaseGround)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseGround);
        if (mazeTileBaseBackground == null)
        {
            tileBackgroundPlacer.PlaceBackground(new MazeLevelDefaultGroundType());
            Logger.Log("TODO: Remove Water");
        }

        //tileBackgroundRemover.RemoveBaseBackground(new MazeLevelDefaultBaseBackgroundType());
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