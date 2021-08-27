using System;
using System.Collections.Generic;
using System.Linq;
using UI;

public class OverworldTileAttributeRemover : TileAttributeRemover
{
    private EditorOverworldTile _tile;

    public OverworldTileAttributeRemover(EditorOverworldTile tile)
    {
        _tile = tile;
    }

    public void Remove(ITileAttribute attribute)
    {
        switch (attribute.GetType())
        {
            case Type t when t == typeof(TileObstacle):
                RemoveTileObstacle(attribute as TileObstacle);
                break;
            case Type t when t == typeof(PlayerSpawnpoint):
                RemovePlayerSpawnpoint(attribute as PlayerSpawnpoint);
                break;
            case Type t when t == typeof(MazeLevelEntry):
                RemoveMazeLevelEntry(attribute as MazeLevelEntry);
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
            case Type tileObstacle when tileObstacle == typeof(TileObstacle):
                RemoveTileObstacle();
                break;
            case Type playerSpawnpoint when playerSpawnpoint == typeof(PlayerSpawnpoint):
                RemovePlayerSpawnpoint();
                break;
            case Type bridgeEdgePrefab when bridgeEdgePrefab == typeof(MazeLevelEntry):
                RemoveMazeLevelEntry();
                break;
            default:
                Logger.Error($"Could not find a prefab for the tile attribute type of {typeof(T)}");
                break;
        }
    }

    protected override void RemoveTileObstacle(TileObstacle tileObstacle = null)
    {
        _tile.SetWalkable(true);

        if(!tileObstacle)
            tileObstacle = (TileObstacle)_tile.GetAttributes().FirstOrDefault(attribute => attribute is TileObstacle);
        if (tileObstacle == null) return;
        if (tileObstacle is PlayerExit) return;

        ObstacleType obstacleType = tileObstacle.ObstacleType;
        int oldConnectionScore = tileObstacle.ConnectionScore;

        if (oldConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(_tile);
            tileBackgroundPlacer.PlaceBackground<OverworldTileBaseGround>();
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

    protected void RemoveMazeLevelEntry(MazeLevelEntry mazeLevelEntry = null)
    {
        if(mazeLevelEntry == null)
            mazeLevelEntry = (MazeLevelEntry)_tile.GetAttributes().FirstOrDefault(attribute => attribute is MazeLevelEntry);

        if (mazeLevelEntry == null) return;
        
        _tile.RemoveAttribute(mazeLevelEntry);
        mazeLevelEntry.Remove();

        if (OverworldGameplayManager.Instance != null && OverworldGameplayManager.Instance.EditorOverworld != null)
        {
            OverworldGameplayManager.Instance.EditorOverworld.MazeEntries.Remove(mazeLevelEntry);
            ScreenSpaceOverworldEditorElements.Instance.RemoveMazeLevelEntryName(mazeLevelEntry);
        }

        ScreenSpaceOverworldEditorElements.Instance.TrySolveEditorIssue(EditorIssueType.MazeLevelMissing, _tile.GridLocation);
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
            Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScoreOnNeighbour} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tileObstacleOnNeighbour.WithConnectionScoreInfo(obstacleConnectionScoreOnNeighbour);

            // If needed, place a background
            if (obstacleConnectionScoreOnNeighbour.RawConnectionScore != NeighbourTileCalculator.ConnectionOnAllSidesScore)
            {
                EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(neighbour.Value as EditorOverworldTile);
                tileBackgroundPlacer.PlaceBackground<OverworldTileBaseGround>();
            }
        }
    }
}
