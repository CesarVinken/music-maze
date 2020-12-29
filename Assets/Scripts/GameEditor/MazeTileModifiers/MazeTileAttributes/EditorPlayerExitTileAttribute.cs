using System.Linq;
using UnityEngine;

public class EditorPlayerExitTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Player Exit"; }

    public override void PlaceAttribute(Tile tile)
    {
        TileAttributePlacer tileAttributePlacer = new TileAttributePlacer(tile);
        TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);

        IMazeTileAttribute playerExit = (PlayerExit)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerExit);
        if (playerExit == null)
        {
            tileAttributeRemover.RemoveEnemySpawnpoint();
            tileAttributeRemover.RemovePlayerOnlyAttribute();
            tileAttributeRemover.RemovePlayerSpawnpoint();
            tileAttributeRemover.RemoveTileObstacle();

            Logger.Warning($"Now place player exit at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlacePlayerExit(ObstacleType.Bush); 
            return;
        }

        tileAttributeRemover.RemovePlayerExit();
    }

    public override Sprite GetSprite()
    {
        return EditorUIContainer.Instance.TileAttributeIcons[1];
    }
}
