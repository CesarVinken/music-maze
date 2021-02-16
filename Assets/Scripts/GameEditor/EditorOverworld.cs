using System.Collections.Generic;
using UnityEngine;

public class EditorOverworld : Overworld, IEditorLevel
{
    private Dictionary<GridLocation, Tile> _tilesByLocation = new Dictionary<GridLocation, Tile>();

    public List<EditorOverworldTile> Tiles = new List<EditorOverworldTile>();

    public Dictionary<GridLocation, Tile> TilesByLocation { get => _tilesByLocation; set => _tilesByLocation = value; }

    public EditorOverworld()
    {

    }

    public EditorOverworld(OverworldData overworldData)
    {
        GameManager.Instance.CurrentEditorLevel = this;

        OverworldName = overworldData.Name;

        if (TilesContainer.Instance != null)
        {
            GameObject.Destroy(TilesContainer.Instance.gameObject);
            TilesContainer.Instance = null;
        }

        _overworldContainer = new GameObject(OverworldName);
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
        Logger.Log("To be implemented");
    }
}
