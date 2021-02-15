public interface IEditorTileBackground : IEditorTileModifierType
{
    string Name { get; }

    void PlaceBackground(EditorMazeTile tile);
    void PlaceBackgroundVariation(EditorMazeTile tile);
}
