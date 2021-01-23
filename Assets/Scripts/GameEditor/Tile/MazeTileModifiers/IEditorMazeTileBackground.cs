public interface IEditorMazeTileBackground : IEditorMazeTileModifierType
{
    string Name { get; }

    void PlaceBackground(EditorTile tile);
    void PlaceBackgroundVariation(EditorTile tile);
}
