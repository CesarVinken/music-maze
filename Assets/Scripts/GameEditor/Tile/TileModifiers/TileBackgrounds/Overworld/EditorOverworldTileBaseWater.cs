using System.Linq;
using UnityEngine;

public class EditorOverworldTileBaseWater : EditorOverworldTileBackgroundModifier, IWaterMaterialModifier
{
    public override string Name => "Water";

    public override void PlaceBackground(EditorOverworldTile tile)
    {
        EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(tile);
        OverworldTileBackgroundRemover tileBackgroundRemover = new OverworldTileBackgroundRemover(tile);

        ITileBackground overworldTileBaseWater = (OverworldTileBaseWater)tile.GetBackgrounds().FirstOrDefault(background => background is OverworldTileBaseWater);
        if (overworldTileBaseWater == null)
        {
            tileBackgroundPlacer.PlaceBackground(new OverworldDefaultWaterType());
            Logger.Log("TODO: Remove Land");
        }

        //tileBackgroundRemover.RemoveBaseBackground(new MazeLevelDefaultBaseBackgroundType());
    }

    public override void PlaceBackgroundVariation(EditorOverworldTile tile)
    {
        Logger.Log("Background variations be implemented");
    }

    public override Sprite GetSprite()
    {
        return OverworldSpriteManager.Instance.DefaultOverworldTileWater[0];
    }
}