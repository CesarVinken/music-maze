using System.Linq;
using UnityEngine;

public class EditorOverworldMazeEntryTileAttribute : EditorOverworldTileAttributeModifier
{
    public override string Name { get => "Maze Entry"; }

    public override void PlaceAttribute(EditorOverworldTile tile)
    {
        EditorOverworldTileAttributePlacer tileAttributePlacer = new EditorOverworldTileAttributePlacer(tile);
        OverworldTileAttributeRemover tileAttributeRemover = new OverworldTileAttributeRemover(tile);

        ITileAttribute mazeEntry = (MazeEntry)tile.TileAttributes.FirstOrDefault(attribute => attribute is MazeEntry);
        if (mazeEntry == null)
        {
            tileAttributeRemover.RemovePlayerSpawnpoint();
            tileAttributeRemover.RemoveTileObstacle();

            Logger.Log(Logger.Editor, $"Now place maze entry at {tile.GridLocation.X}, {tile.GridLocation.Y}");
            tileAttributePlacer.PlaceMazeEntry();
            return;
        }

        tileAttributeRemover.RemoveMazeEntry();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[5];
    }
}
