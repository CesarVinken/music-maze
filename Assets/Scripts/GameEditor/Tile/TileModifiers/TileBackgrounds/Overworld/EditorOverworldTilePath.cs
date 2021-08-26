using System.Linq;
using UnityEngine;

public class EditorOverworldTilePath : EditorOverworldTileBackgroundModifier, IGroundMaterialModifier
{
    public override string Name => "Path";

    public override Sprite GetSprite()
    {
        return OverworldSpriteManager.Instance.Path[15];
    }

    public override void PlaceBackground(EditorOverworldTile tile)
    {
        if (tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }

        EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(tile);
        OverworldTileBackgroundRemover tileBackgroundRemover = new OverworldTileBackgroundRemover(tile);

        ITileBackground overworldTilePath = (OverworldTilePath)tile.GetBackgrounds().FirstOrDefault(background => background is OverworldTilePath);
        if (overworldTilePath == null)
        {
            OverworldTileAttributeRemover tileAttributeRemover = new OverworldTileAttributeRemover(tile);
            tileAttributeRemover.Remove<TileObstacle>();

            tileBackgroundPlacer.PlacePath(new OverworldDefaultPathType());
            return;
        }

        // This path already exists on this tile, so remove it
        tileBackgroundRemover.RemovePath();
    }

    public override void PlaceBackgroundVariation(EditorOverworldTile tile)
    {
        ITileBackground overworldTilePath = (OverworldTilePath)tile.GetBackgrounds().FirstOrDefault(background => background is OverworldTilePath);

        if (overworldTilePath == null) return; // only place variation if there is already a path

        EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(tile);
        tileBackgroundPlacer.PlacePathVariation((OverworldTilePath)overworldTilePath);
    }
}
