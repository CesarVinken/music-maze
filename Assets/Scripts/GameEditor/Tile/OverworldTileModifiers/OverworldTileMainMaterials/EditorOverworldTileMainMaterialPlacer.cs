public class EditorOverworldTileMainMaterialPlacer : OverworldTileMainMaterialPlacer<EditorOverworldTile>
{
    private EditorOverworldTile _tile;

    public override EditorOverworldTile Tile { get => _tile; set => _tile = value; }

    public EditorOverworldTileMainMaterialPlacer(EditorOverworldTile tile)
    {
        Tile = tile;
    }

}
