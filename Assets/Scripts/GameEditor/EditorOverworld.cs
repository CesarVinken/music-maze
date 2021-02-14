using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOverworld : Overworld
{
    public List<EditorTile> Tiles = new List<EditorTile>();
    public Dictionary<GridLocation, EditorTile> TilesByLocation = new Dictionary<GridLocation, EditorTile>();

    public EditorOverworld()
    {

    }

    public EditorOverworld(OverworldData overworldData)
    {
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
        //Dictionary<SerialisableGridLocation, List<EditorTile>> TileTransformationTriggererByGridLocation = new Dictionary<SerialisableGridLocation, List<EditorTile>>();

        for (int i = 0; i < overworldData.Tiles.Count; i++)
        {
            SerialisableTile serialisableTile = overworldData.Tiles[i];
            GameObject tileGO = GameObject.Instantiate(OverworldManager.Instance.EditorTilePrefab, _overworldContainer.transform);

            EditorTile tile = tileGO.GetComponent<EditorTile>();
            tile.SetGridLocation(serialisableTile.GridLocation.X, serialisableTile.GridLocation.Y);
            tile.SetId(serialisableTile.Id);


            tileGO.name = "Tile" + tile.GridLocation.X + ", " + tile.GridLocation.Y;
            tileGO.transform.position = GridLocation.GridToVector(tile.GridLocation);

            Tiles.Add(tile);

            //AddTileAttributes(serialisableTile, tile);
            //AddBackgroundSprites(serialisableTile, tile);

            TilesByLocation.Add(tile.GridLocation, tile);

            GridLocation furthestBounds = OverworldBounds;
            if (tile.GridLocation.X > furthestBounds.X) OverworldBounds.X = tile.GridLocation.X;
            if (tile.GridLocation.Y > furthestBounds.Y) OverworldBounds.Y = tile.GridLocation.Y;
        }
    }
}
