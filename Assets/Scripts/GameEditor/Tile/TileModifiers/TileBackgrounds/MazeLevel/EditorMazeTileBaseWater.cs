using System.Linq;
using UnityEngine;

public class EditorMazeTileBaseWater : EditorMazeTileBackgroundModifier, IWaterMaterialModifier
{
    public override string Name => "Water";

    public override void PlaceBackground(EditorMazeTile tile)
    {
        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);
        MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(tile);

        ITileBackground mazeTileBaseWater = (MazeTileBaseWater)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseWater);
        if (mazeTileBaseWater == null)
        {
            tileBackgroundPlacer.PlaceBackground(new MazeLevelDefaultWaterType());
            Logger.Log("TODO: Remove Land");
        }

        //tileBackgroundRemover.RemoveBaseBackground(new MazeLevelDefaultBaseBackgroundType());
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