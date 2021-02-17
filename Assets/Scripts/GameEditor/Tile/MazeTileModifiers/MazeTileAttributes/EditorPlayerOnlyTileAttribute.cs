using System.Linq;
using UnityEngine;

public class EditorPlayerOnlyTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Player Only"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute playerOnlyAttribute = (PlayerOnly)tile.TileAttributes.FirstOrDefault(attribute => attribute is PlayerOnly);
        if (playerOnlyAttribute == null)
        {
            tileAttributeRemover.RemoveTileObstacle();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemoveEnemySpawnpoint();
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
