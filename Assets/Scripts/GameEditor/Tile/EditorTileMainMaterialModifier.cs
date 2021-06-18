using UnityEngine;

public abstract class EditorTileMainModifierCategory : IEditorTileModifier
{
    public abstract string Name { get; set; }
    public abstract Sprite Sprite { get; set; }

    public abstract Sprite GetSprite();
}
