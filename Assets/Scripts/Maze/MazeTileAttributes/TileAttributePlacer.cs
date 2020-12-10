using System.Collections.Generic;
using UnityEngine;

public class TileAttributePlacer
{
    private Tile _tile;

    public TileAttributePlacer(Tile tile)
    {
        _tile = tile;
    }

    private IMazeTileAttribute InstantiateTileAttributeGO<T>() where T : IMazeTileAttribute
    {
        GameObject tileAttributeGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileAttributePrefab<T>(), _tile.transform);
        return tileAttributeGO.GetComponent<T>();
    }

    // called in editor when calculations are needed
    public void PlacePlayerExit(ObstacleType obstacleType)
    {
        int obstacleConnectionScore = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(_tile, obstacleType, true);
        Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScore} for location {_tile.GridLocation.X}, {_tile.GridLocation.Y}");

        PlayerExit playerExit = (PlayerExit)InstantiateTileAttributeGO<PlayerExit>();
        playerExit.WithObstacleType(obstacleType);
        playerExit.WithConnectionScore(obstacleConnectionScore);

        _tile.Walkable = false;
        _tile.TryMakeMarkable(false);

        Logger.Log("Add player exit to maze tile attribute list");
        _tile.MazeTileAttributes.Add(playerExit);

        // after adding obstacle to this tile, update connections of neighbours
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            TileObstacle tileObstacleOnNeighbour = neighbour.Value.TryGetTileObstacle();

            if (tileObstacleOnNeighbour == null) continue;
            Logger.Log($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");

            bool neighbourIsDoor = false;
            if(tileObstacleOnNeighbour is PlayerExit)
            {
                neighbourIsDoor = true;
            }
            int obstacleConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(neighbour.Value, obstacleType, neighbourIsDoor);
            Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScoreOnNeighbour} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tileObstacleOnNeighbour.WithConnectionScore(obstacleConnectionScoreOnNeighbour);
        }
    }

    // Called in game when we already have the connection score
    public void PlacePlayerExit(ObstacleType obstacleType, int obstacleConnectionScore)
    {
        PlayerExit playerExit = (PlayerExit)InstantiateTileAttributeGO<PlayerExit>();
        playerExit.WithObstacleType(obstacleType);
        playerExit.WithConnectionScore(obstacleConnectionScore);

        _tile.Walkable = false;
        _tile.TryMakeMarkable(false);
        _tile.MazeTileAttributes.Add(playerExit);
    }

    // Called in editor
    public void PlaceTileObstacle(ObstacleType obstacleType)
    {
        // check connections of this tile
        int obstacleConnectionScore = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(_tile, obstacleType, false);
        Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScore} for location {_tile.GridLocation.X}, {_tile.GridLocation.Y}");

        TileObstacle tileObstacle = (TileObstacle)InstantiateTileAttributeGO<TileObstacle>();
        tileObstacle.WithObstacleType(obstacleType);
        tileObstacle.WithConnectionScore(obstacleConnectionScore);

        _tile.Walkable = false;
        _tile.TryMakeMarkable(false);
        _tile.MazeTileAttributes.Add(tileObstacle);

        // If we now have obstacle connections on all sides, remove the backgrounds
        if (obstacleConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            TileBackgroundRemover tileBackgroundRemover = new TileBackgroundRemover(_tile);
            tileBackgroundRemover.RemoveBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
        }

        // after adding obstacle to this tile, update connections of neighbours
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            TileObstacle tileObstacleOnNeighbour = neighbour.Value.TryGetTileObstacle();
            
            if (tileObstacleOnNeighbour == null) continue;

            bool neighbourIsDoor = false;
            if (tileObstacleOnNeighbour is PlayerExit)
            {
                neighbourIsDoor = true;
            }

            Logger.Log($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");
            int obstacleConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(neighbour.Value, obstacleType, neighbourIsDoor);
            Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScoreOnNeighbour} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tileObstacleOnNeighbour.WithConnectionScore(obstacleConnectionScoreOnNeighbour);

            // If we now have obstacle connections on all sides, remove the backgrounds
            if (obstacleConnectionScoreOnNeighbour == NeighbourTileCalculator.ConnectionOnAllSidesScore)
            {
                Logger.Log($"Remove the background on tile {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");
                TileBackgroundRemover tileBackgroundRemover = new TileBackgroundRemover(neighbour.Value);

                tileBackgroundRemover.RemoveBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
            }
        }
    }

    //TODO CLEAN UP: having two almost similar functions is confusing
    // Called in game
    public void PlaceTileObstacle(ObstacleType obstacleType, int obstacleConnectionScore)
    {
        TileObstacle tileObstacle = (TileObstacle)InstantiateTileAttributeGO<TileObstacle>();
        tileObstacle.WithObstacleType(obstacleType);
        tileObstacle.WithConnectionScore(obstacleConnectionScore);

        _tile.Walkable = false;
        _tile.TryMakeMarkable(false);
        _tile.MazeTileAttributes.Add(tileObstacle);
    }

    public void PlacePlayerSpawnpoint()
    {
        PlayerSpawnpoint playerSpawnpoint = (PlayerSpawnpoint)InstantiateTileAttributeGO<PlayerSpawnpoint>();

        _tile.Walkable = true;
        _tile.TryMakeMarkable(false);

        _tile.MazeTileAttributes.Add(playerSpawnpoint);
    }

    public void PlaceEnemySpawnpoint()
    {
        EnemySpawnpoint enemySpawnpoint = (EnemySpawnpoint)InstantiateTileAttributeGO<EnemySpawnpoint>();

        _tile.Walkable = true;
        _tile.TryMakeMarkable(true);
        _tile.MazeTileAttributes.Add(enemySpawnpoint);
    }

    public void PlacePlayerOnlyAttribute(PlayerOnlyType playerOnlyType)
    {
        PlayerOnly playerOnly = (PlayerOnly)InstantiateTileAttributeGO<PlayerOnly>();

        _tile.Walkable = true;
        //_tile.TryMakeMarkable(false);

        _tile.MazeTileAttributes.Add(playerOnly);
    }

    public void PlaceTileObstacleVariation(TileObstacle tileObstacle)
    {
        //return only connections that were updated
        List<TileObstacle> updatedObstacleConnections = NeighbourTileCalculator.GetUpdatedTileModifiersForVariation(_tile, tileObstacle, tileObstacle.ObstacleType.ToString());

        //update the sprites with the new variations
        for (int i = 0; i < updatedObstacleConnections.Count; i++)
        {
            updatedObstacleConnections[i].WithConnectionScore(updatedObstacleConnections[i].ConnectionScore);
        }
    }
}