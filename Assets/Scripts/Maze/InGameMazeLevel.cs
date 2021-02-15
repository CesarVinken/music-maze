﻿using System.Collections.Generic;
using UnityEngine;

public class InGameMazeLevel : MazeLevel
{
    public List<InGameTile> Tiles = new List<InGameTile>();
    public Dictionary<GridLocation, InGameTile> TilesByLocation = new Dictionary<GridLocation, InGameTile>();

    public InGameMazeLevel()
    {

    }

    public InGameMazeLevel(MazeLevelData mazeLevelData)
    {
        MazeName = mazeLevelData.Name;

        if (TilesContainer.Instance != null)
        {
            GameObject.Destroy(TilesContainer.Instance.gameObject);
            TilesContainer.Instance = null;
        }

        _mazeContainer = new GameObject(MazeName);
        _mazeContainer.transform.SetParent(GameManager.Instance.GridGO.transform);
        _mazeContainer.transform.position = new Vector3(0, 0, 0);
        _mazeContainer.AddComponent<TilesContainer>();
        _mazeContainer.SetActive(true);

        BuildTiles(mazeLevelData);
    }

    public static InGameMazeLevel Create(MazeLevelData mazeLevelData)
    {
        Logger.Log(Logger.Initialisation, $"Set up new Maze Level '<color={ConsoleConfiguration.HighlightColour}>{mazeLevelData.Name}</color>'");
        return new InGameMazeLevel(mazeLevelData);
    }

    public void BuildTiles(MazeLevelData mazeLevelData)
    {
        Dictionary<InGameTile, List<SerialisableGridLocation>> TileTransformationGridLocationByTile = new Dictionary<InGameTile, List<SerialisableGridLocation>>();

        for (int i = 0; i < mazeLevelData.Tiles.Count; i++)
        {
            SerialisableTile serialisableTile = mazeLevelData.Tiles[i];
            GameObject tileGO = GameObject.Instantiate(MazeLevelManager.Instance.InGameTilePrefab, _mazeContainer.transform);

            InGameTile tile = tileGO.GetComponent<InGameTile>();
            tileGO.name = "serialisableTile" + serialisableTile.GridLocation.X + ", " + serialisableTile.GridLocation.Y;

            tile.SetGridLocation(serialisableTile.GridLocation.X, serialisableTile.GridLocation.Y);
            tile.SetId(serialisableTile.Id);

            tileGO.name = "Tile" + tile.GridLocation.X + ", " + tile.GridLocation.Y;
            tileGO.transform.position = GridLocation.GridToVector(tile.GridLocation);

            Tiles.Add(tile);

            AddTileAttributes(serialisableTile, tile);
            AddBackgroundSprites(serialisableTile, tile);

            TilesByLocation.Add(tile.GridLocation, tile);

            GridLocation furthestBounds = LevelBounds;
            if (tile.GridLocation.X > furthestBounds.X) LevelBounds.X = tile.GridLocation.X;
            if (tile.GridLocation.Y > furthestBounds.Y) LevelBounds.Y = tile.GridLocation.Y;

            TileTransformationGridLocationByTile.Add(tile, serialisableTile.TilesToTransform);
        }

        foreach (KeyValuePair<InGameTile, List<SerialisableGridLocation>> item in TileTransformationGridLocationByTile)
        {
            List<InGameTile> tilesToTransform = new List<InGameTile>();
            
            for (int i = 0; i < item.Value.Count; i++)
            {
                for (int j = 0; j < Tiles.Count; j++)
                {
                    InGameTile tile = Tiles[j];
                    if (item.Value[i].X == tile.GridLocation.X && item.Value[i].Y == tile.GridLocation.Y)
                    {
                        tilesToTransform.Add(tile);
                        break;
                    }
                }
            }

            item.Key.AddTilesToTransform(tilesToTransform);
        }

        for (int k = 0; k < Tiles.Count; k++)
        {
            InGameTile tile = Tiles[k];
            tile.AddNeighbours(this);
        }
    }

    public void AddTileAttributes(SerialisableTile serialisableTile, InGameTile tile)
    {
        InGameTileAttributePlacer tileAttributePlacer = new InGameTileAttributePlacer(tile);

        foreach (SerialisableTileAttribute serialisableTileAttribute in serialisableTile.TileAttributes)
        {
            int tileAttributeId = serialisableTileAttribute.TileAttributeId;
            if (tileAttributeId == SerialisableTileAttribute.ObstacleAttributeCode)
            {
                tileAttributePlacer.PlaceTileObstacle(ObstacleType.Bush, new TileConnectionScoreInfo(serialisableTileAttribute.ObstacleConnectionScore, serialisableTileAttribute.SpriteNumber)); //TODO, find a way to use polymorphism so we can cast as SerialisableTileObstacleAttribute instead of a general SerialisableTileAttribute
            }
            else if (tileAttributeId == SerialisableTileAttribute.PlayerExitCode)
            {
                tileAttributePlacer.PlacePlayerExit(ObstacleType.Bush, new TileConnectionScoreInfo(serialisableTileAttribute.ObstacleConnectionScore, serialisableTileAttribute.SpriteNumber));
            }
            else if (tileAttributeId == SerialisableTileAttribute.PlayerSpawnpointCode)
            {
                tileAttributePlacer.PlacePlayerSpawnpoint();
            }
            else if (tileAttributeId == SerialisableTileAttribute.PlayerOnlyAttributeCode)
            {
                tileAttributePlacer.PlacePlayerOnlyAttribute(PlayerOnlyType.Bush);
            }
            else if (tileAttributeId == SerialisableTileAttribute.EnemySpawnpointCode)
            {
                tileAttributePlacer.PlaceEnemySpawnpoint();
            }
            else
            {
                Logger.Error($"Unknown tile attribute with tileAttributeId {tileAttributeId}");
            }
        }
    }

    public void AddBackgroundSprites(SerialisableTile serialisableTile, InGameTile tile)
    {
        InGameTileBackgroundPlacer tileBackgroundPlacer = new InGameTileBackgroundPlacer(tile);

        foreach (SerialisableTileBackground serialisableTileBackground in serialisableTile.TileBackgrounds)
        {
            if (serialisableTileBackground.TileBackgroundId == SerialisableTileBackground.PathBackgroundCode)
            {
                tileBackgroundPlacer.PlacePath(new MazeLevelDefaultPathType(), new TileConnectionScoreInfo(serialisableTileBackground.TileConnectionScore)); //TODO, fix path type
            }
            else if (serialisableTileBackground.TileBackgroundId == SerialisableTileBackground.BaseBackgroundCode)
            {
                tileBackgroundPlacer.PlaceBaseBackground(new MazeLevelDefaultBaseBackgroundType()); // TODO fix background type
            }
            else
            {
                Logger.Error($"Unknown TileBackgroundId {serialisableTileBackground.TileBackgroundId}");
            }
        }
    }
}