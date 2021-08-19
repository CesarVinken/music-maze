using System.Collections.Generic;
using System.Linq;

public class MazeTileBackgroundRemover : TileBackgroundRemover
{
    private EditorMazeTile _tile;

    public MazeTileBackgroundRemover(EditorMazeTile tile)
    {
        _tile = tile;
    }

    public override void RemovePath()
    {
        MazeTilePath mazeTilePath = (MazeTilePath)_tile.GetBackgrounds().FirstOrDefault(background => background is MazeTilePath);
        if (mazeTilePath == null) return;

        Logger.Log(mazeTilePath.TilePathType);
        IPathType mazeTilePathType = mazeTilePath.TilePathType;
        int oldConnectionScore = mazeTilePath.ConnectionScore;
        Logger.Log($"Old path score: {oldConnectionScore}");

        // If needed, place a background in the gap that the removed path left
        if (oldConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            Logger.Log($"Place background in gap at {_tile.GridLocation.X},{_tile.GridLocation.Y}.");
            EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(_tile);
            tileBackgroundPlacer.PlaceCoveringBaseGround(); // place background with connections on all sides
        }

        _tile.RemoveBackground(mazeTilePath);
        mazeTilePath.Remove();

        TrySetTileNotMarkable();


        //After removing tile, check with neighbour tiles if wall connections should be updated
        foreach (KeyValuePair<Direction, Tile> neighbour in _tile.Neighbours)
        {
            if (!neighbour.Value) continue;

            TilePath mazeTilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (mazeTilePathOnNeighbour == null) continue;

            int oldConnectionScoreOnNeighbour = mazeTilePathOnNeighbour.ConnectionScore;

            Logger.Warning($"We will now look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");
            TileConnectionScoreInfo mazeTilePathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, mazeTilePathType);
            Logger.Log($"We calculated an path connection type score of {mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            mazeTilePathOnNeighbour.WithConnectionScoreInfo(mazeTilePathConnectionScoreOnNeighbourInfo);

            //Add background where needed
            if (oldConnectionScoreOnNeighbour == NeighbourTileCalculator.ConnectionOnAllSidesScore && 
                mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore != NeighbourTileCalculator.ConnectionOnAllSidesScore)
            {
                EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(neighbour.Value as EditorMazeTile);
                tileBackgroundPlacer.PlaceCoveringBaseGround();
            }
        }

        _tile.RemoveTileAsBeautificationTrigger();
    }

    public override void RemoveBackground<T>()
    {
        T mazeTileBackground = (T)_tile.GetBackgrounds().FirstOrDefault(background => background.GetType() == typeof(T));
        Logger.Log($"Did we find mazeTileBackground {typeof(T)}? {mazeTileBackground != null}");
        if (mazeTileBackground == null) return;
        Logger.Log($"Remove background of type {mazeTileBackground.GetType()}");
        _tile.RemoveBackground(mazeTileBackground);
        mazeTileBackground.Remove();
    }

    private void TrySetTileNotMarkable()
    {
        MazeTilePath mazeTilePath = (MazeTilePath)_tile.GetBackgrounds().FirstOrDefault(background => background is MazeTilePath);

        if (mazeTilePath == null)
        {
            _tile.TryMakeMarkable(false);
        }
    }
}
