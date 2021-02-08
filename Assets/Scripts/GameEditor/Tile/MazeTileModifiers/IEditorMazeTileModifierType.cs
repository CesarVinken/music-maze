using UnityEngine;

public interface IEditorMazeTileModifierType // includes all background and attribute classes
{
    Sprite Sprite { get; }

    Sprite GetSprite();

    void InstantiateModifierActions();
    void DestroyModifierActions();
}