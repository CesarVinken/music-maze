public interface IEditorMazeTileAttribute : IEditorMazeTileModifierType
{
    string Name { get; }

    void PlaceAttribute(Tile tile);
    void PlaceAttributeVariation(Tile tile);
}
