using System.Collections.Generic;
using UnityEngine;


public class MazeTileAttributePlacer<T> : TileAttributePlacer<T> where T : MazeTile
{
    public override T Tile { get; set; }

    public override ITileAttribute InstantiateTileAttributeGO<U>()
    {
        GameObject tileAttributeGO = GameObject.Instantiate(MazeLevelGameplayManager.Instance.GetTileAttributePrefab<U>(), Tile.transform);
        return tileAttributeGO.GetComponent<U>();
    }

    // Loading a player exit for a tile, not creating a new one. We already have the connection score
    public void PlacePlayerExit(ObstacleType obstacleType, TileConnectionScoreInfo obstacleConnectionScoreInfo)
    {
        PlayerExit playerExit = (PlayerExit)InstantiateTileAttributeGO<PlayerExit>();
        playerExit.WithObstacleType(obstacleType);
        playerExit.WithConnectionScoreInfo(obstacleConnectionScoreInfo);

        Tile.SetWalkable(false);
        Tile.TryMakeMarkable(false);
        Tile.AddAttribute(playerExit);
    }

    // Loading a tile obstacle for a tile, not creating a new one. We already have the connection score
    public void PlaceTileObstacle(ObstacleType obstacleType, TileConnectionScoreInfo obstacleConnectionScore)
    {
        TileObstacle tileObstacle = (TileObstacle)InstantiateTileAttributeGO<TileObstacle>();
        tileObstacle.WithObstacleType(obstacleType);
        tileObstacle.WithConnectionScoreInfo(obstacleConnectionScore);

        Tile.SetWalkable(false);
        Tile.TryMakeMarkable(false);
        Tile.AddAttribute(tileObstacle);
    }

    public virtual void PlaceEnemySpawnpoint(List<string> tileAreaIds = null, Dictionary<string, TileArea> globalTileAreas = null)
    {
        EnemySpawnpoint enemySpawnpoint = (EnemySpawnpoint)InstantiateTileAttributeGO<EnemySpawnpoint>();

        Tile.SetWalkable(true);
        Tile.TryMakeMarkable(true);
        Tile.AddAttribute(enemySpawnpoint);

        MazeLevelGameplayManager.Instance.EnemyCharacterSpawnpoints.Add(enemySpawnpoint);

        if (tileAreaIds != null && globalTileAreas != null)
        {
            for (int i = 0; i < tileAreaIds.Count; i++)
            {
                TileArea tileArea = globalTileAreas[tileAreaIds[i]];
                enemySpawnpoint.AddTileArea(tileArea);
            }
        }
    }

    public void PlacePlayerOnlyAttribute(PlayerOnlyType playerOnlyType)
    {
        PlayerOnly playerOnly = (PlayerOnly)InstantiateTileAttributeGO<PlayerOnly>();

        Tile.SetWalkable(true);
        Tile.AddAttribute(playerOnly);
    }

    public void PlaceBridgePiece(BridgeType bridgeType, BridgePieceDirection bridgePieceDirection)
    {
        BridgePiece bridgePiece = (BridgePiece)InstantiateTileAttributeGO<BridgePiece>();
        bridgePiece.WithBridgeType(bridgeType);
        bridgePiece.WithBridgePieceDirection(bridgePieceDirection);
        bridgePiece.SetSprite();

        Tile.SetWalkable(true);
        Tile.TryMakeMarkable(false);
        Tile.AddAttribute(bridgePiece);

        TileWater tileWater = Tile.TryGetTileWater();
        if (tileWater)
        {
            tileWater.SetWalkabilityForBridge(bridgePieceDirection);
        }
    }

    public void PlaceBridgeEdge(BridgeType bridgeType, Direction edgeSide)
    {
        BridgeEdge bridgeEdge = (BridgeEdge)InstantiateTileAttributeGO<BridgeEdge>();
        bridgeEdge.WithBridgeEdgeSide(edgeSide);
        bridgeEdge.WithBridgeType(bridgeType);
        bridgeEdge.SetSprite();
        bridgeEdge.SetTile(Tile);

        Tile.AddBridgeEdge(bridgeEdge);
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

    public virtual void PlaceMusicInstrumentCase()
    {
        MusicInstrumentCase musicInstrumentCase = (MusicInstrumentCase)InstantiateTileAttributeGO<MusicInstrumentCase>();

        Tile.AddAttribute(musicInstrumentCase);
    }

    public virtual void PlaceSheetmusic()
    {
        Sheetmusic musicInstrumentCase = (Sheetmusic)InstantiateTileAttributeGO<Sheetmusic>();

        Tile.AddAttribute(musicInstrumentCase);
    }

    public virtual void PlaceFerryRoute(string id, List<Tile> ferryRoutePointTiles, int dockingStartDirection, int dockingEndDirection)
    {

    }
}