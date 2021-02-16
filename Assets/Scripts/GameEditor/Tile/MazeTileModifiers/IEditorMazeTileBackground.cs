public interface IEditorTileBackground<T> : IEditorTileModifierType where T : Tile
{
    string Name { get; }

    void PlaceBackground(T tile);
    void PlaceBackgroundVariation(T tile);
}
