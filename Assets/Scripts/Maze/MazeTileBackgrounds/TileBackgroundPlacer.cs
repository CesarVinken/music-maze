using System.Collections.Generic;
using UnityEngine;

public class TileBackgroundPlacer
{
    private Tile _tile;

    public TileBackgroundPlacer(Tile tile)
    {
        _tile = tile;
    }

    //private IMazeTileAttribute InstantiateTileBackgroundGO<T>() where T : IMazeTileAttribute
    //{
    //    GameObject tileAttributeGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileAttributePrefab<T>(), _tile.transform);
    //    return tileAttributeGO.GetComponent<T>();
    //}

    public void PlacePath(MazeTilePathType mazeTilePathType)
    {
        Logger.Warning("Start placing path.....");
        int pathConnectionScore = NeighbourTileCalculator.MapNeighbourPathsOfTile(_tile, mazeTilePathType);
        Logger.Log($"Found a score of {pathConnectionScore}");
        GameObject mazeTilePathGO = GameObject.Instantiate(MazeLevelManager.Instance.TilePathPrefab, _tile.transform);
        MazeTilePath mazeTilePath = mazeTilePathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithPathType(MazeTilePathType.Default);
        mazeTilePath.SetSprite(pathConnectionScore);
        _tile.MazeTileBackgrounds.Add(mazeTilePath as IMazeTileBackground);

        // Update pathConnections for neighbouring tiles
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            MazeTilePath tilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (tilePathOnNeighbour == null) continue;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");

            int mazeTilePathConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, mazeTilePathType);
            Logger.Log($"We calculated an maze connection type score of neighbour {mazeTilePathConnectionScoreOnNeighbour} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tilePathOnNeighbour.WithPathConnectionScore(mazeTilePathConnectionScoreOnNeighbour);
        }
    }
}
