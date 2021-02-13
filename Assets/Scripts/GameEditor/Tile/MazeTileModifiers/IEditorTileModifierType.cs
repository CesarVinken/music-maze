using UnityEngine;

public interface IEditorTileModifierType // includes all background and attribute classes
{
    Sprite Sprite { get; }

    Sprite GetSprite();

    void InstantiateModifierActions();
}