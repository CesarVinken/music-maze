public interface IEditorTileAttribute : IEditorTileModifierType
{
    string Name { get; }

    void PlaceAttribute(EditorMazeTile tile);
    void PlaceAttributeVariation(EditorMazeTile tile);
}
