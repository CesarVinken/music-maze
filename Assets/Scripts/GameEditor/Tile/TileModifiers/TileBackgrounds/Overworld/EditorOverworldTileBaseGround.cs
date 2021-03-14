using System.Linq;
using UnityEngine;

public class EditorOverworldTileBaseGround : EditorOverworldTileBackgroundModifier, IGroundMaterialModifier
{
    public override string Name => "Grass Ground";

    public override void PlaceBackground(EditorOverworldTile tile)
    {
        EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(tile);
        OverworldTileBackgroundRemover tileBackgroundRemover = new OverworldTileBackgroundRemover(tile);

        ITileBackground overworldTileBaseBackground = (OverworldTileBaseGround)tile.GetBackgrounds().FirstOrDefault(background => background is OverworldTileBaseGround);
        if (overworldTileBaseBackground == null)
        {
            tileBackgroundPlacer.PlaceBackground(new OverworldDefaultBaseGroundType());
            Logger.Log("TODO: Remove Water");
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