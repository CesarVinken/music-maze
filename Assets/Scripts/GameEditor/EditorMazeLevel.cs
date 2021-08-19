using Console;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorMazeLevel : MazeLevel, IEditorLevel
{
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

        InitialiseEditorTileAreas(mazeLevelData);
        BuildTiles(mazeLevelData);
    }

    public static EditorMazeLevel Create(MazeLevelData mazeLevelData)
    {
        Logger.Log(Logger.Initialisation, $"Set up new Maze Level '<color={ConsoleConfiguration.HighlightColour}>{mazeLevelData.Name}</color>'");
        return new EditorMazeLevel(mazeLevelData);
    }

    protected override void InitialiseEditorTileAreas(MazeLevelData mazeLevelData)
    {
        for (int i = 0; i < mazeLevelData.TileAreas.Count; i++)
        {
            SerialisableTileArea serialisableTileArea = mazeLevelData.TileAreas[i];
            TileArea newTileArea = new TileArea(serialisableTileArea);
            TileAreas.Add(newTileArea.Id, newTileArea);
        }
    }

    public override void BuildTiles(MazeLevelData mazeLevelData)
    {
        Dictionary<SerialisableGridLocation, List<EditorMazeTile>> TileTransformationTriggererByGridLocation = new Dictionary<SerialisableGridLocation, List<EditorMazeTile>>();

        for (int i = 0; i < mazeLevelData.Tiles.Count; i++)
        {
            SerialisableTile serialisableTile = mazeLevelData.Tiles[i];
            GameObject tileGO = GameObject.Instantiate(MazeLevelGameplayManager.Instance.EditorTilePrefab, _mazeContainer.transform);

            EditorMazeTile tile = tileGO.GetComponent<EditorMazeTile>();
            tile.SetGridLocation(serialisableTile.GridLocation.X, serialisableTile.GridLocation.Y);
            tile.SetId(serialisableTile.Id);
            

            tileGO.name = "Tile" + tile.GridLocation.X + ", " + tile.GridLocation.Y;
            tileGO.transform.position = GridLocation.GridToVector(tile.GridLocation);

            Tiles.Add(tile);

            AddTileAttributes(serialisableTile, tile);
            AddBackgroundSprites(serialisableTile, tile);
            AddCornerFillers(serialisableTile, tile);
            AddTileAreas(serialisableTile, tile);

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
                EditorMazeTile tile = Tiles[i] as EditorMazeTile;
                if(item.Key.X == tile.GridLocation.X && item.Key.Y == tile.GridLocation.Y)
                {
                    tile.BeautificationTriggerers = item.Value;
                }
            }
        }

        for (int k = 0; k < Tiles.Count; k++)
        {
            EditorMazeTile tile = Tiles[k] as EditorMazeTile;
            tile.AddNeighbours(this);
        }

        ConnectBridgeEdgesToTheirBridgePieces();
    }

    private void AddTileAttributes(SerialisableTile serialisableTile, EditorMazeTile tile)
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
                SerialisableEnemySpawnpointAttribute serialisableEnemySpawnpointAttribute = (SerialisableEnemySpawnpointAttribute)JsonUtility.FromJson(serialisableTileAttribute.SerialisedData, type);
                tileAttributePlacer.PlaceEnemySpawnpoint(serialisableEnemySpawnpointAttribute.TileAreaIds, TileAreas);
            }
            else if (type.Equals(typeof(SerialisableBridgePieceAttribute)))
            {
                SerialisableBridgePieceAttribute serialisableBridgePieceAttribute = (SerialisableBridgePieceAttribute)JsonUtility.FromJson(serialisableTileAttribute.SerialisedData, type);
                if (Enum.TryParse(serialisableBridgePieceAttribute.BridgePieceDirection, out BridgePieceDirection bridgePieceDirection))
                {
                    tileAttributePlacer.PlaceBridgePiece(BridgeType.Wooden, bridgePieceDirection);
                }
                else
                {
                    Logger.Error($"Could not parse the BridgePieceDirection value{serialisableBridgePieceAttribute.BridgePieceDirection}");
                }
            }
            else if (type.Equals(typeof(SerialisableBridgeEdgeAttribute)))
            {
                SerialisableBridgeEdgeAttribute serialisableBridgeEdgeAttribute = (SerialisableBridgeEdgeAttribute)JsonUtility.FromJson(serialisableTileAttribute.SerialisedData, type);
                if (Enum.TryParse(serialisableBridgeEdgeAttribute.BridgeEdgeSide, out Direction bridgeEdgeSide))
                {
                    tileAttributePlacer.PlaceBridgeEdge(BridgeType.Wooden, bridgeEdgeSide);
                }
                else
                {
                    Logger.Error($"Could not parse the BridgeEdgeSide value{serialisableBridgeEdgeAttribute.BridgeEdgeSide}");
                }
            }
            else if (type.Equals(typeof(SerialisableMusicInstrumentCaseAttribute)))
            {
                tileAttributePlacer.PlaceMusicInstrumentCase();
            }
            else
            {
                Logger.Error($"Unknown tile attribute with type {type}");
            }
        }
    }

    private void AddBackgroundSprites(SerialisableTile serialisableTile, EditorMazeTile tile)
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
            EditorMazeTile tile = Tiles[i] as EditorMazeTile;
            if (tile.BeautificationTriggerers.Contains(transformationTriggererTile))
            {
                tilesToTransform.Add(tile);
            }
        }

        return tilesToTransform;
    }

    private void AddCornerFillers(SerialisableTile serialisableTile, EditorMazeTile tile)
    {
        EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(tile);   // corner filler is also an IBackground

        foreach (SerialisableTileCornerFiller serialisableTileCornerFiller in serialisableTile.TileCornerFillers)
        {
            if (Enum.TryParse(serialisableTileCornerFiller.TileCorner, out TileCorner tileCorner))
            {
                tileBackgroundPlacer.PlaceCornerFiler(tileCorner);
            }
            else
            {
                Logger.Error($"Could not parse the TileCorner value{serialisableTileCornerFiller.TileCorner}");
            }
        }
    }

    private void ConnectBridgeEdgesToTheirBridgePieces()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            EditorMazeTile tile = Tiles[i] as EditorMazeTile;
            List<BridgeEdge> bridgeEdges = tile.GetBridgeEdges();
            for (int j = 0; j < bridgeEdges.Count; j++)
            {
                Direction edgeSide = bridgeEdges[j].EdgeSide;
                EditorMazeTile neighbourTile = null;

                switch (edgeSide)
                {
                    case Direction.Up:
                        neighbourTile = tile.Neighbours[ObjectDirection.Up] as EditorMazeTile;
                        break;
                    case Direction.Right:
                        neighbourTile = tile.Neighbours[ObjectDirection.Right] as EditorMazeTile;
                        break;
                    case Direction.Down:
                        neighbourTile = tile.Neighbours[ObjectDirection.Down] as EditorMazeTile;
                        break;
                    case Direction.Left:
                        neighbourTile = tile.Neighbours[ObjectDirection.Left] as EditorMazeTile;
                        break;
                    default:
                        break;
                }

                if (neighbourTile)
                {
                    BridgePiece bridgePiece = neighbourTile.TryGetAttribute<BridgePiece>();
                    if (bridgePiece == null)
                    {
                        Logger.Error($"Expected but could not find bridge piece at {neighbourTile.GridLocation.X}, {neighbourTile.GridLocation.Y}.");
                        return;
                    }
                    bridgeEdges[j].WithBridgePieceConnection(bridgePiece);
                }
            }
        }
    }

    private void AddTileAreas(SerialisableTile serialisableTile, EditorMazeTile tile)
    {
        for (int i = 0; i < serialisableTile.TileAreaIds?.Count; i++)
        {
            string tileAreaId = serialisableTile.TileAreaIds[i];

            if(TileAreas.TryGetValue(tileAreaId, out TileArea tileArea))
            {
                tile.AddTileArea(tileArea);
            }
        }
    }
}
