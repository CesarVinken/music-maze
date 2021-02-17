﻿using System.Collections.Generic;
using UnityEngine;

public interface IEditorLevel
{
    Dictionary<GridLocation, Tile> TilesByLocation { get; set; }
     GridLocation LevelBounds { get; set; }
}

public class EditorMazeLevel : MazeLevel, IEditorLevel
{
    private Dictionary<GridLocation, Tile> _tilesByLocation = new Dictionary<GridLocation, Tile>();

    public List<EditorMazeTile> Tiles = new List<EditorMazeTile>();

    public Dictionary<GridLocation, Tile> TilesByLocation { get => _tilesByLocation; set => _tilesByLocation = value; }


    public EditorMazeLevel()
    {

    }

    public EditorMazeLevel(MazeLevelData mazeLevelData)
    {
        GameManager.Instance.CurrentEditorLevel = this;
        
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

    public static EditorMazeLevel Create(MazeLevelData mazeLevelData)
    {
        Logger.Log(Logger.Initialisation, $"Set up new Maze Level '<color={ConsoleConfiguration.HighlightColour}>{mazeLevelData.Name}</color>'");
        return new EditorMazeLevel(mazeLevelData);
    }

    public void BuildTiles(MazeLevelData mazeLevelData)
    {
        Dictionary<SerialisableGridLocation, List<EditorMazeTile>> TileTransformationTriggererByGridLocation = new Dictionary<SerialisableGridLocation, List<EditorMazeTile>>();

        for (int i = 0; i < mazeLevelData.Tiles.Count; i++)
        {
            SerialisableTile serialisableTile = mazeLevelData.Tiles[i];
            GameObject tileGO = GameObject.Instantiate(MazeLevelManager.Instance.EditorTilePrefab, _mazeContainer.transform);

            EditorMazeTile tile = tileGO.GetComponent<EditorMazeTile>();
            tile.SetGridLocation(serialisableTile.GridLocation.X, serialisableTile.GridLocation.Y);
            tile.SetId(serialisableTile.Id);
            

            tileGO.name = "Tile" + tile.GridLocation.X + ", " + tile.GridLocation.Y;
            tileGO.transform.position = GridLocation.GridToVector(tile.GridLocation);

            Tiles.Add(tile);

            AddTileAttributes(serialisableTile, tile);
            AddBackgroundSprites(serialisableTile, tile);

            TilesByLocation.Add(tile.GridLocation, tile);

            GridLocation furthestBounds = LevelBounds;
            if (tile.GridLocation.X > furthestBounds.X) _levelBounds.X = tile.GridLocation.X;
            if (tile.GridLocation.Y > furthestBounds.Y) _levelBounds.Y = tile.GridLocation.Y;

            if (serialisableTile.TilesToTransform != null)
            {
                for (int j = 0; j < serialisableTile.TilesToTransform.Count; j++)
                {
                    if (TileTransformationTriggererByGridLocation.ContainsKey(serialisableTile.TilesToTransform[j]))
                    {
                        List<EditorMazeTile> transformationTriggerers = TileTransformationTriggererByGridLocation[serialisableTile.TilesToTransform[j]];
                        transformationTriggerers.Add(tile);
                    }
                    else
                    {
                        List<EditorMazeTile> transformationTriggerers = new List<EditorMazeTile>();
                        transformationTriggerers.Add(tile);
                        TileTransformationTriggererByGridLocation.Add(serialisableTile.TilesToTransform[j], transformationTriggerers);
                    }
                }
            }
        }

        foreach (KeyValuePair<SerialisableGridLocation, List<EditorMazeTile>> item in TileTransformationTriggererByGridLocation)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                EditorMazeTile tile = Tiles[i];
                if(item.Key.X == tile.GridLocation.X && item.Key.Y == tile.GridLocation.Y)
                {
                    tile.BeautificationTriggerers = item.Value;
                }
            }
        }

        for (int k = 0; k < Tiles.Count; k++)
        {
            EditorMazeTile tile = Tiles[k];
            tile.AddNeighbours(this);
        }
    }

    public void AddTileAttributes(SerialisableTile serialisableTile, EditorMazeTile tile)
    {
        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);

        foreach (SerialisableTileAttribute serialisableTileAttribute in serialisableTile.TileAttributes)
        {
            int tileAttributeId = serialisableTileAttribute.TileAttributeId;
            if (tileAttributeId == SerialisableTileAttribute.ObstacleAttributeCode)
            {
                tileAttributePlacer.PlaceTileObstacle(ObstacleType.Bush, new TileConnectionScoreInfo(serialisableTileAttribute.ObstacleConnectionScore, serialisableTileAttribute.SpriteNumber)); //TODO, find a way to use polymorphism so we can cast as SerialisableTileObstacleAttribute instead of a general 
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

    public void AddBackgroundSprites(SerialisableTile serialisableTile, EditorMazeTile tile)
    {
        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);

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

    public List<EditorMazeTile> FindTilesToTransform(EditorMazeTile transformationTriggererTile)
    {
        List<EditorMazeTile> tilesToTransform = new List<EditorMazeTile>();

        for (int i = 0; i < Tiles.Count; i++)
        {
            if (Tiles[i].BeautificationTriggerers.Contains(transformationTriggererTile))
            {
                tilesToTransform.Add(Tiles[i]);
            }
        }

        return tilesToTransform;
    }
}
