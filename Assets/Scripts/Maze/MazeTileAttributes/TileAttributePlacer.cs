using System.Collections.Generic;

public abstract class TileAttributePlacer<T> where T : Tile
{
    public abstract T Tile { get; set; }

    public abstract IMazeTileAttribute InstantiateTileAttributeGO<U>() where U : IMazeTileAttribute;

    // Loading a player exit for a tile, not creating a new one. We already have the connection score
    public void PlacePlayerExit(ObstacleType obstacleType, TileConnectionScoreInfo obstacleConnectionScoreInfo)
    {
        PlayerExit playerExit = (PlayerExit)InstantiateTileAttributeGO<PlayerExit>();
        playerExit.WithObstacleType(obstacleType);
        playerExit.WithConnectionScoreInfo(obstacleConnectionScoreInfo);

        Tile.Walkable = false;
        Tile.TryMakeMarkable(false);
        Tile.MazeTileAttributes.Add(playerExit);
    }

    // Loading a tile obstacle for a tile, not creating a new one. We already have the connection score
    public void PlaceTileObstacle(ObstacleType obstacleType, TileConnectionScoreInfo obstacleConnectionScore)
    {
        TileObstacle tileObstacle = (TileObstacle)InstantiateTileAttributeGO<TileObstacle>();
        tileObstacle.WithObstacleType(obstacleType);
        tileObstacle.WithConnectionScoreInfo(obstacleConnectionScore);

        Tile.Walkable = false;
        Tile.TryMakeMarkable(false);
        Tile.MazeTileAttributes.Add(tileObstacle);
    }

    public void PlacePlayerSpawnpoint()
    {
        PlayerSpawnpoint playerSpawnpoint = (PlayerSpawnpoint)InstantiateTileAttributeGO<PlayerSpawnpoint>();

        Tile.Walkable = true;
        Tile.TryMakeMarkable(false);

        Tile.MazeTileAttributes.Add(playerSpawnpoint);
    }

    public void PlaceEnemySpawnpoint()
    {
        EnemySpawnpoint enemySpawnpoint = (EnemySpawnpoint)InstantiateTileAttributeGO<EnemySpawnpoint>();

        Tile.Walkable = true;
        Tile.TryMakeMarkable(true);
        Tile.MazeTileAttributes.Add(enemySpawnpoint);
    }

    public void PlacePlayerOnlyAttribute(PlayerOnlyType playerOnlyType)
    {
        PlayerOnly playerOnly = (PlayerOnly)InstantiateTileAttributeGO<PlayerOnly>();

        Tile.Walkable = true;
        Tile.MazeTileAttributes.Add(playerOnly);
    }

    public void PlaceTileObstacleVariation(TileObstacle tileObstacle)
    {
        //return only connections that were updated
        List<TileObstacle> updatedObstacleConnections = NeighbourTileCalculator.GetUpdatedTileModifiersForVariation(Tile, tileObstacle, tileObstacle.ObstacleType.ToString());

        //update the sprites with the new variations
        for (int i = 0; i < updatedObstacleConnections.Count; i++)
        {
            updatedObstacleConnections[i].WithConnectionScoreInfo(new TileConnectionScoreInfo(updatedObstacleConnections[i].ConnectionScore, updatedObstacleConnections[i].SpriteNumber));
        }
    }
}