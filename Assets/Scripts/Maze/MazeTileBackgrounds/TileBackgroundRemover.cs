using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        _tile.MazeTileBackgrounds.Remove(mazeTilePath);
        mazeTilePath.Remove();

        //After removing tile, check with neighbour tiles if wall connections should be updated
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            MazeTilePath MazeTilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (MazeTilePathOnNeighbour == null) continue;
            Logger.Log($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");
            int mazeTilePathConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, mazeTilePathType);
            Logger.Log($"We calculated an path connection type score of {mazeTilePathConnectionScoreOnNeighbour} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            MazeTilePathOnNeighbour.WithPathConnectionScore(mazeTilePathConnectionScoreOnNeighbour);
        }
    }
}
