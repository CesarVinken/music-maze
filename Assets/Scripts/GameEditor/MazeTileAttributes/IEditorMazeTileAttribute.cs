using UnityEngine;

public interface IEditorMazeTileAttribute
{
    string Name { get; }
    Sprite Sprite { get; }
    EditorMazeTileAttributeType AttributeType { get; }

    void PlaceAttribute(Tile tile);
}

