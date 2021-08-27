using System.Linq;
using UnityEngine;

public class EditorMusicInstrumentCaseTileAttribute : EditorMazeTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Music Instrument Case"; }

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

        ITileAttribute musicInstrumentCase = (MusicInstrumentCase)tile.GetAttributes().FirstOrDefault(attribute => attribute is MusicInstrumentCase);
        if (musicInstrumentCase == null)
        {
            tileAttributeRemover.Remove<EnemySpawnpoint>();
            tileAttributeRemover.Remove<PlayerExit>();
            tileAttributeRemover.Remove<PlayerOnly>();
            tileAttributeRemover.Remove<PlayerSpawnpoint>();
            tileAttributeRemover.Remove<TileObstacle>();
            tileAttributeRemover.Remove<Sheetmusic>();

            Logger.Warning(Logger.Editor, $"Now place music instrument case at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlaceMusicInstrumentCase();
            return;
        }

        tileAttributeRemover.Remove<MusicInstrumentCase>();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[7];
    }
}
