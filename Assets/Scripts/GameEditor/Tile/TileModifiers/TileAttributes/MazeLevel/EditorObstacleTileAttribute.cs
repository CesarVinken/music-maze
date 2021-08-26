using System.Linq;
using UnityEngine;

public class EditorObstacleTileAttribute : EditorMazeTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Obstacle"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        if(tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }

        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute tileObstacle = (TileObstacle)tile.GetAttributes().FirstOrDefault(attribute => (attribute is TileObstacle && !(attribute is PlayerExit)));
        if (tileObstacle == null)
        {
            tileAttributeRemover.Remove<EnemySpawnpoint>();
            tileAttributeRemover.Remove<PlayerExit>();
            tileAttributeRemover.Remove<PlayerOnly>();
            tileAttributeRemover.Remove<PlayerSpawnpoint>();
            tileAttributeRemover.Remove<MusicInstrumentCase>();

            MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(tile);
            tileBackgroundRemover.RemovePath();

            tileAttributePlacer.PlaceTileObstacle(ObstacleType.Bush);
            return;
        }

        // Tile is already blocked
        tileAttributeRemover.Remove<TileObstacle>();
    }

    public override void PlaceAttributeVariation(EditorMazeTile tile)
    {
        ITileAttribute tileObstacle = (TileObstacle)tile.GetAttributes().FirstOrDefault(attribute => attribute is TileObstacle);

        if (tileObstacle == null) return; // only place variation if there is already an obstacle

        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        tileAttributePlacer.PlaceTileObstacleVariation((TileObstacle)tileObstacle);
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[0];
    }
}
