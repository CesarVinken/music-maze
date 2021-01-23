public interface IEditorMazeTileBackground : IEditorMazeTileModifierType
{
    string Name { get; }

    void PlaceBackground(Tile tile);
    void PlaceBackgroundVariation(Tile tile);
}
