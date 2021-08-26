using System.Linq;
using UnityEngine;

public class EditorPlayerSpawnpointMazeTileAttribute : EditorMazeTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Player Spawnpoint"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        if (tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }
        
        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute playerSpawnpoint = (PlayerSpawnpoint)tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null)
        {
            tileAttributeRemover.Remove<EnemySpawnpoint>();
            tileAttributeRemover.Remove<PlayerExit>();
            tileAttributeRemover.Remove<PlayerOnly>();
            tileAttributeRemover.Remove<TileObstacle>();

            tileAttributePlacer.PlacePlayerSpawnpoint();
            return;
        }

        tileAttributeRemover.Remove<PlayerSpawnpoint>();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[2];
    }
}
