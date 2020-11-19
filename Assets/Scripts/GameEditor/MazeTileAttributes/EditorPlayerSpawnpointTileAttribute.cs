using System.Linq;
using UnityEngine;

public class EditorPlayerSpawnpointTileAttribute : IEditorMazeTileAttribute
{
    public string Name { get => "Player Spawnpoint"; }

    public Sprite Sprite { get => null; }

    public EditorMazeTileAttributeType AttributeType { get => EditorMazeTileAttributeType.PlayerSpawnpoint; }

    public void PlaceAttribute(Tile tile)
    {
        TileAttributePlacer tileAttributePlacer = new TileAttributePlacer(tile);
        TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);

        IMazeTileAttribute playerSpawnpoint = (PlayerSpawnpoint)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null)
        {
            tileAttributeRemover.RemoveTileObstacle();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemoveEnemySpawnpoint();

            tileAttributePlacer.PlacePlayerSpawnpoint();
            return;
        }

        tileAttributeRemover.RemovePlayerSpawnpoint();
    }
}
