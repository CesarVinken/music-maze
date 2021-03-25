using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorMazeLevel : MazeLevel, IEditorLevel
{
    private List<EditorMazeTile> _tiles = new List<EditorMazeTile>();
    public new List<EditorMazeTile> Tiles { get => _tiles; set => _tiles = value; }

    public EditorMazeLevel()
    {

    }

    public EditorMazeLevel(MazeLevelData mazeLevelData)
    {
        GameManager.Instance.CurrentEditorLevel = this;
        
        Name = mazeLevelData.Name;

        if (TilesContainer.Instance != null)
        {
            GameObject.Destroy(TilesContainer.Instance.gameObject);
            TilesContainer.Instance = null;
        }

        _mazeContainer = new GameObject(Name);
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

            ITileMainMaterial mainMaterial = AddMainMaterial(serialisableTile);
            tile.SetMainMaterial(mainMaterial);

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
            Type type = Type.GetType(serialisableTileAttribute.AttributeType);

            if (type.Equals(typeof(SerialisableTileObstacleAttribute)))
            {
                SerialisableTileObstacleAttribute serialisableTileObstacleAttribute = (SerialisableTileObstacleAttribute)JsonUtility.FromJson(serialisableTileAttribute.SerialisedData, type);
                tileAttributePlacer.PlaceTileObstacle(ObstacleType.Bush, new TileConnectionScoreInfo(serialisableTileObstacleAttribute.ConnectionScore, serialisableTileObstacleAttribute.SpriteNumber));
            }
            else if (type.Equals(typeof(SerialisablePlayerExitAttribute)))
            {
                SerialisablePlayerExitAttribute serialisablePlayerExitAttribute = (SerialisablePlayerExitAttribute)JsonUtility.FromJson(serialisableTileAttribute.SerialisedData, type);
                tileAttributePlacer.PlacePlayerExit(ObstacleType.Bush, new TileConnectionScoreInfo(serialisablePlayerExitAttribute.ConnectionScore, serialisablePlayerExitAttribute.SpriteNumber));
            }
            else if (type.Equals(typeof(SerialisablePlayerSpawnpointAttribute)))
            {
                tileAttributePlacer.PlacePlayerSpawnpoint();
            }
            else if (type.Equals(typeof(SerialisablePlayerOnlyAttribute)))
            {
                tileAttributePlacer.PlacePlayerOnlyAttribute(PlayerOnlyType.Bush);
            }
            else if (type.Equals(typeof(SerialisableEnemySpawnpointAttribute)))
            {
                tileAttributePlacer.PlaceEnemySpawnpoint();
            }
            else
            {
                Logger.Error($"Unknown tile attribute with type {type}");
            }
        }
    }

    public void AddBackgroundSprites(SerialisableTile serialisableTile, EditorMazeTile tile)
    {
        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);

        foreach (SerialisableTileBackground serialisableTileBackground in serialisableTile.TileBackgrounds)
        {
            Type type = Type.GetType(serialisableTileBackground.BackgroundType);

            if (type.Equals(typeof(SerialisableTilePathBackground)))
            {
                SerialisableTilePathBackground serialisableTilePathBackground = (SerialisableTilePathBackground)JsonUtility.FromJson(serialisableTileBackground.SerialisedData, type);
                tileBackgroundPlacer.PlacePath(new MazeLevelDefaultPathType(), new TileConnectionScoreInfo(serialisableTilePathBackground.TileConnectionScore));
            }
            else if (type.Equals(typeof(SerialisableTileBaseGround)))
            {
                SerialisableTileBaseGround serialisableTileBaseGround = (SerialisableTileBaseGround)JsonUtility.FromJson(serialisableTileBackground.SerialisedData, type);
                tileBackgroundPlacer.PlaceGround(new MazeLevelDefaultGroundType(), new TileConnectionScoreInfo(serialisableTileBaseGround.TileConnectionScore));
            }
            else if (type.Equals(typeof(SerialisableTileBaseWater)))
            {
                tileBackgroundPlacer.PlaceCoveringBaseWater();
            }
            else
            {
                Logger.Error($"Unknown TileBackgroundType {serialisableTileBackground.BackgroundType}");
            }
        }
    }

    private ITileMainMaterial AddMainMaterial(SerialisableTile serialisableTile)
    {
        SerialisableTileMainMaterial serialisableMainMaterial = serialisableTile.MainMaterial;
        if (serialisableMainMaterial.MainMaterialType == "GroundMainMaterial")
        {
            return new GroundMainMaterial();
        }
        else if (serialisableMainMaterial.MainMaterialType == "WaterMainMaterial")
        {
            return new WaterMainMaterial();
        }
        else
        {
            Logger.Error($"Unknown SerialisableTileMainMaterial {serialisableMainMaterial.MainMaterialType}");
            return null;
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
