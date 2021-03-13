using UnityEngine;

public abstract class EditorTileBackgroundModifier : IEditorTileModifier
{
    public abstract string Name { get; set; }

    public abstract Sprite Sprite { get; set; }

    public abstract Sprite GetSprite();

    public abstract void InstantiateModifierActions();
    public abstract void DestroyModifierActions();

    public abstract void PlaceBackground<T>(T tile) where T : Tile;

    public abstract void PlaceBackgroundVariation<T>(T tile) where T : Tile;
}

