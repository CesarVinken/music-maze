using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public GameObject CharacterPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(CharacterPrefab, "Could not find CharacterPrefab");
    }

    public void SpawnCharacters()
    {
        MazeLevel level = MazeLevelManager.Instance.Level;

        for (int i = 0; i < level.CharacterStartLocations.Count; i++)
        {
            CharacterStartLocation characterStart = level.CharacterStartLocations[i];
            SpawnCharacter(characterStart.Character, characterStart.GridLocation);
        }
    }

    public void SpawnCharacter(Character character, GridLocation gridLocation)
    {
        // TODO: later load correct prefab based on what character
        GameObject characterGO = Instantiate(CharacterPrefab, transform);

        // place on grid
        characterGO.transform.position = GridLocation.GridToVector(gridLocation);
    }
}
