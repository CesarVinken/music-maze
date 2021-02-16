﻿using System.Collections.Generic;
using UnityEngine;

public class MazeTileAttributePlacer<T> : TileAttributePlacer<T> where T : MazeTile
{
    public override T Tile { get; set; }
    //public ITileAttribute InstantiateTileAttributeGO<U>() where U : ITileAttribute;

    public override ITileAttribute InstantiateTileAttributeGO<U>()
    {
        GameObject tileAttributeGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileAttributePrefab<U>(), Tile.transform);
        return tileAttributeGO.GetComponent<U>();
    }

    // Loading a player exit for a tile, not creating a new one. We already have the connection score
    public void PlacePlayerExit(ObstacleType obstacleType, TileConnectionScoreInfo obstacleConnectionScoreInfo)
    {
        PlayerExit playerExit = (PlayerExit)InstantiateTileAttributeGO<PlayerExit>();
        playerExit.WithObstacleType(obstacleType);
        playerExit.WithConnectionScoreInfo(obstacleConnectionScoreInfo);

        Tile.Walkable = false;
        Tile.TryMakeMarkable(false);
        Tile.TileAttributes.Add(playerExit);
    }

    // Loading a tile obstacle for a tile, not creating a new one. We already have the connection score
    public void PlaceTileObstacle(ObstacleType obstacleType, TileConnectionScoreInfo obstacleConnectionScore)
    {
        TileObstacle tileObstacle = (TileObstacle)InstantiateTileAttributeGO<TileObstacle>();
        tileObstacle.WithObstacleType(obstacleType);
        tileObstacle.WithConnectionScoreInfo(obstacleConnectionScore);

        Tile.Walkable = false;
        Tile.TryMakeMarkable(false);
        Tile.TileAttributes.Add(tileObstacle);
    }

    public void PlaceEnemySpawnpoint()
    {
        EnemySpawnpoint enemySpawnpoint = (EnemySpawnpoint)InstantiateTileAttributeGO<EnemySpawnpoint>();

        Tile.Walkable = true;
        Tile.TryMakeMarkable(true);
        Tile.TileAttributes.Add(enemySpawnpoint);
    }

    public void PlacePlayerOnlyAttribute(PlayerOnlyType playerOnlyType)
    {
        PlayerOnly playerOnly = (PlayerOnly)InstantiateTileAttributeGO<PlayerOnly>();

        Tile.Walkable = true;
        Tile.TileAttributes.Add(playerOnly);
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