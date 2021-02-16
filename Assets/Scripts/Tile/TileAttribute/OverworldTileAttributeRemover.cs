﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverworldTileAttributeRemover : TileAttributeRemover
{
    private EditorOverworldTile _tile;

    public OverworldTileAttributeRemover(EditorOverworldTile tile)
    {
        _tile = tile;
    }

    public override void RemoveTileObstacle()
    {
        _tile.Walkable = true;
        TileObstacle tileObstacle = (TileObstacle)_tile.TileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);
        if (tileObstacle == null) return;
        if (tileObstacle is PlayerExit) return;

        ObstacleType obstacleType = tileObstacle.ObstacleType;
        int oldConnectionScore = tileObstacle.ConnectionScore;

        if (oldConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(_tile);
            tileBackgroundPlacer.PlaceBaseBackground(new OverworldDefaultBaseBackgroundType());
        }

        _tile.TileAttributes.Remove(tileObstacle);
        tileObstacle.Remove();

        //After removing tile, check with neighbour tiles if wall connections should be updated
        UpdateNeighboursForRemovedObstacle(obstacleType);
    }

    public override void RemovePlayerSpawnpoint()
    {
        Logger.Log("To be implemented");
        //ITileAttribute playerSpawnpoint = (PlayerSpawnpoint)_tile.TileAttributes.FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        //if (playerSpawnpoint == null) return;
        //_tile.TileAttributes.Remove(playerSpawnpoint);
        //playerSpawnpoint.Remove();
    }

    private void UpdateNeighboursForRemovedObstacle(ObstacleType obstacleType)
    {
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            TileObstacle tileObstacleOnNeighbour = neighbour.Value.TryGetTileObstacle();

            if (tileObstacleOnNeighbour == null) continue;
            Logger.Log($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");
            TileConnectionScoreInfo obstacleConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(neighbour.Value, obstacleType);
            Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScoreOnNeighbour} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tileObstacleOnNeighbour.WithConnectionScoreInfo(obstacleConnectionScoreOnNeighbour);

            // If needed, place a background
            if (obstacleConnectionScoreOnNeighbour.RawConnectionScore != NeighbourTileCalculator.ConnectionOnAllSidesScore)
            {
                EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(neighbour.Value as EditorOverworldTile);
                tileBackgroundPlacer.PlaceBaseBackground(new OverworldDefaultBaseBackgroundType());
            }
        }
    }
}