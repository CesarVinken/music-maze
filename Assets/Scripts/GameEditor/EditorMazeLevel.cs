using System.Collections.Generic;
using UnityEngine;

public class EditorMazeLevel : MazeLevel
{
    public List<EditorTile> Tiles = new List<EditorTile>();
    public Dictionary<GridLocation, EditorTile> TilesByLocation = new Dictionary<GridLocation, EditorTile>();

    public EditorMazeLevel()
    {

    }

    public EditorMazeLevel(MazeLevelData mazeLevelData)
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

    public static EditorMazeLevel Create(MazeLevelData mazeLevelData)
    {
        Logger.Log(Logger.Initialisation, $"Set up new Maze Level '<color={ConsoleConfiguration.HighlightColour}>{mazeLevelData.Name}</color>'");
        return new EditorMazeLevel(mazeLevelData);
    }

    public void BuildTiles(MazeLevelData mazeLevelData)
    {
        Dictionary<SerialisableGridLocation, List<EditorTile>> TileTransformationTriggererByGridLocation = new Dictionary<SerialisableGridLocation, List<EditorTile>>();

        for (int i = 0; i < mazeLevelData.Tiles.Count; i++)
        {
            SerialisableTile serialisableTile = mazeLevelData.Tiles[i];
            GameObject tileGO = GameObject.Instantiate(MazeLevelManager.Instance.EditorTilePrefab, _mazeContainer.transform);

            EditorTile tile = tileGO.GetComponent<EditorTile>();
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

            if (serialisableTile.TilesToTransform != null)
            {
                for (int j = 0; j < serialisableTile.TilesToTransform.Count; j++)
                {
                    if (TileTransformationTriggererByGridLocation.ContainsKey(serialisableTile.TilesToTransform[j]))
                    {
                        List<EditorTile> transformationTriggerers = TileTransformationTriggererByGridLocation[serialisableTile.TilesToTransform[j]];
                        transformationTriggerers.Add(tile);
                    }
                    else
                    {
                        List<EditorTile> transformationTriggerers = new List<EditorTile>();
                        transformationTriggerers.Add(tile);
                        TileTransformationTriggererByGridLocation.Add(serialisableTile.TilesToTransform[j], transformationTriggerers);
                    }
                }
            }
        }

        foreach (KeyValuePair<SerialisableGridLocation, List<EditorTile>> item in TileTransformationTriggererByGridLocation)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                EditorTile tile = Tiles[i];
                if(item.Key.X == tile.GridLocation.X && item.Key.Y == tile.GridLocation.Y)
                {
                    tile.TransformationTriggerers = item.Value;
                }
            }
        }

        for (int k = 0; k < Tiles.Count; k++)
        {
            EditorTile tile = Tiles[k];
            tile.AddNeighbours(this);
        }
    }

    public void AddTileAttributes(SerialisableTile serialisableTile, EditorTile tile)
    {
        EditorTileAttributePlacer tileAttributePlacer = new EditorTileAttributePlacer(tile);

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

    public void AddBackgroundSprites(SerialisableTile serialisableTile, EditorTile tile)
    {
        EditorTileBackgroundPlacer tileBackgroundPlacer = new EditorTileBackgroundPlacer(tile);

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

    public List<EditorTile> FindTilesToTransform(EditorTile transformationTriggererTile)
    {
        List<EditorTile> tilesToTransform = new List<EditorTile>();

        for (int i = 0; i < Tiles.Count; i++)
        {
            if (Tiles[i].TransformationTriggerers.Contains(transformationTriggererTile))
            {
                tilesToTransform.Add(Tiles[i]);
            }
        }

        return tilesToTransform;
    }
}
