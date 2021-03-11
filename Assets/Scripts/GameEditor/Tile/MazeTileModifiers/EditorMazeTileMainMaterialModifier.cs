public abstract class EditorMazeTileMainMaterialModifier : EditorTileMainMaterialModifier
{
    public override string Name { get; set; }

    public override void PlaceMainMaterial<T>(T tile)
    {
        PlaceMainMaterial(tile as EditorMazeTile);
    }

    public virtual void PlaceMainMaterial(EditorMazeTile tile)
    { }
}
