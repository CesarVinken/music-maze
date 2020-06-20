using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public GameObject EnemyCharacterPrefab;
    public GameObject PlayerCharacterPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EnemyCharacterPrefab, "Could not find EnemyCharacterPrefab");
        Guard.CheckIsNull(PlayerCharacterPrefab, "Could not find PlayerCharacterPrefab");
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
        GameObject characterGO = GetCharacterPrefab(character);
        // place on grid
        characterGO.transform.position = GridLocation.GridToVector(gridLocation);
    }

    public GameObject GetCharacterPrefab(Character character)
    {
        switch (character.CharacterType)
        {
            case CharacterType.Bard:
                return Instantiate(PlayerCharacterPrefab, transform);
            case CharacterType.Dragon:
                return Instantiate(EnemyCharacterPrefab, transform);
            default:
                Logger.Error(Logger.Initialisation, "Cannot find prefab for character type {0}", character.CharacterType);
                return null;
        }
    }
}
