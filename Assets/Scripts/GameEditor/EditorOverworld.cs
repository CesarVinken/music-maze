using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorOverworld : Overworld, IEditorLevel
{
    private List<EditorOverworldTile> _tiles = new List<EditorOverworldTile>();
    public new List<EditorOverworldTile> Tiles { get => _tiles; set => _tiles = value; }

    public List<string> MazeLevelNames = new List<string>();
    

    public EditorOverworld()
    {
    }

    public EditorOverworld(OverworldData overworldData)
    {
        GameManager.Instance.CurrentEditorLevel = this;

        Name = overworldData.Name;

        if (TilesContainer.Instance != null)
        {
            GameObject.Destroy(TilesContainer.Instance.gameObject);
            TilesContainer.Instance = null;
        }

        _overworldContainer = new GameObject(Name);
        _overworldContainer.transform.SetParent(GameManager.Instance.GridGO.transform);
        _overworldContainer.transform.position = new Vector3(0, 0, 0);
        _overworldContainer.AddComponent<TilesContainer>();
        _overworldContainer.SetActive(true);

        BuildTiles(overworldData);

        MazeLevelNames = MazeLevelLoader.GetAllPlayableLevelNames();
    }

    public static EditorOverworld Create(OverworldData overworldData)
    {
        Logger.Log(Logger.Initialisation, $"Set up new Overworld '<color={ConsoleConfiguration.HighlightColour}>{overworldData.Name}</color>'");
        return new EditorOverworld(overworldData);
    }

    public void BuildTiles(OverworldData overworldData)
    {
        for (int i = 0; i < overworldData.Tiles.Count; i++)
        {
            SerialisableTile serialisableTile = overworldData.Tiles[i];
            GameObject tileGO = GameObject.Instantiate(OverworldManager.Instance.EditorTilePrefab, _overworldContainer.transform);

            EditorOverworldTile tile = tileGO.GetComponent<EditorOverworldTile>();
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
        }

        for (int k = 0; k < Tiles.Count; k++)
        {
            EditorOverworldTile tile = Tiles[k];
            tile.AddNeighbours(this);
        }
    }

    public void AddBackgroundSprites(SerialisableTile serialisableTile, EditorOverworldTile tile)
    {
        EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(tile);

        foreach (SerialisableTileBackground serialisableTileBackground in serialisableTile.TileBackgrounds)
        {
            Type type = Type.GetType(serialisableTileBackground.BackgroundType);

            if (type.Equals(typeof(SerialisableTilePathBackground)))
            {
                SerialisableTilePathBackground serialisableTilePathBackground = (SerialisableTilePathBackground)JsonUtility.FromJson(serialisableTileBackground.SerialisedData, type);
                tileBackgroundPlacer.PlacePath(new OverworldDefaultPathType(), new TileConnectionScoreInfo(serialisableTilePathBackground.TileConnectionScore));
            }
            else if (type.Equals(typeof(SerialisableTileBaseGround)))
            {
                SerialisableTileBaseGround serialisableTileBaseGround = (SerialisableTileBaseGround)JsonUtility.FromJson(serialisableTileBackground.SerialisedData, type);
                tileBackgroundPlacer.PlaceGround(new OverworldDefaultGroundType(), new TileConnectionScoreInfo(serialisableTileBaseGround.TileConnectionScore));
            }
            else if (type.Equals(typeof(SerialisableTileBaseWater)))
            {
                tileBackgroundPlacer.PlaceBackground<OverworldTileBaseWater>();
            }
            else
            {
                Logger.Error($"Unknown TileBackgroundType {serialisableTileBackground.BackgroundType}");
            }
        }
    }

    public void AddTileAttributes(SerialisableTile serialisableTile, EditorOverworldTile tile)
    {
        EditorOverworldTileAttributePlacer tileAttributePlacer = new EditorOverworldTileAttributePlacer(tile);

        foreach (SerialisableTileAttribute serialisableTileAttribute in serialisableTile.TileAttributes)
        {
            Type type = Type.GetType(serialisableTileAttribute.AttributeType);

            //if (tileAttributeId == SerialisableTileAttribute.ObstacleAttributeCode)
            //{
            //    tileAttributePlacer.PlaceTileObstacle(ObstacleType.Bush, new TileConnectionScoreInfo(serialisableTileAttribute.ObstacleConnectionScore, serialisableTileAttribute.SpriteNumber)); //TODO, find a way to use polymorphism so we can cast as SerialisableTileObstacleAttribute instead of a general 
            //}
            //else if (tileAttributeId == SerialisableTileAttribute.PlayerExitCode)
            //{
            //    tileAttributePlacer.PlacePlayerExit(ObstacleType.Bush, new TileConnectionScoreInfo(serialisableTileAttribute.ObstacleConnectionScore, serialisableTileAttribute.SpriteNumber));
            //}
            if (type.Equals(typeof(SerialisablePlayerSpawnpointAttribute)))
            {
                tileAttributePlacer.PlacePlayerSpawnpoint();
            }
            else if (type.Equals(typeof(SerialisableMazeLevelEntryAttribute)))
            {
               SerialisableMazeLevelEntryAttribute serialisableMazeLevelEntryAttribute = (SerialisableMazeLevelEntryAttribute)JsonUtility.FromJson(serialisableTileAttribute.SerialisedData, type);
               MazeLevelEntry mazeLevelEntry = tileAttributePlacer.PlaceMazeLevelEntry(serialisableMazeLevelEntryAttribute.MazeLevelName);
               MazeEntries.Add(mazeLevelEntry);
            }
            //else if (tileAttributeId == SerialisableTileAttribute.PlayerOnlyAttributeCode)
            //{
            //    tileAttributePlacer.PlacePlayerOnlyAttribute(PlayerOnlyType.Bush);
            //}
            //else if (tileAttributeId == SerialisableTileAttribute.EnemySpawnpointCode)
            //{
            //    tileAttributePlacer.PlaceEnemySpawnpoint();
            //}
            else
            {
                Logger.Error($"Unknown tile attribute of the type {type}");
            }
        }
    }
}
