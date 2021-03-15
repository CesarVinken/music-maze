using System;
using System.Collections.Generic;
using UnityEngine;

public class InGameOverworld : Overworld, IInGameLevel
{
    private List<InGameOverworldTile> _tiles = new List<InGameOverworldTile>();
    public new List<InGameOverworldTile> Tiles { get => _tiles; set => _tiles = value; }

    public InGameOverworld()
    {

    }

    public InGameOverworld(OverworldData overworldData)
    {
        Name = overworldData.Name;
        GameManager.Instance.CurrentGameLevel = this;

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
    }

    public static InGameOverworld Create(OverworldData overworldData)
    {
        Logger.Log(Logger.Initialisation, $"Set up new Maze Level '<color={ConsoleConfiguration.HighlightColour}>{overworldData.Name}</color>'");
        return new InGameOverworld(overworldData);
    }

    public void BuildTiles(OverworldData overworldData)
    {
        Dictionary<InGameOverworldTile, List<SerialisableGridLocation>> TileTransformationGridLocationByTile = new Dictionary<InGameOverworldTile, List<SerialisableGridLocation>>();

        for (int i = 0; i < overworldData.Tiles.Count; i++)
        {
            SerialisableTile serialisableTile = overworldData.Tiles[i];
            GameObject tileGO = GameObject.Instantiate(OverworldManager.Instance.InGameTilePrefab, _overworldContainer.transform);

            InGameOverworldTile tile = tileGO.GetComponent<InGameOverworldTile>();

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

            TileTransformationGridLocationByTile.Add(tile, serialisableTile.TilesToTransform);
        }

        for (int k = 0; k < Tiles.Count; k++)
        {
            InGameOverworldTile tile = Tiles[k];
            tile.AddNeighbours(this);
        }
    }

    public void AddTileAttributes(SerialisableTile serialisableTile, InGameOverworldTile tile)
    {
        InGameOverworldTileAttributePlacer tileAttributePlacer = new InGameOverworldTileAttributePlacer(tile);

        foreach (SerialisableTileAttribute serialisableTileAttribute in serialisableTile.TileAttributes)
        {
            Type type = Type.GetType(serialisableTileAttribute.AttributeType);

            if (type.Equals(typeof(SerialisableMazeLevelEntryAttribute)))
            {
                SerialisableMazeLevelEntryAttribute serialisableMazeLevelEntryAttribute = (SerialisableMazeLevelEntryAttribute)JsonUtility.FromJson(serialisableTileAttribute.SerialisedData, type);
                MazeLevelEntry mazeLevelEntry = tileAttributePlacer.PlaceMazeLevelEntry(serialisableMazeLevelEntryAttribute.MazeLevelName);
                MazeEntries.Add(mazeLevelEntry);
            }
            else if (type.Equals(typeof(SerialisablePlayerSpawnpointAttribute)))
            {
                tileAttributePlacer.PlacePlayerSpawnpoint();
            }
            else
            {
                Logger.Error($"Unknown tile attribute of type {type}");
            }
        }
    }

    public void AddBackgroundSprites(SerialisableTile serialisableTile, InGameOverworldTile tile)
    {
        InGameOverworldTileBackgroundPlacer tileBackgroundPlacer = new InGameOverworldTileBackgroundPlacer(tile);

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
                tileBackgroundPlacer.PlaceBackground<OverworldTileBaseGround>();
            }
            else
            {
                Logger.Error($"Unknown background of type {type}");
            }
        }
    }
}
