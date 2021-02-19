public class InGameOverworldTileBackgroundPlacer : OverworldTileBackgroundPlacer<InGameOverworldTile>
{
    private InGameOverworldTile _tile;

    public override InGameOverworldTile Tile { get => _tile; set => _tile = value; }

    public InGameOverworldTileBackgroundPlacer(InGameOverworldTile tile)
    {
        Tile = tile;
    }
}
