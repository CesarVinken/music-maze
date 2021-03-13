public class EditorMazeTileMainMaterialPlacer : MazeTileMainMaterialPlacer<EditorMazeTile>
{
    private EditorMazeTile _tile;

    public override EditorMazeTile Tile { get => _tile; set => _tile = value; }

    public EditorMazeTileMainMaterialPlacer(EditorMazeTile tile)
    {
        Tile = tile;
    }
}
