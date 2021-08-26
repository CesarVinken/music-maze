using System.Linq;
using UnityEngine;

public class EditorPlayerSpawnpointOverworldTileAttribute : EditorOverworldTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Player Spawnpoint"; }

    public override void PlaceAttribute(EditorOverworldTile tile)
    {
        if (tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }

        EditorOverworldTileAttributePlacer tileAttributePlacer = new EditorOverworldTileAttributePlacer(tile);
        OverworldTileAttributeRemover tileAttributeRemover = new OverworldTileAttributeRemover(tile);

        ITileAttribute playerSpawnpoint = (PlayerSpawnpoint)tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null)
        {
            //tileAttributeRemover.RemoveTileObstacle();

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