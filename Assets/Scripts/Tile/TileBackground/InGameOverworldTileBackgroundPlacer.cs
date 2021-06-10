using UnityEngine;

public class InGameOverworldTileBackgroundPlacer : OverworldTileBackgroundPlacer<InGameOverworldTile>
{
    private InGameOverworldTile _tile;

    public override InGameOverworldTile Tile { get => _tile; set => _tile = value; }

    public InGameOverworldTileBackgroundPlacer(InGameOverworldTile tile)
    {
        Tile = tile;
    }

    public void PlaceCornerFiler(TileCorner tileCorner)
    {
        //create cornerfiller
        GameObject backgroundGO = GameObject.Instantiate(OverworldGameplayManager.Instance.GetTileBackgroundPrefab<TileCornerFiller>(), _tile.BackgroundsContainer);
        TileCornerFiller cornerFiller = backgroundGO.GetComponent<TileCornerFiller>();

        cornerFiller.SetTile(_tile);
        cornerFiller.WithType(new OverworldDefaultGroundType());
        cornerFiller.WithCorner(tileCorner); // pick sprite based on corner

        _tile.AddCornerFiller(cornerFiller);
    }
}
