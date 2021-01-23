
public class InGameTileBackgroundPlacer : TileBackgroundPlacer<InGameTile>
{
    private InGameTile _tile;

    public override InGameTile Tile { get => _tile; set => _tile = value; }

    public InGameTileBackgroundPlacer(InGameTile tile)
    {
        Tile = tile;
    }
}