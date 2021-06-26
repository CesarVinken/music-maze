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
            tileAttributeRemover.RemoveTileObstacle();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemoveEnemySpawnpoint();
            tileAttributeRemover.RemoveMusicInstrumentCase();
            tileAttributeRemover.RemovePlayerSpawnpoint();

            Logger.Warning($"Now place player only attribute at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlacePlayerOnlyAttribute(PlayerOnlyType.Bush);
            return;
        }

        tileAttributeRemover.RemovePlayerOnlyAttribute();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[4];
    }
}
