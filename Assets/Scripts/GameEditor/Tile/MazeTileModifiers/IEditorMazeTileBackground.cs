public interface IEditorTileBackground : IEditorTileModifierType
{
    string Name { get; }

    void PlaceBackground(EditorTile tile);
    void PlaceBackgroundVariation(EditorTile tile);
}
