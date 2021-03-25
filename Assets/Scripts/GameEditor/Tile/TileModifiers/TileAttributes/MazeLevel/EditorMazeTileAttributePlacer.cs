using System.Collections.Generic;
using UnityEngine;

public class EditorMazeTileAttributePlacer : MazeTileAttributePlacer<EditorMazeTile>
{
    private EditorMazeTile _tile;

    public override EditorMazeTile Tile { get => _tile; set => _tile = value; }

    public EditorMazeTileAttributePlacer(EditorMazeTile tile)
    {
        Tile = tile;
    }

    public override ITileAttribute InstantiateTileAttributeGO<U>()
    {
        GameObject tileAttributeGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileAttributePrefab<U>(), Tile.transform);
        return tileAttributeGO.GetComponent<U>();
    }

    public void CreatePlayerExit(ObstacleType obstacleType)
    {
        if (_tile.TileMainMaterial.GetType() != typeof(GroundMainMaterial))
        {
            return;
        }
        
        TileConnectionScoreInfo obstacleConnectionScore = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(Tile, obstacleType);
        Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScore} for location {Tile.GridLocation.X}, {Tile.GridLocation.Y}");

        PlayerExit playerExit = (PlayerExit)InstantiateTileAttributeGO<PlayerExit>();
        playerExit.WithObstacleType(obstacleType);
        playerExit.WithConnectionScoreInfo(obstacleConnectionScore);

        Tile.Walkable = false;
        Tile.TryMakeMarkable(false);

        Logger.Log("Add player exit to maze tile attribute list");
        Tile.AddAttribute(playerExit);

        // after adding obstacle to this tile, update connections of neighbours
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            if (!neighbour.Value) continue;

            TileObstacle tileObstacleOnNeighbour = neighbour.Value.TryGetTileObstacle();

            if (tileObstacleOnNeighbour == null) continue;
            Logger.Log($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo obstacleConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(neighbour.Value, obstacleType);
            Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScoreOnNeighbour} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tileObstacleOnNeighbour.WithConnectionScoreInfo(obstacleConnectionScoreOnNeighbour);
        }
    }

    public void CreateTileObstacle(ObstacleType obstacleType)
    {
        // check connections of this tile
        TileConnectionScoreInfo obstacleConnectionScore = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(Tile, obstacleType);
        Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScore} for location {Tile.GridLocation.X}, {Tile.GridLocation.Y}");

        TileObstacle tileObstacle = (TileObstacle)InstantiateTileAttributeGO<TileObstacle>();
        tileObstacle.WithObstacleType(obstacleType);
        tileObstacle.WithConnectionScoreInfo(obstacleConnectionScore);

        Tile.Walkable = false;
        Tile.TryMakeMarkable(false);
        Tile.AddAttribute(tileObstacle);

        // If we now have obstacle connections on all sides, remove the backgrounds
        if (obstacleConnectionScore.RawConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(Tile);
            tileBackgroundRemover.RemoveBackground<MazeTileBaseGround>();
        }

        // after adding obstacle to this tile, update connections of neighbours
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            if (!neighbour.Value) continue;
            
            TileObstacle tileObstacleOnNeighbour = neighbour.Value.TryGetTileObstacle();

            if (tileObstacleOnNeighbour == null) continue;

            Logger.Log($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");
            TileConnectionScoreInfo obstacleConnectionScoreOnNeighbour = NeighbourTileCalculator.MapNeighbourObstaclesOfTile(neighbour.Value, obstacleType);
            Logger.Log($"We calculated an obstacle connection type score of {obstacleConnectionScoreOnNeighbour.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tileObstacleOnNeighbour.WithConnectionScoreInfo(obstacleConnectionScoreOnNeighbour);

            // If we now have obstacle connections on all sides, remove the backgrounds
            if (obstacleConnectionScoreOnNeighbour.RawConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
            {
                Logger.Log($"Remove the background on tile {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");
                MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(neighbour.Value as EditorMazeTile);

                tileBackgroundRemover.RemoveBackground<MazeTileBaseGround>();
            }
        }
    }

    public void PlacePlayerSpawnpoint()
    {
        PlayerSpawnpoint playerSpawnpoint = (PlayerSpawnpoint)InstantiateTileAttributeGO<PlayerSpawnpoint>();

        Tile.Walkable = true;
        Tile.TryMakeMarkable(false);

        Tile.AddAttribute(playerSpawnpoint);
    }

}