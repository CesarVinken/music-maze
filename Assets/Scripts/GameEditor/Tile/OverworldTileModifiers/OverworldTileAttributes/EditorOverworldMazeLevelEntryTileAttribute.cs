using System.Linq;
using UnityEngine;

public class EditorOverworldMazeLevelEntryTileAttribute : EditorOverworldTileAttributeModifier
{
    public override string Name { get => "Maze Entry"; }

    public override void PlaceAttribute(EditorOverworldTile tile)
    {
        EditorOverworldTileAttributePlacer tileAttributePlacer = new EditorOverworldTileAttributePlacer(tile);
        OverworldTileAttributeRemover tileAttributeRemover = new OverworldTileAttributeRemover(tile);

        ITileAttribute MazeLevelEntry = (MazeLevelEntry)tile.TileAttributes.FirstOrDefault(attribute => attribute is MazeLevelEntry);
        if (MazeLevelEntry == null)
        {
            tileAttributeRemover.RemovePlayerSpawnpoint();
            tileAttributeRemover.RemoveTileObstacle();

            Logger.Log(Logger.Editor, $"Now place maze entry at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlaceMazeLevelEntry();
            return;
        }

        tileAttributeRemover.RemoveMazeLevelEntry();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[5];
    }

    public override void InstantiateModifierActions()
    {
        Logger.Log("Load actions for triggerer");
        GameObject.Instantiate(EditorCanvasUI.Instance.AssignMazeLevelEntryPrefab, EditorOverworldTileModificationPanel.Instance.TileModifierActionsContainer);
    }
}
