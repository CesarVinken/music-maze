using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public GameObject EnemyCharacterPrefab;
    public GameObject PlayerCharacterPrefab;

    public List<PlayerCharacter> Players = new List<PlayerCharacter>();

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

    public void SpawnCharacter(CharacterBlueprint character, GridLocation gridLocation)
    {
        GameObject characterGO = GetCharacterPrefab(character);
        // place on grid
        characterGO.transform.position = GridLocation.GridToVector(gridLocation);

        if(character.IsPlayable)
        {
            PlayerCharacter playerCharacter = characterGO.GetComponent<PlayerCharacter>();
            Players.Add(playerCharacter);

            if(GameManager.Instance.CurrentPlatform == Platform.PC)
            {
                if (Players.Count == 1)
                {
                    playerCharacter.KeyboardInput = KeyboardInput.Player1;
                    playerCharacter.PlayerNoInGame = 1;
                }
                else if (Players.Count == 2)
                {
                    playerCharacter.KeyboardInput = KeyboardInput.Player2;
                    playerCharacter.PlayerNoInGame = 2;
                }
                else
                {
                    Logger.Warning("There are {0} players in the level. There can be max 2 players in a level", Players.Count);
                }
            }
        }
    }

    public GameObject GetCharacterPrefab(CharacterBlueprint character)
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
