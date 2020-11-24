using UnityEngine;

public interface IEditorMazeTileBackground : IEditorMazeTileModifierType
{
    string Name { get; }
    Sprite Sprite { get; }

    void PlaceBackground(Tile tile);
}
