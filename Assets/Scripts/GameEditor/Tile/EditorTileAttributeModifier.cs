using UnityEngine;

public abstract class EditorTileAttributeModifier : IEditorTileModifier
{
    public abstract string Name { get; set; }

    public abstract Sprite Sprite { get; set; }

    public abstract Sprite GetSprite();

    public abstract void InstantiateModifierActions();
    public abstract void DestroyModifierActions();

    public abstract void PlaceAttribute<T>(T tile) where T : Tile;
    public abstract void PlaceAttributeVariation<T>(T tile) where T : Tile;
}