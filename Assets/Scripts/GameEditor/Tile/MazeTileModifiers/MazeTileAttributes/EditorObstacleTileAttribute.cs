using System.Linq;
using UnityEngine;

public class EditorObstacleTileAttribute : EditorMazeTileAttributeModifier
{
    public override string Name { get => "Obstacle"; }

    public override void PlaceAttribute(EditorTile tile)
    {
        EditorTileAttributePlacer tileAttributePlacer = new EditorTileAttributePlacer(tile);
        TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);

        IMazeTileAttribute tileObstacle = (TileObstacle)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);
        if (tileObstacle == null)
        {
            tileAttributeRemover.RemoveEnemySpawnpoint();
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemovePlayerOnlyAttribute();
            tileAttributeRemover.RemovePlayerSpawnpoint();

            TileBackgroundRemover tileBackgroundRemover = new TileBackgroundRemover(tile);
            tileBackgroundRemover.RemovePath();

            tileAttributePlacer.CreateTileObstacle(ObstacleType.Bush);
            return;
        }

        // Tile is already blocked
        tileAttributeRemover.RemoveTileObstacle();
    }

    public override void PlaceAttributeVariation(EditorTile tile)
    {
        IMazeTileAttribute tileObstacle = (TileObstacle)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);

        if (tileObstacle == null) return; // only place variation if there is already an obstacle

        EditorTileAttributePlacer tileAttributePlacer = new EditorTileAttributePlacer(tile);
        tileAttributePlacer.PlaceTileObstacleVariation((TileObstacle)tileObstacle);
    }

    public override Sprite GetSprite()
    {
        return EditorUIContainer.Instance.TileAttributeIcons[0];
    }
}
