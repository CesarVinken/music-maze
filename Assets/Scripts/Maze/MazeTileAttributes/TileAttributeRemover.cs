using System.Collections.Generic;
using System.Linq;

public class TileAttributeRemover
{
    private Tile _tile;

    public TileAttributeRemover(Tile tile)
    {
        _tile = tile;
    }

    public void RemovePlayerExit()
    {
        RemoveTileObstacle();
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
            int obstacleConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(neighbour.Value, obstacleType, false);
            Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScoreOnNeighbour} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tileObstacleOnNeighbour.WithObstacleConnectionScore(obstacleConnectionScoreOnNeighbour);
        }
    }

    public void RemovePlayerSpawnpoint()
    {
        IMazeTileAttribute playerSpawnpoint = (PlayerSpawnpoint)_tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null) return;
        _tile.MazeTileAttributes.Remove(playerSpawnpoint);
        playerSpawnpoint.Remove();
    }

    public void RemoveEnemySpawnpoint()
    {
        IMazeTileAttribute enemySpawnpoint = (EnemySpawnpoint)_tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is EnemySpawnpoint);
        if (enemySpawnpoint == null) return;
        _tile.MazeTileAttributes.Remove(enemySpawnpoint);
        enemySpawnpoint.Remove();
    }
}