public interface IEditorTileAttribute<T> : IEditorTileModifierType where T : Tile
{
    string Name { get; }

    void PlaceAttribute(T tile);
    void PlaceAttributeVariation(T tile);
}
