using System.Linq;
using UnityEngine;

public class EditorPlayerSpawnpointMazeTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Player Spawnpoint"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute playerSpawnpoint = (PlayerSpawnpoint)tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null)
        {
            tileAttributeRemover.RemoveEnemySpawnpoint();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemovePlayerOnlyAttribute();
            tileAttributeRemover.RemoveTileObstacle();

            tileAttributePlacer.PlacePlayerSpawnpoint();
            return;
        }

        tileAttributeRemover.RemovePlayerSpawnpoint();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[2];
    }
}
