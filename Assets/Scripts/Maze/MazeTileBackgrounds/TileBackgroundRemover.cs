using System.Collections.Generic;
using System.Linq;

public class TileBackgroundRemover
{
    private Tile _tile;

    public TileBackgroundRemover(Tile tile)
    {
        _tile = tile;
    }

    public void RemovePath()
    {
        MazeTilePath mazeTilePath = (MazeTilePath)_tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTilePath);
        if (mazeTilePath == null) return;

        MazeTilePathType mazeTilePathType = mazeTilePath.MazeTilePathType;
        int oldConnectionScore = mazeTilePath.ConnectionScore;

        // If needed, place a background in the gap that the removed path left
        if (oldConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            TileBackgroundPlacer tileBackgroundPlacer = new TileBackgroundPlacer(_tile);
            tileBackgroundPlacer.PlaceBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
        }

        _tile.MazeTileBackgrounds.Remove(mazeTilePath);
        mazeTilePath.Remove();

        TrySetTileNotMarkable();


        //After removing tile, check with neighbour tiles if wall connections should be updated
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            MazeTilePath mazeTilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (mazeTilePathOnNeighbour == null) continue;

            int oldConnectionScoreOnNeighbour = mazeTilePathOnNeighbour.ConnectionScore;

            Logger.Log($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");
            int mazeTilePathConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, mazeTilePathType);
            Logger.Log($"We calculated an path connection type score of {mazeTilePathConnectionScoreOnNeighbour} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            mazeTilePathOnNeighbour.WithConnectionScore(mazeTilePathConnectionScoreOnNeighbour);

            //Add background where needed
            if (oldConnectionScoreOnNeighbour == NeighbourTileCalculator.ConnectionOnAllSidesScore && mazeTilePathConnectionScoreOnNeighbour != NeighbourTileCalculator.ConnectionOnAllSidesScore)
            {
                TileBackgroundPlacer tileBackgroundPlacer = new TileBackgroundPlacer(neighbour.Value);
                tileBackgroundPlacer.PlaceBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
            }
        }
    }

    public void RemoveBaseBackground(MazeTileBaseBackgroundType mazeTileBaseBackgroundType)
    {
        MazeTileBaseBackground mazeTileBaseBackground = (MazeTileBaseBackground)_tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTileBaseBackground);

        if (mazeTileBaseBackground == null) return;

        _tile.MazeTileBackgrounds.Remove(mazeTileBaseBackground);
        mazeTileBaseBackground.Remove();
    }

    private void TrySetTileNotMarkable()
    {
        MazeTilePath mazeTilePath = (MazeTilePath)_tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTilePath);

        if (mazeTilePath == null)
        {
            _tile.TryMakeMarkable(false);
        }
    }
}
