using System.Collections.Generic;
using UnityEngine;

public class MazeLevel
{
    public List<Tile> Tiles = new List<Tile>();
    public List<Tile> UnwalkableTiles = new List<Tile>();
    public List<MazeExit> MazeExits = new List<MazeExit>();

    public int NumberOfUnmarkedTiles = -1;

    public Dictionary<GridLocation, Tile> TilesByLocation = new Dictionary<GridLocation, Tile>();

    public MazeName MazeName;

    public GridLocation LevelBounds = new GridLocation(0, 0);


    public MazeLevel()
    {

    }

    public MazeLevel(MazeName mazeName)
    {
        MazeName = mazeName;

        if(TilesContainer.Instance != null)
        {
            GameObject.Destroy(TilesContainer.Instance.gameObject);
            TilesContainer.Instance = null;
        }

        GameObject mazeContainer = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Level/" + MazeName));

        if (mazeContainer == null)
            Logger.Error("Could not find prefab for level {0}", mazeName);

        mazeContainer.transform.SetParent(GameManager.Instance.GridGO.transform);
        mazeContainer.SetActive(true);

        Tiles = TilesContainer.Instance.Tiles;

        MazeLevelManager.Instance.ValidateSpawnpoints(); // make sure we have exactly 2 player spawnpoints

        OrderTilesByLocation();
    }

    public void OrderTilesByLocation()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            Tile tile = Tiles[i];
            GridLocation gridLocation = new GridLocation(tile.GridLocation.X, tile.GridLocation.Y);
            TilesByLocation.Add(gridLocation, tile);

            GridLocation furthestBounds = LevelBounds;
            if (gridLocation.X > furthestBounds.X) LevelBounds.X = gridLocation.X;
            if (gridLocation.Y > furthestBounds.Y) LevelBounds.Y = gridLocation.Y;
        }
    }

    public static MazeLevel Create(MazeName mazeName = MazeName.Blank6x6)
    {
        Logger.Log(Logger.Initialisation, "Set up new Maze Level '<color=" + ConsoleConfiguration.HighlightColour + ">" + mazeName + "</color>'");
        return new MazeLevel(mazeName);
    }

    public void AddUnwalkableTile(Tile tile)
    {
        //Logger.Log("{0},{1} is an unwalkable tile", tile.transform.position.x, tile.transform.position.y);
        UnwalkableTiles.Add(tile);
    }
}

public struct CharacterStartLocation
{
    public GridLocation GridLocation;
    public CharacterBlueprint Character;

    public CharacterStartLocation(GridLocation gridLocation, CharacterBlueprint character)
    {
        GridLocation = gridLocation;
        Character = character;
    }
}