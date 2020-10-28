using UnityEngine;

public class EditorObstacleTileAttribute : IEditorMazeTileAttribute
{
    public string Name { get => "Block"; }
    public Sprite Sprite { get => null; }
    public EditorMazeTileAttributeType AttributeType { get => EditorMazeTileAttributeType.Obstacle; }

    public void PlaceAttribute(Tile tile)
    {
        if (tile.Walkable)
        {
            tile.BuildTileObstacle();
            return;
        }

        // Tile is already blocked
        tile.RemoveTileObstacle();
    }
}
