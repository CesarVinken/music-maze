using System;
using System.Collections.Generic;
using System.Linq;

public class MazeTileAttributeRemover : TileAttributeRemover
{
    private EditorMazeTile _tile;

    public MazeTileAttributeRemover(EditorMazeTile tile)
    {
        _tile = tile;
    }

    public void RemovePlayerExit(PlayerExit playerExit = null)
    {
        if(playerExit == null)
            playerExit = (PlayerExit)_tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerExit);
        
        if (playerExit == null) return;
        
        _tile.Walkable = true;

        ObstacleType obstacleType = playerExit.ObstacleType;

        _tile.RemoveAttribute(playerExit);
        playerExit.Remove();

        UpdateNeighboursForRemovedObstacle(obstacleType);
    }

    public override void RemoveTileObstacle(TileObstacle tileObstacle = null)
    {
        if(!tileObstacle)
            tileObstacle = (TileObstacle)_tile.GetAttributes().FirstOrDefault(attribute => attribute is TileObstacle);
        
        if (tileObstacle == null) return;
        if (tileObstacle is PlayerExit) return;

        _tile.Walkable = true;

        ObstacleType obstacleType = tileObstacle.ObstacleType;
        int oldConnectionScore = tileObstacle.ConnectionScore;

        // If needed, place a background in the gap that the removed path left. 
        // OPTIMISATION: Currently only looking at connection score from obstacles, but should also take eg. door attributes into account.
        if (oldConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(_tile);
            tileBackgroundPlacer.PlaceBackground<MazeTileBaseGround>();
        }

        _tile.RemoveAttribute(tileObstacle);
        tileObstacle.Remove();

        //After removing tile, check with neighbour tiles if wall connections should be updated
        UpdateNeighboursForRemovedObstacle(obstacleType);
    }

    public override void RemovePlayerSpawnpoint(PlayerSpawnpoint playerSpawnpoint = null)
    {
        if(playerSpawnpoint == null)
            playerSpawnpoint = (PlayerSpawnpoint)_tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerSpawnpoint);

        if (playerSpawnpoint == null) return;

        _tile.RemoveAttribute(playerSpawnpoint);
        playerSpawnpoint.Remove();
    }

    public void RemoveEnemySpawnpoint(EnemySpawnpoint enemySpawnpoint = null)
    {
        if(enemySpawnpoint == null)
            enemySpawnpoint = (EnemySpawnpoint)_tile.GetAttributes().FirstOrDefault(attribute => attribute is EnemySpawnpoint);
    
        if (enemySpawnpoint == null) return;
        
        _tile.RemoveAttribute(enemySpawnpoint);
        enemySpawnpoint.Remove();
    }

    public void RemovePlayerOnlyAttribute(PlayerOnly playerOnlyAttribute = null)
    {
        if(playerOnlyAttribute == null)
            playerOnlyAttribute = (PlayerOnly)_tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerOnly);
    
        if (playerOnlyAttribute == null) return;
        
        _tile.RemoveAttribute(playerOnlyAttribute);
        playerOnlyAttribute.Remove();
    }

    public void Remove(ITileAttribute attribute)
    {
        switch (attribute.GetType())
        {
            case Type t when t == typeof(PlayerExit):
                RemovePlayerExit(attribute as PlayerExit);
                break;
            case Type t when t == typeof(TileObstacle):
                RemoveTileObstacle(attribute as TileObstacle);
                break;
            case Type t when t == typeof(PlayerSpawnpoint):
                RemovePlayerSpawnpoint(attribute as PlayerSpawnpoint);
                break;
            case Type t when t == typeof(EnemySpawnpoint):
                RemoveEnemySpawnpoint(attribute as EnemySpawnpoint);
                break;
            case Type t when t == typeof(PlayerOnly):
                RemovePlayerOnlyAttribute(attribute as PlayerOnly);
                break;
            default:
                Logger.Error($"Does not know how to remove attribute with type {attribute.GetType()}");
                break;
        }
    }

    private void UpdateNeighboursForRemovedObstacle(ObstacleType obstacleType)
    {
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            if (!neighbour.Value) continue;
            
            TileObstacle tileObstacleOnNeighbour = neighbour.Value.TryGetTileObstacle();

            if (tileObstacleOnNeighbour == null) continue;
            Logger.Log($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");
            TileConnectionScoreInfo obstacleConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(neighbour.Value, obstacleType);
            Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScoreOnNeighbour.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tileObstacleOnNeighbour.WithConnectionScoreInfo(obstacleConnectionScoreOnNeighbour);

            // If needed, place a background
            if (obstacleConnectionScoreOnNeighbour.RawConnectionScore != NeighbourTileCalculator.ConnectionOnAllSidesScore)
            {
                MazeTileBaseGround oldMazeTileBaseGround = (MazeTileBaseGround)neighbour.Value.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseGround);
                if(oldMazeTileBaseGround == null)
                {
                    EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(neighbour.Value as EditorMazeTile);
                    tileBackgroundPlacer.PlaceBackground<MazeTileBaseGround>();
                }
            }
        }
    }
}