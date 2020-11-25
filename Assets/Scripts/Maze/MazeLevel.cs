using System.Collections.Generic;
using UnityEngine;

public class MazeLevel
{
    public string MazeName;
    public GridLocation LevelBounds = new GridLocation(0, 0);

    public List<Tile> Tiles = new List<Tile>();
    public List<PlayerExit> MazeExits = new List<PlayerExit>();

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
        _mazeContainer.transform.position = new Vector3(0, 0, 0);
        _mazeContainer.AddComponent<TilesContainer>();
        _mazeContainer.SetActive(true);

        BuildTiles(mazeLevelData);
    }

    public static MazeLevel Create(MazeLevelData mazeLevelData)
    {
        Logger.Log(Logger.Initialisation, $"Set up new Maze Level '<color={ConsoleConfiguration.HighlightColour}>{mazeLevelData.Name}</color>'");
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

            tileGO.name = "Tile" + tile.GridLocation.X + ", " + tile.GridLocation.Y;
            tileGO.transform.position = GridLocation.GridToVector(tile.GridLocation);

            Tiles.Add(tile);

            AddTileAttributes(serialisableTile, tile);
            AddBackgroundSprites(serialisableTile, tile);

            TilesByLocation.Add(tile.GridLocation, tile);

            GridLocation furthestBounds = LevelBounds;
            if (tile.GridLocation.X > furthestBounds.X) LevelBounds.X = tile.GridLocation.X;
            if (tile.GridLocation.Y > furthestBounds.Y) LevelBounds.Y = tile.GridLocation.Y;
        }

        for (int j = 0; j < Tiles.Count; j++)
        {
            Tile tile = Tiles[j];
            tile.AddNeighbours(this);
        }
    }

    public void AddTileAttributes(SerialisableTile serialisableTile, Tile tile)
    {
        TileAttributePlacer tileAttributePlacer = new TileAttributePlacer(tile);

        foreach (SerialisableTileAttribute serialisableTileAttribute in serialisableTile.TileAttributes)
        {
            int tileAttributeId = serialisableTileAttribute.TileAttributeId;
            if (tileAttributeId == SerialisableTileAttribute.ObstacleAttributeCode)
            {
                tileAttributePlacer.PlaceTileObstacle(ObstacleType.Wall, serialisableTileAttribute.ObstacleConnectionScore); //TODO, find a way to use polymorphism so we can cast as SerialisableTileObstacleAttribute instead of a general SerialisableTileAttribute
            }
            else if (tileAttributeId == SerialisableTileAttribute.PlayerExitCode)
            {
                tileAttributePlacer.PlacePlayerExit(ObstacleType.Wall, serialisableTileAttribute.ObstacleConnectionScore);
            }
            else if (tileAttributeId == SerialisableTileAttribute.PlayerSpawnpointCode)
            {
                tileAttributePlacer.PlacePlayerSpawnpoint();
            }
            else if (tileAttributeId == SerialisableTileAttribute.EnemySpawnpointCode)
            {
                tileAttributePlacer.PlaceEnemySpawnpoint();
            }
        }
    }

    public void AddBackgroundSprites(SerialisableTile serialisableTile, Tile tile)
    {
        TileBackgroundPlacer tileBackgroundPlacer = new TileBackgroundPlacer(tile);

        foreach (SerialisableTileBackground serialisableTileBackground in serialisableTile.TileBackgrounds)
        {
            if (serialisableTileBackground.TileBackgroundId == SerialisableTileBackground.PathBackgroundCode)
            {
                tileBackgroundPlacer.PlacePath(MazeTilePathType.Default, serialisableTileBackground.PathConnectionScore); //TODO, fi
            }
            else
            {
                Logger.Error($"Unknown TileBackgroundId {serialisableTileBackground.TileBackgroundId}");
            }
        }
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