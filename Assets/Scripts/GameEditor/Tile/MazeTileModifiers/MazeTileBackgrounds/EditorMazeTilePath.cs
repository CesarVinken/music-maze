using System.Linq;
using UnityEngine;

public class EditorMazeTilePath : EditorMazeTileBackgroundModifier
{
    public override string Name => "Path";

    public override void PlaceBackground(EditorTile tile)
    {
        EditorTileBackgroundPlacer tileBackgroundPlacer = new EditorTileBackgroundPlacer(tile);
        TileBackgroundRemover tileBackgroundRemover = new TileBackgroundRemover(tile);

        IMazeTileBackground mazeTilePath = (MazeTilePath)tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTilePath);
        if (mazeTilePath == null)
        {
            TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);
            tileAttributeRemover.RemoveTileObstacle();

            tileBackgroundPlacer.PlacePath(new MazeLevelDefaultPathType());
            return;
        }

        // This path already exists on this tile, so remove it
        tileBackgroundRemover.RemovePath();
    }

    public override void PlaceBackgroundVariation(EditorTile tile)
    {
        IMazeTileBackground mazeTilePath = (MazeTilePath)tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTilePath);
        
        if (mazeTilePath == null) return; // only place variation if there is already a path

        EditorTileBackgroundPlacer tileBackgroundPlacer = new EditorTileBackgroundPlacer(tile);
        tileBackgroundPlacer.PlacePathVariation((MazeTilePath)mazeTilePath);
    }

    public override Sprite GetSprite()
    {
        return SpriteManager.Instance.DefaultPath[15];
    }
}
