using System.Collections.Generic;
using UnityEngine;

public class MazeLevel
{
    public string MazeName;
    public GridLocation LevelBounds = new GridLocation(0, 0);

    public List<Tile> Tiles = new List<Tile>();
    public List<PlayerExit> MazeExits = new List<PlayerExit>();

    public int NumberOfUnmarkedTiles = -1;

    public Dictionary<GridLocation, Tile> TilesByLocation = new Dictionary<GridLocation, Tile>();

    public List<CharacterSpawnpoint> PlayerCharacterSpawnpoints = new List<CharacterSpawnpoint>();
    public List<CharacterSpawnpoint> EnemyCharacterSpawnpoints = new List<CharacterSpawnpoint>();



    private GameObject _mazeContainer;

    public MazeLevel()
    {

    }
     
    public MazeLevel(MazeLevelData mazeLevelData)
    {
        MazeName = mazeLevelData.Name;

        if (TilesContainer.Instance != null)
        {
            GameObject.Destroy(TilesContainer.Instance.gameObject);
            TilesContainer.Instance = null;
        }

        _mazeContainer = new GameObject(MazeName);
        _mazeContainer.transform.SetParent(GameManager.Instance.GridGO.transform);
        _mazeContainer.AddComponent<TilesContainer>();
        _mazeContainer.SetActive(true);

        BuildTiles(mazeLevelData);
    }
    public static MazeLevel Create(MazeLevelData mazeLevelData)
    {
        Logger.Log(Logger.Initialisation, "Set up new Maze Level '<color=" + ConsoleConfiguration.HighlightColour + ">" + mazeLevelData.Name + "</color>'");
        return new MazeLevel(mazeLevelData);
    }

    public void BuildTiles(MazeLevelData mazeLevelData)
    {
        for (int i = 0; i < mazeLevelData.Tiles.Count; i++)
        {
            SerialisableTile serialisableTile = mazeLevelData.Tiles[i];
            GameObject tileGO = GameObject.Instantiate(MazeLevelManager.Instance.TilePrefab, _mazeContainer.transform);

            Tile tile = tileGO.GetComponent<Tile>();
            tile.SetGridLocation(serialisableTile.GridLocationX, serialisableTile.GridLocationY);
            tile.SetId(serialisableTile.Id);
            tile.SetSprite();

            tileGO.name = "Tile" + tile.GridLocation.X + ", " + tile.GridLocation.Y;
            tileGO.transform.position = GridLocation.GridToVector(tile.GridLocation);

            Tiles.Add(tile);

            tile = AddTileAttributes(serialisableTile, tile);
            //if (!serialisableTile.Walkable)
            //{
            //    tile.PlaceTileObstacle();
            //}

            TilesByLocation.Add(tile.GridLocation, tile);

            GridLocation furthestBounds = LevelBounds;
            if (tile.GridLocation.X > furthestBounds.X) LevelBounds.X = tile.GridLocation.X;
            if (tile.GridLocation.Y > furthestBounds.Y) LevelBounds.Y = tile.GridLocation.Y;
        }
    }

    public Tile AddTileAttributes(SerialisableTile serialisableTile, Tile tile)
    {
        Tile tileWithAttributes = tile;

        foreach (int attributeNumber in serialisableTile.TileAttributes)
        {
            if (attributeNumber == SerialisableTile.ObstacleAttributeCode)
            {
                tile.PlaceTileObstacle();
            }
            if (attributeNumber == SerialisableTile.PlayerExitCode)
            {
                tile.PlacePlayerExit();
            }
            if (attributeNumber == SerialisableTile.PlayerSpawnpointCode)
            {
                tile.PlacePlayerSpawnpoint();
            }
            if (attributeNumber == SerialisableTile.EnemySpawnpointCode)
            {
                tile.PlaceEnemySpawnpoint();
            }

        }

        return tileWithAttributes;
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