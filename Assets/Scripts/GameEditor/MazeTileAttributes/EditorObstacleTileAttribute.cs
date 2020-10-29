using UnityEngine;

public class EditorObstacleTileAttribute : IEditorMazeTileAttribute
{
    public string Name { get => "Obstacle"; }
    public Sprite Sprite { get => null; }
    public EditorMazeTileAttributeType AttributeType { get => EditorMazeTileAttributeType.Obstacle; }

    public void PlaceAttribute(Tile tile)
    {
        if (tile.Walkable)
        {
            tile.PlaceTileObstacle();
            return;
        }

        // Tile is already blocked
        tile.RemoveTileObstacle();
    }
}
