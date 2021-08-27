using System.Linq;
using UnityEngine;

public class EditorSheetmusicTileAttribute : EditorMazeTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Sheet music"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        if (tile.TileMainMaterial.GetType() == typeof(WaterMainMaterial))
        {
            if(tile.TryGetAttribute<BridgePiece>() == null)
            {
                return;
            }
        }

        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute sheetMusic = (Sheetmusic)tile.GetAttributes().FirstOrDefault(attribute => attribute is Sheetmusic);
        if (sheetMusic == null)
        {
            tileAttributeRemover.Remove<EnemySpawnpoint>();
            tileAttributeRemover.Remove<PlayerExit>();
            tileAttributeRemover.Remove<PlayerOnly>();
            tileAttributeRemover.Remove<PlayerSpawnpoint>();
            tileAttributeRemover.Remove<TileObstacle>();
            tileAttributeRemover.Remove<MusicInstrumentCase>();

            Logger.Warning(Logger.Editor, $"Now place sheetmusic at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlaceSheetmusic();
            return;
        }

        tileAttributeRemover.Remove<Sheetmusic>();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[8];
    }
}
