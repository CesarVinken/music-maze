using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileBackgroundPlacer
{
    private Tile _tile;

    public TileBackgroundPlacer(Tile tile)
    {
        _tile = tile;
    }

    // Called in the editor, when we need to update the connection score
    public void PlacePath(MazeTilePathType mazeTilePathType)
    {
        Logger.Warning("Start placing path.....");

        int pathConnectionScore = NeighbourTileCalculator.MapNeighbourPathsOfTile(_tile, mazeTilePathType);
        Logger.Log($"Found a score of {pathConnectionScore}");
        GameObject mazeTilePathGO = GameObject.Instantiate(MazeLevelManager.Instance.TilePathPrefab, _tile.BackgroundsContainer);
        MazeTilePath mazeTilePath = mazeTilePathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithPathType(MazeTilePathType.Default);
        mazeTilePath.WithConnectionScore(pathConnectionScore);
        _tile.MazeTileBackgrounds.Add(mazeTilePath as IMazeTileBackground);

        // Update pathConnections for neighbouring tiles
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            MazeTilePath tilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (tilePathOnNeighbour == null) continue;
            int oldConnectionScoreOnNeighbour = tilePathOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");

            int mazeTilePathConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, mazeTilePathType);
            Logger.Log($"We calculated an maze connection type score of neighbour {mazeTilePathConnectionScoreOnNeighbour} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");


            //update connection score on neighbour
            tilePathOnNeighbour.WithConnectionScore(mazeTilePathConnectionScoreOnNeighbour);

            if(oldConnectionScoreOnNeighbour == 16 && mazeTilePathConnectionScoreOnNeighbour != 16)
            {
                PlaceBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
            }
        }
    }

    // Called in game when we already have the connection score
    public void PlacePath(MazeTilePathType mazeTilePathType, int pathConnectionScore)
    {
        GameObject mazeTilePathGO = GameObject.Instantiate(MazeLevelManager.Instance.TilePathPrefab, _tile.BackgroundsContainer);
        MazeTilePath mazeTilePath = mazeTilePathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithPathType(mazeTilePathType);
        mazeTilePath.WithConnectionScore(pathConnectionScore);
        mazeTilePath.SetTile(_tile);

        _tile.MazeTileBackgrounds.Add(mazeTilePath);
        _tile.TryMakeMarkable(true);
    }

    public void PlacePathVariation(MazeTilePath mazeTilePath)
    {
        //return only connections that were updated
        List<MazeTilePath> updatedPathConnections = NeighbourTileCalculator.GetUpdatedTileModifiersForVariation<MazeTilePath>(_tile, mazeTilePath, mazeTilePath.MazeTilePathType.ToString());

        //update the sprites with the new variations
        for (int i = 0; i < updatedPathConnections.Count; i++)
        {
            updatedPathConnections[i].WithConnectionScore(updatedPathConnections[i].ConnectionScore);
        }
    }

    public void PlaceBaseBackground(MazeTileBaseBackgroundType mazeTileBaseBackgroundType)
    {
        MazeTileBaseBackground oldBackground = (MazeTileBaseBackground)_tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTileBaseBackground);
        if (oldBackground != null) return;

        GameObject mazeTileBaseBackgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.TileBaseBackgroundPrefab, _tile.BackgroundsContainer);
        MazeTileBaseBackground mazeTileBaseBackground = mazeTileBaseBackgroundGO.GetComponent<MazeTileBaseBackground>();

        int defaultConnectionScore = -1;
        mazeTileBaseBackground.WithPathConnectionScore(defaultConnectionScore);
        mazeTileBaseBackground.SetTile(_tile);
        _tile.MazeTileBackgrounds.Add(mazeTileBaseBackground as IMazeTileBackground);
    }
}
