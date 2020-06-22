using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeLevel
{
    public List<Tile> Tiles = new List<Tile>();
    public List<Tile> UnwalkableTiles = new List<Tile>();

    public List<CharacterStartLocation> CharacterStartLocations = new List<CharacterStartLocation>();

    private MazeName _mazeName;

    public MazeLevel(MazeName mazeName)
    {
        _mazeName = mazeName;

        if(TilesContainer.Instance != null)
        {
            GameObject.Destroy(TilesContainer.Instance.gameObject);
            TilesContainer.Instance = null;
        }

        GameObject mazeContainer = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Level/" + _mazeName));

        if (mazeContainer == null)
            Logger.Error("Could not find prefab for level {0}", mazeName);

        mazeContainer.transform.SetParent(GameManager.Instance.GridGO.transform);
        mazeContainer.SetActive(true);

        TilesContainer.SetInstance(mazeContainer.GetComponent<TilesContainer>());
        Tiles = TilesContainer.Instance.Tiles;
     
        // TODO set character start location through editor and load in to level
        CharacterStartLocation startLocation = new CharacterStartLocation(
            new GridLocation(0, 0),
            new CharacterBlueprint(CharacterType.Bard)
            );

        CharacterStartLocations.Add(startLocation);

        int locationsForPlayers = CharacterStartLocations.Where(location => location.Character.IsPlayable).ToArray().Count();
        if (locationsForPlayers == 0)
        {
            Logger.Warning(Logger.Initialisation, "The level {0} does not have any player starting positions set up while it needs 2.", _mazeName);
        }
        else if (locationsForPlayers == 1)
        {
            Logger.Warning(Logger.Initialisation, "The level {0} has only 1 player starting position set up while it needs 2.", _mazeName);
        }
        else if (locationsForPlayers > 2)
        {
            Logger.Warning(Logger.Initialisation, "The level {0} has {1} player starting positions set up. There should be 2 positions.", _mazeName, locationsForPlayers);
        }
    }

    public static MazeLevel Create(MazeName mazeName = MazeName.Blank6x6)
    {
        Logger.Log(Logger.Initialisation, "Set up new Maze Level '<color=" + ConsoleConfiguration.HighlightColour + ">" + mazeName + "</color>'");
        return new MazeLevel(mazeName);
    }

    public void AddUnwalkableTile(Tile tile)
    {
        Logger.Log("{0},{1} is an unwalkable tile", tile.transform.position.x, tile.transform.position.y);
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