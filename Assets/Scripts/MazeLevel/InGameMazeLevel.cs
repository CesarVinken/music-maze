using Console;
using DataSerialisation;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InGameMazeLevel : MazeLevel, IInGameLevel
{
    public InGameMazeLevel()
    {

    }

    public InGameMazeLevel(MazeLevelData mazeLevelData)
    {
        Name = mazeLevelData.Name;
        GameManager.Instance.CurrentGameLevel = this;

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

    public static InGameMazeLevel Create(MazeLevelData mazeLevelData)
    {
        Logger.Log(Logger.Initialisation, $"Set up new Maze Level '<color={ConsoleConfiguration.HighlightColour}>{mazeLevelData.Name}</color>'");
        return new InGameMazeLevel(mazeLevelData);
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
        Dictionary<InGameMazeTile, List<SerialisableGridLocation>> TileTransformationGridLocationByTile = new Dictionary<InGameMazeTile, List<SerialisableGridLocation>>();

        for (int i = 0; i < mazeLevelData.Tiles.Count; i++)
        {
            SerialisableTile serialisableTile = mazeLevelData.Tiles[i];
            GameObject tileGO = GameObject.Instantiate(MazeLevelGameplayManager.Instance.InGameTilePrefab, _mazeContainer.transform);

            InGameMazeTile tile = tileGO.GetComponent<InGameMazeTile>();

            tile.SetGridLocation(serialisableTile.GridLocation.X, serialisableTile.GridLocation.Y);
            tile.SetId(serialisableTile.Id);

            tileGO.name = "Tile" + tile.GridLocation.X + ", " + tile.GridLocation.Y;
            tileGO.transform.position = GridLocation.GridToVector(tile.GridLocation);

            Tiles.Add(tile);

            AddBackgroundSprites(serialisableTile, tile);
            AddTileAttributes(serialisableTile, tile);
            AddCornerFillers(serialisableTile, tile);
            AddTileAreas(serialisableTile, tile);

            TilesByLocation.Add(tile.GridLocation, tile);

            GridLocation furthestBounds = LevelBounds;
            if (tile.GridLocation.X > furthestBounds.X) _levelBounds.X = tile.GridLocation.X;
            if (tile.GridLocation.Y > furthestBounds.Y) _levelBounds.Y = tile.GridLocation.Y;

            TileTransformationGridLocationByTile.Add(tile, serialisableTile.TilesToTransform);
        }

        foreach (KeyValuePair<InGameMazeTile, List<SerialisableGridLocation>> item in TileTransformationGridLocationByTile)
        {
            List<InGameMazeTile> tilesToTransform = new List<InGameMazeTile>();
            
            for (int i = 0; i < item.Value.Count; i++)
            {
                for (int j = 0; j < Tiles.Count; j++)
                {
                    InGameMazeTile tile = Tiles[j] as InGameMazeTile;
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
            InGameMazeTile tile = Tiles[k] as InGameMazeTile;
            tile.AddNeighbours(this);
        }

        ConnectBridgeEdgesToTheirBridgePieces();
    }

    private void AddTileAttributes(SerialisableTile serialisableTile, InGameMazeTile tile)
    {
        InGameMazeTileAttributePlacer tileAttributePlacer = new InGameMazeTileAttributePlacer(tile);

        foreach (SerialisableTileAttribute serialisableTileAttribute in serialisableTile.TileAttributes)
        {
            Type type = Type.GetType("DataSerialisation." + serialisableTileAttribute.AttributeType);

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
            else if (type.Equals(typeof(SerialisableSheetmusicAttribute)))
            {
                tileAttributePlacer.PlaceSheetmusic();
            }
            else
            {
                Logger.Error($"Unknown tile attribute of type {type}");
            }
        }
    }

    private void AddBackgroundSprites(SerialisableTile serialisableTile, InGameMazeTile tile)
    {
        InGameMazeTileBackgroundPlacer tileBackgroundPlacer = new InGameMazeTileBackgroundPlacer(tile);

        foreach (SerialisableTileBackground serialisableTileBackground in serialisableTile.TileBackgrounds)
        {
            Type type = Type.GetType("DataSerialisation." + serialisableTileBackground.BackgroundType);

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
                tileBackgroundPlacer.PlaceBackground<MazeTileBaseWater>();
            }
            else
            {
                Logger.Error($"Unknown TileBackgroundId {serialisableTileBackground.TileBackgroundId}");
            }
        }
    }

    private void AddCornerFillers(SerialisableTile serialisableTile, InGameMazeTile tile)
    {
        InGameMazeTileBackgroundPlacer tileBackgroundPlacer = new InGameMazeTileBackgroundPlacer(tile);   // corner filler is also an IBackground

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
            InGameMazeTile tile = Tiles[i] as InGameMazeTile;
            List<BridgeEdge> bridgeEdges =  tile.GetBridgeEdges();
            for (int j = 0; j < bridgeEdges.Count; j++)
            {
                Direction edgeSide = bridgeEdges[j].EdgeSide;
                InGameMazeTile neighbourTile = null;

                switch (edgeSide)
                {
                    case Direction.Up:
                        neighbourTile = tile.Neighbours[Direction.Up] as InGameMazeTile;
                        break;
                    case Direction.Right:
                        neighbourTile = tile.Neighbours[Direction.Right] as InGameMazeTile;
                        break;
                    case Direction.Down:
                        neighbourTile = tile.Neighbours[Direction.Down] as InGameMazeTile;
                        break;
                    case Direction.Left:
                        neighbourTile = tile.Neighbours[Direction.Left] as InGameMazeTile;
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

    private void AddTileAreas(SerialisableTile serialisableTile, MazeTile tile)
    {
        for (int i = 0; i < serialisableTile.TileAreaIds?.Count; i++)
        {
            string tileAreaId = serialisableTile.TileAreaIds[i];

            if (TileAreas.TryGetValue(tileAreaId, out TileArea tileArea))
            {
                tile.AddTileArea(tileArea);
            }
        }
    }
}