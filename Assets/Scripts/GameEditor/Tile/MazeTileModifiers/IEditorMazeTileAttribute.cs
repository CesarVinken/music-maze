public interface IEditorMazeTileAttribute : IEditorMazeTileModifierType
{
    string Name { get; }

    void PlaceAttribute(EditorTile tile);
    void PlaceAttributeVariation(EditorTile tile);
}
