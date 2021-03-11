public abstract class EditorOverworldTileMainMaterialModifier : EditorTileMainMaterialModifier
{
    public override string Name { get; set; }

    public override void PlaceMainMaterial<T>(T tile)
    {
        PlaceMainMaterial(tile as EditorOverworldTile);
    }

    public virtual void PlaceMainMaterial(EditorOverworldTile tile)
    { }

}
