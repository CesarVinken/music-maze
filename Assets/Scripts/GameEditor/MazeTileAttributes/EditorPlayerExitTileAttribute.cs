using System.Linq;
using UnityEngine;

public class EditorPlayerExitTileAttribute : IEditorMazeTileAttribute
{
    public string Name { get => "Player Exit"; }

    public Sprite Sprite { get => null; }

    public EditorMazeTileAttributeType AttributeType { get => EditorMazeTileAttributeType.PlayerExit; }

    public void PlaceAttribute(Tile tile)
    {
        TileAttributePlacer tileAttributePlacer = new TileAttributePlacer(tile);

        IMazeTileAttribute playerExit = (PlayerExit)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerExit);
        if (playerExit == null)
        {
            tileAttributePlacer.RemoveTileObstacle();
            tileAttributePlacer.RemoveEnemySpawnpoint();
            tileAttributePlacer.RemovePlayerSpawnpoint();

            Logger.Warning($"Now place player exit at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlacePlayerExit(ObstacleType.Wall);
            return;
        }

        tileAttributePlacer.RemovePlayerExit();
    }
}
