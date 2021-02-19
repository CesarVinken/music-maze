using System.Collections.Generic;
using UnityEngine;

public class EditorOverworld : Overworld, IEditorLevel
{
    public List<EditorOverworldTile> Tiles = new List<EditorOverworldTile>();

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
            if (serialisableTileBackground.TileBackgroundId == SerialisableTileBackground.PathBackgroundCode)
            {
                tileBackgroundPlacer.PlacePath(new OverworldDefaultPathType(), new TileConnectionScoreInfo(serialisableTileBackground.TileConnectionScore));
            }
            else if (serialisableTileBackground.TileBackgroundId == SerialisableTileBackground.BaseBackgroundCode)
            {
                tileBackgroundPlacer.PlaceBaseBackground(new OverworldDefaultBaseBackgroundType());
            }
            else
            {
                Logger.Error($"Unknown TileBackgroundId {serialisableTileBackground.TileBackgroundId}");
            }
        }
    }

    public void AddTileAttributes(SerialisableTile serialisableTile, EditorOverworldTile tile)
    {
        EditorOverworldTileAttributePlacer tileAttributePlacer = new EditorOverworldTileAttributePlacer(tile);

        foreach (SerialisableTileAttribute serialisableTileAttribute in serialisableTile.TileAttributes)
        {
            int tileAttributeId = serialisableTileAttribute.TileAttributeId;
            //if (tileAttributeId == SerialisableTileAttribute.ObstacleAttributeCode)
            //{
            //    tileAttributePlacer.PlaceTileObstacle(ObstacleType.Bush, new TileConnectionScoreInfo(serialisableTileAttribute.ObstacleConnectionScore, serialisableTileAttribute.SpriteNumber)); //TODO, find a way to use polymorphism so we can cast as SerialisableTileObstacleAttribute instead of a general 
            //}
            //else if (tileAttributeId == SerialisableTileAttribute.PlayerExitCode)
            //{
            //    tileAttributePlacer.PlacePlayerExit(ObstacleType.Bush, new TileConnectionScoreInfo(serialisableTileAttribute.ObstacleConnectionScore, serialisableTileAttribute.SpriteNumber));
            //}
            if (tileAttributeId == SerialisableTileAttribute.PlayerSpawnpointCode)
            {
                tileAttributePlacer.PlacePlayerSpawnpoint();
            }
            else if (tileAttributeId == SerialisableTileAttribute.MazeEntryCode)
            {
                tileAttributePlacer.PlaceMazeEntry();
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
                Logger.Error($"Unknown tile attribute with tileAttributeId {tileAttributeId}");
            }
        }
    }
}
