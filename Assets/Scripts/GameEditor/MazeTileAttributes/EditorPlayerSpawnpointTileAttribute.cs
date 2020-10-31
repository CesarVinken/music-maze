using System.Linq;
using UnityEngine;

public class EditorPlayerSpawnpointTileAttribute : IEditorMazeTileAttribute
{
    public string Name { get => "Player Spawnpoint"; }

    public Sprite Sprite { get => null; }

    public EditorMazeTileAttributeType AttributeType { get => EditorMazeTileAttributeType.PlayerSpawnpoint; }

    public void PlaceAttribute(Tile tile)
    {
        IMazeTileAttribute playerSpawnpoint = (PlayerSpawnpoint)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null)
        {
            tile.RemoveTileObstacle();
            tile.RemovePlayerExit();
            tile.RemoveEnemySpawnpoint();

            tile.PlacePlayerSpawnpoint();
            return;
        }

        tile.RemovePlayerSpawnpoint();
    }
}
