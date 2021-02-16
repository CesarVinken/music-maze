using System.Collections.Generic;
using UnityEngine;

public class MazeLevel
{
    public string MazeName;

    protected GridLocation _levelBounds = new GridLocation(0, 0);
    public GridLocation LevelBounds { get => _levelBounds; set => _levelBounds = value; }

    public List<PlayerExit> MazeExits = new List<PlayerExit>();

    public Dictionary<PlayerNumber, CharacterSpawnpoint> PlayerCharacterSpawnpoints = new Dictionary<PlayerNumber, CharacterSpawnpoint>();
    public List<CharacterSpawnpoint> EnemyCharacterSpawnpoints = new List<CharacterSpawnpoint>();

    protected GameObject _mazeContainer;
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