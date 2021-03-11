public abstract class EditorTileMainMaterialModifier : EditorTileModifier
{
    public abstract string Name { get; set; }

    public abstract void PlaceMainMaterial<T>(T tile) where T : Tile;
}
