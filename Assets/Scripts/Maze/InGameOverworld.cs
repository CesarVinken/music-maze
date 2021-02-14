using System.Collections.Generic;
using UnityEngine;

public class InGameOverworld : Overworld
{
    public List<InGameTile> Tiles = new List<InGameTile>();
    public Dictionary<GridLocation, InGameTile> TilesByLocation = new Dictionary<GridLocation, InGameTile>();

    public InGameOverworld()
    {

    }

    public InGameOverworld(OverworldData overworldData)
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

    public static InGameOverworld Create(OverworldData overworldData)
    {
        Logger.Log(Logger.Initialisation, $"Set up new Maze Level '<color={ConsoleConfiguration.HighlightColour}>{overworldData.Name}</color>'");
        return new InGameOverworld(overworldData);
    }

    public void BuildTiles(OverworldData overworldData)
    {
        Logger.Log("TODO: implement BuildTiles function");
        //Dictionary<InGameTile, List<SerialisableGridLocation>> TileTransformationGridLocationByTile = new Dictionary<InGameTile, List<SerialisableGridLocation>>();

        //for (int i = 0; i < mazeLevelData.Tiles.Count; i++)
        //{
        //    SerialisableTile serialisableTile = mazeLevelData.Tiles[i];
        //    GameObject tileGO = GameObject.Instantiate(MazeLevelManager.Instance.InGameTilePrefab, _mazeContainer.transform);

        //    InGameTile tile = tileGO.GetComponent<InGameTile>();
        //    tileGO.name = "serialisableTile" + serialisableTile.GridLocation.X + ", " + serialisableTile.GridLocation.Y;

        //    tile.SetGridLocation(serialisableTile.GridLocation.X, serialisableTile.GridLocation.Y);
        //    tile.SetId(serialisableTile.Id);

        //    tileGO.name = "Tile" + tile.GridLocation.X + ", " + tile.GridLocation.Y;
        //    tileGO.transform.position = GridLocation.GridToVector(tile.GridLocation);

        //    Tiles.Add(tile);

        //    AddTileAttributes(serialisableTile, tile);
        //    AddBackgroundSprites(serialisableTile, tile);

        //    TilesByLocation.Add(tile.GridLocation, tile);

        //    GridLocation furthestBounds = LevelBounds;
        //    if (tile.GridLocation.X > furthestBounds.X) LevelBounds.X = tile.GridLocation.X;
        //    if (tile.GridLocation.Y > furthestBounds.Y) LevelBounds.Y = tile.GridLocation.Y;

        //    TileTransformationGridLocationByTile.Add(tile, serialisableTile.TilesToTransform);
        //}

        //foreach (KeyValuePair<InGameTile, List<SerialisableGridLocation>> item in TileTransformationGridLocationByTile)
        //{
        //    List<InGameTile> tilesToTransform = new List<InGameTile>();

        //    for (int i = 0; i < item.Value.Count; i++)
        //    {
        //        for (int j = 0; j < Tiles.Count; j++)
        //        {
        //            InGameTile tile = Tiles[j];
        //            if (item.Value[i].X == tile.GridLocation.X && item.Value[i].Y == tile.GridLocation.Y)
        //            {
        //                tilesToTransform.Add(tile);
        //                break;
        //            }
        //        }
        //    }

        //    item.Key.AddTilesToTransform(tilesToTransform);
        //}

        //for (int k = 0; k < Tiles.Count; k++)
        //{
        //    InGameTile tile = Tiles[k];
        //    tile.AddNeighbours(this);
        //}
    }
}
