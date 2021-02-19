public class InGameOverworldTileAttributePlacer : OverworldTileAttributePlacer<InGameOverworldTile>
{
    private InGameOverworldTile _tile;

    public override InGameOverworldTile Tile { get => _tile; set => _tile = value; }

    public InGameOverworldTileAttributePlacer(InGameOverworldTile tile)
    {
        Tile = tile;
    }
}
