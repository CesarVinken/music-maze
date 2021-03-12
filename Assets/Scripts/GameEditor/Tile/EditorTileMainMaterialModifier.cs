using UnityEngine;

public abstract class EditorTileMainMaterialModifier : EditorTileModifier
{
    public abstract string Name { get; set; }
    public abstract Sprite Sprite { get; set; }

    public abstract Sprite GetSprite();

    public abstract void PlaceMainMaterial<T>(T tile) where T : Tile;
}
