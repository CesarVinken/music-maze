using System.Collections.Generic;
using System.Linq;

public class MazeTileAttributeRemover : TileAttributeRemover
{
    private EditorMazeTile _tile;

    public MazeTileAttributeRemover(EditorMazeTile tile)
    {
        _tile = tile;
    }

    public void RemovePlayerExit()
    {
        _tile.Walkable = true;
        PlayerExit playerExit = (PlayerExit)_tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerExit);
        if (playerExit == null) return;

        ObstacleType obstacleType = playerExit.ObstacleType;

        _tile.RemoveAttribute(playerExit);
        playerExit.Remove();

        UpdateNeighboursForRemovedObstacle(obstacleType);
    }

    public override void RemoveTileObstacle()
    {
        _tile.Walkable = true;
        TileObstacle tileObstacle = (TileObstacle)_tile.GetAttributes().FirstOrDefault(attribute => attribute is TileObstacle);
        if (tileObstacle == null) return;
        if (tileObstacle is PlayerExit) return;

        ObstacleType obstacleType = tileObstacle.ObstacleType;
        int oldConnectionScore = tileObstacle.ConnectionScore;

        // If needed, place a background in the gap that the removed path left. 
        // OPTIMISATION: Currently only looking at connection score from obstacles, but should also take eg. door attributes into account.
        if (oldConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(_tile);
            tileBackgroundPlacer.PlaceBaseBackground(new MazeLevelDefaultBaseBackgroundType());
        }

        _tile.RemoveAttribute(tileObstacle);
        tileObstacle.Remove();

        //After removing tile, check with neighbour tiles if wall connections should be updated
        UpdateNeighboursForRemovedObstacle(obstacleType);
    }

    public override void RemovePlayerSpawnpoint()
    {
        ITileAttribute playerSpawnpoint = (PlayerSpawnpoint)_tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null) return;
        _tile.RemoveAttribute(playerSpawnpoint);
        playerSpawnpoint.Remove();
    }

    public void RemoveEnemySpawnpoint()
    {
        ITileAttribute enemySpawnpoint = (EnemySpawnpoint)_tile.GetAttributes().FirstOrDefault(attribute => attribute is EnemySpawnpoint);
        if (enemySpawnpoint == null) return;
        _tile.RemoveAttribute(enemySpawnpoint);
        enemySpawnpoint.Remove();
    }

    public void RemovePlayerOnlyAttribute()
    {
        ITileAttribute playerOnlyAttribute = (PlayerOnly)_tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerOnly);
        if (playerOnlyAttribute == null) return;
        _tile.RemoveAttribute(playerOnlyAttribute);
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
                EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(neighbour.Value as EditorMazeTile);
                tileBackgroundPlacer.PlaceBaseBackground(new MazeLevelDefaultBaseBackgroundType());
            }
        }
    }
}