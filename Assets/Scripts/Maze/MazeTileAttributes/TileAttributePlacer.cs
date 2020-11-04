﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileAttributePlacer
{
    private Tile _tile;

    public TileAttributePlacer(Tile tile)
    {
        _tile = tile;
    }

    public void PlacePlayerExit()
    {
        GameObject playerExitGO = GameObject.Instantiate(MazeLevelManager.Instance.PlayerExitPrefab, _tile.transform);
        PlayerExit playerExit = playerExitGO.GetComponent<PlayerExit>();

        _tile.Walkable = false;
        _tile.Markable = false;
        _tile.MazeTileAttributes.Add(playerExit);
    }

    public void RemovePlayerExit()
    {
        _tile.Walkable = true;
        IMazeTileAttribute playerExit = (PlayerExit)_tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerExit);
        if (playerExit == null) return;
        _tile.MazeTileAttributes.Remove(playerExit);
        playerExit.Remove();
    }

    // Called in editor
    public void PlaceTileObstacle(ObstacleType obstacleType)
    {
        // check connections of this tile
        int obstacleConnectionScore = MapNeighbourObstaclesOfTile(_tile, obstacleType);

        GameObject tileObstacleGO = GameObject.Instantiate(MazeLevelManager.Instance.TileObstaclePrefab, _tile.transform);
        TileObstacle tileObstacle = tileObstacleGO.GetComponent<TileObstacle>();
        tileObstacle.WithObstacleType(obstacleType);
        tileObstacle.WithObstacleConnectionScore(obstacleConnectionScore);
        _tile.Walkable = false;
        _tile.Markable = false;
        _tile.MazeTileAttributes.Add(tileObstacle);

        // after adding obstacle to this tile, update connections of neighbours
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            TileObstacle tileObstacleOnNeighbour = neighbour.Value.TryGetTileObstacle();
            
            if (tileObstacleOnNeighbour == null) continue;
            Logger.Log($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");
            int obstacleConnectionScoreOnNeighbour = MapNeighbourObstaclesOfTile(neighbour.Value, obstacleType);
            //update connection score on neighbour
            tileObstacleOnNeighbour.WithObstacleConnectionScore(obstacleConnectionScoreOnNeighbour);
        }
    }

    // Called in game
    public void PlaceTileObstacle(ObstacleType obstacleType, int obstacleConnectionScore)
    {
        GameObject tileObstacleGO = GameObject.Instantiate(MazeLevelManager.Instance.TileObstaclePrefab, _tile.transform);
        TileObstacle tileObstacle = tileObstacleGO.GetComponent<TileObstacle>();
        tileObstacle.WithObstacleType(obstacleType);

        tileObstacle.WithObstacleConnectionScore(obstacleConnectionScore);

        _tile.Walkable = false;
        _tile.Markable = false;
        _tile.MazeTileAttributes.Add(tileObstacle);
    }

    private int MapNeighbourObstaclesOfTile(Tile tile, ObstacleType obstacleType)
    {
        Logger.Log($"Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}");
        bool obstacleRight = false;
        bool obstacleDown = false;
        bool obstacleLeft = false;
        bool obstacleUp = false;
        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourObstaclesOnSelf was not called from the editor");
            return -1;
        }

        MazeLevel mazeLevel = EditorManager.EditorLevel;

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
                Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");
            if (neighbour.Key == ObjectDirection.Right) {
                TileObstacle tileObstacle = neighbour.Value.TryGetTileObstacle();
                    Logger.Log($"Tried to find tileObstacle on {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");
                if (tileObstacle != null && tileObstacle.ObstacleType == obstacleType)
                {
                    obstacleRight = true;
                }
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                TileObstacle tileObstacle = neighbour.Value.TryGetTileObstacle();
                if (tileObstacle != null && tileObstacle.ObstacleType == obstacleType)
                {
                    obstacleDown = true;
                }
            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                TileObstacle tileObstacle = neighbour.Value.TryGetTileObstacle();
                if (tileObstacle != null && tileObstacle.ObstacleType == obstacleType)
                {
                    obstacleLeft = true;
                }
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                TileObstacle tileObstacle = neighbour.Value.TryGetTileObstacle();
                if (tileObstacle != null && tileObstacle.ObstacleType == obstacleType)
                {
                    obstacleUp = true;
                }
            }
        }
       
        int obstacleConnectionScore = CalculateObstacleConnectionScore(obstacleRight, obstacleDown, obstacleLeft, obstacleUp);
        Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScore} for location {tile.GridLocation.X}, {tile.GridLocation.Y}");

        return obstacleConnectionScore;
    }

    public void RemoveTileObstacle()
    {
        _tile.Walkable = true;
        TileObstacle tileObstacle = (TileObstacle)_tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);
        if (tileObstacle == null) return;

        ObstacleType obstacleType = tileObstacle.ObstacleType;
        _tile.MazeTileAttributes.Remove(tileObstacle);
        tileObstacle.Remove();

        //After removing tile, check with neighbour tiles if wall connections should be updated
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            TileObstacle tileObstacleOnNeighbour = neighbour.Value.TryGetTileObstacle();

            if (tileObstacleOnNeighbour == null) continue;
            Logger.Log($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");
            int obstacleConnectionScoreOnNeighbour = MapNeighbourObstaclesOfTile(neighbour.Value, obstacleType);
            //update connection score on neighbour
            tileObstacleOnNeighbour.WithObstacleConnectionScore(obstacleConnectionScoreOnNeighbour);
        }
    }

    public void PlacePlayerSpawnpoint()
    {
        GameObject playerSpawnpointGO = GameObject.Instantiate(MazeLevelManager.Instance.PlayerSpawnpointPrefab, _tile.transform);
        PlayerSpawnpoint playerSpawnpoint = playerSpawnpointGO.GetComponent<PlayerSpawnpoint>();

        _tile.Walkable = true;
        _tile.Markable = false;

        _tile.MazeTileAttributes.Add(playerSpawnpoint);
    }

    public void RemovePlayerSpawnpoint()
    {
        IMazeTileAttribute playerSpawnpoint = (PlayerSpawnpoint)_tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null) return;
        _tile.MazeTileAttributes.Remove(playerSpawnpoint);
        playerSpawnpoint.Remove();
    }

    public void PlaceEnemySpawnpoint()
    {
        GameObject enemySpawnpointGO = GameObject.Instantiate(MazeLevelManager.Instance.EnemySpawnpointPrefab, _tile.transform);
        EnemySpawnpoint enemySpawnpoint = enemySpawnpointGO.GetComponent<EnemySpawnpoint>();

        _tile.Walkable = true;
        _tile.Markable = true;

        _tile.MazeTileAttributes.Add(enemySpawnpoint);
    }

    public void RemoveEnemySpawnpoint()
    {
        IMazeTileAttribute enemySpawnpoint = (EnemySpawnpoint)_tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is EnemySpawnpoint);
        if (enemySpawnpoint == null) return;
        _tile.MazeTileAttributes.Remove(enemySpawnpoint);
        enemySpawnpoint.Remove();
    }

    private int CalculateObstacleConnectionScore(bool right, bool down, bool left, bool up)
    {
        if (right)
        {
            if (down)
            {
                if (left)
                {
                    if (up)
                    {
                        return 16;
                    }
                    return 12;
                }
                else if (up)
                {
                    return 13;
                }
                return 6;
            }
            else if (left)
            {
                if (up)
                {
                    return 14;
                }
                return 7;
            }
            else if (up)
            {
                return 8;
            }
            return 2;
        }
        else if (down)
        {
            if (left)
            {
                if (up)
                {
                    return 15;
                }
                return 10;
            }
            else if (up)
            {
                return 9;
            }
            return 3;
        }
        else if (left)
        {
            if (up)
            {
                return 11;
            }
            return 4;
        }
        else if (up)
        {
            return 5;
        }

        return 1;
    }
}