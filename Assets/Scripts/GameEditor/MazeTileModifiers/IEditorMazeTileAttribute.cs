using UnityEngine;

public interface IEditorMazeTileAttribute : IEditorMazeTileModifierType
{
    string Name { get; }
    Sprite Sprite { get; }
    //EditorMazeTileAttributeType AttributeType { get; }

    void PlaceAttribute(Tile tile);
    void PlaceAttributeVariation(Tile tile);
    Sprite GetSprite();
}
