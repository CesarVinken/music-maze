using UnityEngine;

public abstract class EditorTileTransformationModifier
{
    public abstract string Name { get; set; }

    public abstract Sprite Sprite { get; set; }
    public abstract Sprite GetSprite();

    public abstract void InstantiateModifierActions();
    public abstract void DestroyModifierActions();

    //public abstract void PlaceBackground<T>(T tile) where T : Tile;
    public abstract void HandleBeautificationTriggerPlacement<T>(T tile) where T : Tile;
    public abstract void UnsetSelectedTile();
}
