using UnityEngine;

public abstract class EditorTileAreaModifier : IEditorTileModifier
{
    public abstract string Name { get; set; }
    public abstract Sprite Sprite { get; set; }

    public abstract Sprite GetSprite();

    public abstract void InstantiateModifierActions();

    public abstract void SetSelectedTile<T>(T tile) where T : Tile;
}
