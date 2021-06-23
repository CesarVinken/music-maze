using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorEnemySpawnpointTileAttribute : EditorMazeTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Enemy Spawnpoint"; }
    public List<TileArea> TileAreas = new List<TileArea>();

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        Logger.Log("Try place Enemy Spawnpoint");
        if (tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }
        
        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute enemySpawnpoint = (EnemySpawnpoint)tile.GetAttributes().FirstOrDefault(attribute => attribute is EnemySpawnpoint);
        if (enemySpawnpoint == null)
        {
            tileAttributeRemover.RemovePlayerExit();
            tileAttributeRemover.RemovePlayerOnlyAttribute();
            tileAttributeRemover.RemovePlayerSpawnpoint();
            tileAttributeRemover.RemoveTileObstacle();

            tileAttributePlacer.PlaceEnemySpawnpoint();

            TileAreaToEnemySpawnpointAssigner.Instance?.CheckForEnemySpawnpointOnTile();
            return;
        }

        tileAttributeRemover.RemoveEnemySpawnpoint();
        TileAreaToEnemySpawnpointAssigner.Instance?.CheckForEnemySpawnpointOnTile();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[3];
    }

    public override void InstantiateModifierActions()
    {
        Logger.Log("Load actions for enemy spawnpoint");
        GameObject.Instantiate(EditorCanvasUI.Instance.AssignTileAreasToEnemySpawnpointPrefab, EditorMazeTileModificationPanel.Instance.TileModifierActionsContainer);
    }
}
