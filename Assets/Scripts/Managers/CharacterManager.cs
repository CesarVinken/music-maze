﻿using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections.Generic;
using UnityEngine;


public class CharacterManager : MonoBehaviourPunCallbacks
{
    public struct CharacterBundle
    {
        public GameObject CharacterGO;
        public Character Character;

        public CharacterBundle(Character character, GameObject characterGO)
        {
            Character = character;
            CharacterGO = characterGO;
        }
    }

    public static CharacterManager Instance;

    public GameObject EnemyCharacterPrefab;
    public GameObject PlayerCharacterPrefab;

    public GameObject Player1GO;
    public GameObject Player2GO;

    public List<PlayerCharacter> MazePlayers = new List<PlayerCharacter>();

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EnemyCharacterPrefab, "Could not find EnemyCharacterPrefab");
        Guard.CheckIsNull(PlayerCharacterPrefab, "Could not find PlayerCharacterPrefab");
    }

    public void SpawnCharacters()
    {
        if (PlayerManager.LocalPlayerInstance == null)
        {
            MazeLevel level = MazeLevelManager.Instance.Level;

            if(MazeLevelManager.PlayerCharacterSpawnpoints.Count != 2)
            {
                Logger.Error("Did not find 2, but {0} character startlocations for level", MazeLevelManager.PlayerCharacterSpawnpoints.Count);
            }
            
            if (PhotonNetwork.IsMasterClient || GameManager.Instance.GameType == GameType.SinglePlayer)
            {
                Debug.Log("Instantiating Player 1");
                CharacterBundle PlayerBundle = SpawnCharacter(MazeLevelManager.PlayerCharacterSpawnpoints[0].CharacterBlueprint, MazeLevelManager.PlayerCharacterSpawnpoints[0].GridLocation);
                Player1GO = PlayerBundle.CharacterGO;
                PlayerCharacter player = PlayerBundle.Character as PlayerCharacter;

                SpawnEnemies();
            }
            else
            {
                Debug.Log("Instantiating Player 2");

                CharacterBundle PlayerBundle = SpawnCharacter(MazeLevelManager.PlayerCharacterSpawnpoints[1].CharacterBlueprint, MazeLevelManager.PlayerCharacterSpawnpoints[1].GridLocation);
                Player2GO = PlayerBundle.CharacterGO;
                PlayerCharacter player = PlayerBundle.Character as PlayerCharacter;
            }
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < MazeLevelManager.EnemyCharacterSpawnpoints.Count; i++)
        {
            CharacterBundle enemy = SpawnCharacter(MazeLevelManager.EnemyCharacterSpawnpoints[i].CharacterBlueprint, MazeLevelManager.EnemyCharacterSpawnpoints[i].GridLocation);
            enemy.CharacterGO.name = "The Enemy";
        }
    }

    public CharacterBundle SpawnCharacter(CharacterBlueprint character, GridLocation gridLocation)
    {
        string prefabName = GetPrefabNameByCharacter(character);
        Vector2 startPosition = GetCharacterGridPosition(GridLocation.GridToVector(gridLocation)); // start position is grid position plus grid tile offset
        GameObject characterGO = PhotonNetwork.Instantiate(prefabName, startPosition, Quaternion.identity, 0);
        if (character.IsPlayable)
        {
            PlayerCharacter playerCharacter = characterGO.GetComponent<PlayerCharacter>();
            playerCharacter.CharacterBlueprint = character;

            playerCharacter.SetStartingPosition(playerCharacter, gridLocation);
            MazePlayers.Add(playerCharacter);

            if(GameManager.Instance.CurrentPlatform == Platform.PC)
            {
                if (MazePlayers.Count == 1)
                {
                    playerCharacter.KeyboardInput = KeyboardInput.Player1;
                    playerCharacter.PlayerNoInGame = 1;
                }
                else if (MazePlayers.Count == 2)
                {
                    playerCharacter.KeyboardInput = KeyboardInput.Player2;
                    playerCharacter.PlayerNoInGame = 2;
                }
                else
                {
                    Logger.Warning("There are {0} players in the level. There can be max 2 players in a level", MazePlayers.Count);
                }
            }
            CharacterBundle characterBundle = new CharacterBundle(playerCharacter, characterGO);
            return characterBundle;
        }
        else
        {
            EnemyCharacter enemyCharacter = characterGO.GetComponent<EnemyCharacter>();
            enemyCharacter.SetStartingPosition(enemyCharacter, gridLocation);
            enemyCharacter.CharacterBlueprint = character;

            CharacterBundle characterBundle = new CharacterBundle(enemyCharacter, characterGO);
            return characterBundle;
        }
    }

    public Vector3 GetCharacterGridPosition(Vector3 gridVectorLocation)
    {
        return new Vector3(gridVectorLocation.x + GridLocation.OffsetToTileMiddle, gridVectorLocation.y + GridLocation.OffsetToTileMiddle);
    }

    public void PutCharacterOnGrid(GameObject characterGO, Vector3 gridVectorLocation)
    {
        //Logger.Log("{0} was on {1},{2}", characterGO.name, characterGO.transform.position.x, characterGO.transform.position.y);
        characterGO.transform.position = new Vector3(gridVectorLocation.x + GridLocation.OffsetToTileMiddle, gridVectorLocation.y + GridLocation.OffsetToTileMiddle);
        //Logger.Log("{0} will be reset to {1},{2}", characterGO.name, characterGO.transform.position.x, characterGO.transform.position.y);
    }

    public string GetPrefabNameByCharacter(CharacterBlueprint character)
    {

        switch (character.CharacterType)
        {
            case CharacterType.Bard:
                return "Prefabs/Character/PlayerCharacter";
            case CharacterType.Dragon:
                return "Prefabs/Character/EnemyCharacter";
            default:
                Logger.Error(Logger.Initialisation, "Cannot find prefab for character type {0}", character.CharacterType);
                return null;
        }
    }
}
