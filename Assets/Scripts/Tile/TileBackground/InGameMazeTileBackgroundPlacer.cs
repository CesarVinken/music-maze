
using UnityEngine;

public class InGameMazeTileBackgroundPlacer : MazeTileBackgroundPlacer<InGameMazeTile>
{
    private InGameMazeTile _tile;

    public override InGameMazeTile Tile { get => _tile; set => _tile = value; }

    public InGameMazeTileBackgroundPlacer(InGameMazeTile tile)
    {
        Tile = tile;
    }
}