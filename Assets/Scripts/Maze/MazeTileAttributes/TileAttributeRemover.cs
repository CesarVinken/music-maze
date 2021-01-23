using System.Collections.Generic;
using System.Linq;

public class TileAttributeRemover
{
    private EditorTile _tile;

    public TileAttributeRemover(EditorTile tile)
    {
        _tile = tile;
    }

    public void RemovePlayerExit()
    {
        _tile.Walkable = true;
        PlayerExit playerExit = (PlayerExit)_tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerExit);
        if (playerExit == null) return;

        ObstacleType obstacleType = playerExit.ObstacleType;

        _tile.MazeTileAttributes.Remove(playerExit);
        playerExit.Remove();

        UpdateNeighboursForRemovedObstacle(obstacleType);
    }

    public void RemoveTileObstacle()
    {
        _tile.Walkable = true;
        TileObstacle tileObstacle = (TileObstacle)_tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);
        if (tileObstacle == null) return;
        if (tileObstacle is PlayerExit) return;

        ObstacleType obstacleType = tileObstacle.ObstacleType;
        int oldConnectionScore = tileObstacle.ConnectionScore;

        // If needed, place a background in the gap that the removed path left. 
        // OPTIMISATION: Currently only looking at connection score from obstacles, but should also take eg. door attributes into account.
        if (oldConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            EditorTileBackgroundPlacer tileBackgroundPlacer = new EditorTileBackgroundPlacer(_tile);
            tileBackgroundPlacer.PlaceBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
        }

        _tile.MazeTileAttributes.Remove(tileObstacle);
        tileObstacle.Remove();

        //After removing tile, check with neighbour tiles if wall connections should be updated
        UpdateNeighboursForRemovedObstacle(obstacleType);
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

    public void RemovePlayerOnlyAttribute()
    {
        IMazeTileAttribute playerOnlyAttribute = (PlayerOnly)_tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerOnly);
        if (playerOnlyAttribute == null) return;
        _tile.MazeTileAttributes.Remove(playerOnlyAttribute);
        playerOnlyAttribute.Remove();
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
                EditorTileBackgroundPlacer tileBackgroundPlacer = new EditorTileBackgroundPlacer(neighbour.Value as EditorTile);
                tileBackgroundPlacer.PlaceBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
            }
        }
    }
}