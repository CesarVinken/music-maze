using System.Collections.Generic;
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
            new Character()
            );

        CharacterStartLocations.Add(startLocation);
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