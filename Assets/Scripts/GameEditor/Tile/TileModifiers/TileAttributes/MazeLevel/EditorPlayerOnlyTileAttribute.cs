using System.Linq;
using UnityEngine;

public class EditorPlayerOnlyTileAttribute : EditorMazeTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Player Only"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        if (tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }
        
        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute playerOnlyAttribute = (PlayerOnly)tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerOnly);
        if (playerOnlyAttribute == null)
        {
            tileAttributeRemover.Remove<TileObstacle>();
            tileAttributeRemover.Remove<PlayerExit>();
            tileAttributeRemover.Remove<EnemySpawnpoint>();
            tileAttributeRemover.Remove<MusicInstrumentCase>();
            tileAttributeRemover.Remove<PlayerSpawnpoint>();
            tileAttributeRemover.Remove<Sheetmusic>();

            Logger.Warning($"Now place player only attribute at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlacePlayerOnlyAttribute(PlayerOnlyType.Bush);
            return;
        }

        tileAttributeRemover.Remove<PlayerOnly>();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[4];
    }
}
