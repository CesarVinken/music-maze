public interface IEditorTileAttribute : IEditorTileModifierType
{
    string Name { get; }

    void PlaceAttribute(EditorTile tile);
    void PlaceAttributeVariation(EditorTile tile);
}
