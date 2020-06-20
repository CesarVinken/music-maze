using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeLevel
{
    public List<Tile> Tiles = new List<Tile>();

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

        //TODO mazeName should not be hardcoded but generic
        GameObject mazeContainer = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Level/Blank6x6"));

        if (mazeContainer == null)
            Logger.Error("Could not find prefab for level {0}", mazeName);

        mazeContainer.transform.SetParent(GameManager.Instance.GridGO.transform);
        mazeContainer.SetActive(true);

        TilesContainer.SetInstance(mazeContainer.GetComponent<TilesContainer>());
        Tiles = TilesContainer.Instance.Tiles;

        // TODO set character start location through editor and load in to level
        CharacterStartLocation startLocation = new CharacterStartLocation(
            new GridLocation(1, 1),
            new Character(CharacterType.Bard)
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
}

public struct CharacterStartLocation
{
    public GridLocation GridLocation;
    public Character Character;

    public CharacterStartLocation(GridLocation gridLocation, Character character)
    {
        GridLocation = gridLocation;
        Character = character;
    }
}