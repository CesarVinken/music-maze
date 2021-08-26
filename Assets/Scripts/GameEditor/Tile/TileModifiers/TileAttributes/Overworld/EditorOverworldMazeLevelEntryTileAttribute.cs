using System.Linq;
using UnityEngine;

public class EditorOverworldMazeLevelEntryTileAttribute : EditorOverworldTileAttributeModifier, IGroundMaterialModifier
{
    public override string Name { get => "Maze Entry"; }

    public override void PlaceAttribute(EditorOverworldTile tile)
    {
        if (tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }

        EditorOverworldTileAttributePlacer tileAttributePlacer = new EditorOverworldTileAttributePlacer(tile);
        OverworldTileAttributeRemover tileAttributeRemover = new OverworldTileAttributeRemover(tile);

        ITileAttribute MazeLevelEntry = (MazeLevelEntry)tile.GetAttributes().FirstOrDefault(attribute => attribute is MazeLevelEntry);
        if (MazeLevelEntry == null)
        {
            tileAttributeRemover.Remove<PlayerSpawnpoint>();
            tileAttributeRemover.Remove<TileObstacle>();

            Logger.Log(Logger.Editor, $"Now place maze entry at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlaceMazeLevelEntry();
            return;
        }

        tileAttributeRemover.Remove<MazeLevelEntry>();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[5];
    }

    public override void InstantiateModifierActions()
    {
        Logger.Log("Load actions for modifier");
        GameObject.Instantiate(EditorCanvasUI.Instance.AssignMazeLevelEntryPrefab, EditorOverworldTileModificationPanel.Instance.TileModifierActionsContainer);
    }
}
