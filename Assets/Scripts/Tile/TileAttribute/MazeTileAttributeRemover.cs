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
            case Type t when t == typeof(BridgePiece):
                RemoveBridgePiece(attribute as BridgePiece);
                break;
            case Type t when t == typeof(MusicInstrumentCase):
                RemoveMusicInstrumentCase(attribute as MusicInstrumentCase);
                break;
            case Type t when t == typeof(Sheetmusic):
                RemoveSheetmusic(attribute as Sheetmusic);
                break;
            case Type t when t == typeof(FerryRoute):
                RemoveFerryRoute(attribute as FerryRoute);
                break;
            default:
                Logger.Error($"Does not know how to remove attribute with type {attribute.GetType()}");
                break;
        }
    }

    public void Remove<T>() where T : ITileAttribute
    {
        switch (typeof(T))
        {
            case Type playerExit when playerExit == typeof(PlayerExit):
                RemovePlayerExit();
                break;
            case Type tileObstacle when tileObstacle == typeof(TileObstacle):
                RemoveTileObstacle();
                break;
            case Type playerOnly when playerOnly == typeof(PlayerOnly):
                RemovePlayerOnlyAttribute();
                break;
            case Type playerSpawnpoint when playerSpawnpoint == typeof(PlayerSpawnpoint):
                RemovePlayerSpawnpoint();
                break;
            case Type enemySpawnpoint when enemySpawnpoint == typeof(EnemySpawnpoint):
                RemoveEnemySpawnpoint();
                break;
            case Type bridgePiecePrefab when bridgePiecePrefab == typeof(BridgePiece):
                RemoveBridgePiece();
                break;
            case Type musicInstrumentCase when musicInstrumentCase == typeof(MusicInstrumentCase):
                RemoveMusicInstrumentCase();
                break;
            case Type sheetmusic when sheetmusic == typeof(Sheetmusic):
                RemoveSheetmusic();
                break;
            case Type ferryRoute when ferryRoute == typeof(FerryRoute):
                RemoveFerryRoute();
                break;
            default:
                Logger.Error($"Could not find a prefab for the tile attribute type of {typeof(T)}");
                break;
        }
    }

    private void RemovePlayerExit(PlayerExit playerExit = null)
    {
        if(playerExit == null)
            playerExit = (PlayerExit)_tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerExit);
        
        if (playerExit == null) return;
        
        _tile.SetWalkable(true);

        ObstacleType obstacleType = playerExit.ObstacleType;

        _tile.RemoveAttribute(playerExit);
        playerExit.Remove();

        UpdateNeighboursForRemovedObstacle(obstacleType);
    }

    protected override void RemoveTileObstacle(TileObstacle tileObstacle = null)
    {
        if(!tileObstacle)
            tileObstacle = (TileObstacle)_tile.GetAttributes().FirstOrDefault(attribute => attribute is TileObstacle);
        
        if (tileObstacle == null) return;
        if (tileObstacle is PlayerExit) return;

        _tile.SetWalkable(true);

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

    protected override void RemovePlayerSpawnpoint(PlayerSpawnpoint playerSpawnpoint = null)
    {
        if(playerSpawnpoint == null)
            playerSpawnpoint = (PlayerSpawnpoint)_tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerSpawnpoint);

        if (playerSpawnpoint == null) return;

        _tile.RemoveAttribute(playerSpawnpoint);
        playerSpawnpoint.Remove();
    }

    protected void RemoveEnemySpawnpoint(EnemySpawnpoint enemySpawnpoint = null)
    {
        if(enemySpawnpoint == null)
            enemySpawnpoint = (EnemySpawnpoint)_tile.GetAttributes().FirstOrDefault(attribute => attribute is EnemySpawnpoint);
    
        if (enemySpawnpoint == null) return;
        
        _tile.RemoveAttribute(enemySpawnpoint);
        MazeLevelGameplayManager.Instance.EnemyCharacterSpawnpoints.Remove(enemySpawnpoint);
        enemySpawnpoint.Remove();
    }

    protected void RemovePlayerOnlyAttribute(PlayerOnly playerOnlyAttribute = null)
    {
        if(playerOnlyAttribute == null)
            playerOnlyAttribute = (PlayerOnly)_tile.GetAttributes().FirstOrDefault(attribute => attribute is PlayerOnly);
    
        if (playerOnlyAttribute == null) return;
        
        _tile.RemoveAttribute(playerOnlyAttribute);
        playerOnlyAttribute.Remove();
    }

    protected void RemoveBridgePiece(BridgePiece bridgePieceAttribute = null)
    {
        if (bridgePieceAttribute == null)
            bridgePieceAttribute = (BridgePiece)_tile.GetAttributes().FirstOrDefault(attribute => attribute is BridgePiece);
       
        if (bridgePieceAttribute == null) return;

        bridgePieceAttribute.RemoveObsoleteBridgeEdges();

        _tile.RemoveAttribute(bridgePieceAttribute);
        bridgePieceAttribute.Remove();

        //Update path connections of neighbours
        foreach (KeyValuePair<Direction, Tile> neighbour in _tile.Neighbours)
        {
            if (!neighbour.Value) continue;

            TilePath mazeTilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (mazeTilePathOnNeighbour == null) continue;

            int oldConnectionScoreOnNeighbour = mazeTilePathOnNeighbour.ConnectionScore;
            TileConnectionScoreInfo mazeTilePathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, mazeTilePathOnNeighbour.TilePathType);

            //update connection score on neighbour
            mazeTilePathOnNeighbour.WithConnectionScoreInfo(mazeTilePathConnectionScoreOnNeighbourInfo);
        }
    }

    protected void RemoveMusicInstrumentCase(MusicInstrumentCase musicInstrumentCase = null)
    {
        if (musicInstrumentCase == null)
            musicInstrumentCase = (MusicInstrumentCase)_tile.GetAttributes().FirstOrDefault(attribute => attribute is MusicInstrumentCase);

        if (musicInstrumentCase == null) return;

        _tile.RemoveAttribute(musicInstrumentCase);
        musicInstrumentCase.Remove();
    }

    protected void RemoveSheetmusic(Sheetmusic sheetmusic = null)
    {
        if (sheetmusic == null)
            sheetmusic = (Sheetmusic)_tile.GetAttributes().FirstOrDefault(attribute => attribute is Sheetmusic);

        if (sheetmusic == null) return;

        _tile.RemoveAttribute(sheetmusic);
        sheetmusic.Remove();
    }

    protected void RemoveFerryRoute(FerryRoute ferryRoute = null)
    {
        if (ferryRoute == null)
            ferryRoute = (FerryRoute)_tile.GetAttributes().FirstOrDefault(attribute => attribute is FerryRoute);

        if (ferryRoute == null) return;

        _tile.RemoveAttribute(ferryRoute);
        ferryRoute.Remove();
    }

    private void UpdateNeighboursForRemovedObstacle(ObstacleType obstacleType)
    {
        foreach (KeyValuePair<Direction, Tile> neighbour in _tile.Neighbours)
        {
            if (!neighbour.Value) continue;
            
            TileObstacle tileObstacleOnNeighbour = neighbour.Value.TryGetAttribute<TileObstacle>();

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