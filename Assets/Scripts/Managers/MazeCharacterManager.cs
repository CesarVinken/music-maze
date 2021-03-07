﻿using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class MazeCharacterManager : MonoBehaviourPunCallbacks, ICharacterManager
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

    public GameObject EnemyCharacterPrefab;
    public GameObject PlayerCharacterPrefab;

    public RuntimeAnimatorController EnemyController;

    public List<EnemyCharacter> Enemies = new List<EnemyCharacter>();

    [SerializeField] private GameObject _player1GO;
    [SerializeField] private GameObject _player2GO;

    [SerializeField] private RuntimeAnimatorController _bard1Controller;
    [SerializeField] private RuntimeAnimatorController _bard2Controller;

    private Dictionary<PlayerNumber, MazePlayerCharacter> _players = new Dictionary<PlayerNumber, MazePlayerCharacter>();

    public GameObject Player1GO { get => _player1GO; set => _player1GO = value; }
    public GameObject Player2GO { get => _player2GO; set => _player2GO = value; }

    public RuntimeAnimatorController Bard1Controller { get => _bard1Controller; set => _bard1Controller = value; }
    public RuntimeAnimatorController Bard2Controller { get => _bard2Controller; set => _bard2Controller = value; }

    public void Awake()
    {
        Guard.CheckIsNull(EnemyCharacterPrefab, "EnemyCharacterPrefab", gameObject);
        Guard.CheckIsNull(PlayerCharacterPrefab, "PlayerCharacterPrefab", gameObject);

        Guard.CheckIsNull(_bard1Controller, "Bard1Controller", gameObject);
        Guard.CheckIsNull(_bard2Controller, "Bard2Controller", gameObject);
        Guard.CheckIsNull(EnemyController, "EnemyController", gameObject);

        GameManager.Instance.CharacterManager = this;
    }

    public void SpawnCharacters()
    {
        Logger.Log("Spawn characters...");

        IInGameLevel level = GameManager.Instance.CurrentGameLevel;

        if (level.PlayerCharacterSpawnpoints.Count != 2)
        {
            Logger.Error("Did not find 2, but {0} character startlocations for level", level.PlayerCharacterSpawnpoints.Count);
        }

        if (PhotonNetwork.IsMasterClient || GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            Debug.Log("Instantiating Player 1");

            CharacterBundle PlayerBundle = SpawnCharacter(level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].CharacterBlueprint, level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].GridLocation);
            Player1GO = PlayerBundle.CharacterGO;
            PlayerCharacter player = PlayerBundle.Character as PlayerCharacter;
            SpawnEnemies();
        }
        else
        {
            Debug.Log("Instantiating Player 2");

            CharacterBundle PlayerBundle = SpawnCharacter(level.PlayerCharacterSpawnpoints[PlayerNumber.Player2].CharacterBlueprint, level.PlayerCharacterSpawnpoints[PlayerNumber.Player2].GridLocation);
            Player2GO = PlayerBundle.CharacterGO;
            PlayerCharacter player = PlayerBundle.Character as PlayerCharacter;
        }
    }

    private CharacterBundle SpawnCharacter(CharacterBlueprint character, GridLocation gridLocation)
    {
        string prefabName = GetPrefabNameByCharacter(character);
        Vector2 startPosition = GetCharacterGridPosition(GridLocation.GridToVector(gridLocation)); // start position is grid position plus grid tile offset

        GameObject characterGO = PhotonNetwork.Instantiate(prefabName, startPosition, Quaternion.identity, 0); // TODO solve prefab for single player
        if (character.IsPlayable)
        {
            PlayerCharacter playerCharacter = characterGO.GetComponent<PlayerCharacter>();
            playerCharacter.CharacterBlueprint = character;

            playerCharacter.FreezeCharacter();
            playerCharacter.SetStartingPosition(playerCharacter, gridLocation);

            if (PersistentGameManager.CurrentPlatform == Platform.PC)
            {
                if (_players.Count == 0)
                {
                    playerCharacter.KeyboardInput = KeyboardInput.Player1;
                    playerCharacter.PlayerNoInGame = 1;

                }
                else if (_players.Count == 1)
                {
                    playerCharacter.KeyboardInput = KeyboardInput.Player2;
                    playerCharacter.PlayerNoInGame = 2;
                }
                else
                {
                    Logger.Warning("There are {0} players in the level. There can be max 2 players in a level", _players.Count);
                }
            }
            CharacterBundle characterBundle = new CharacterBundle(playerCharacter, characterGO);
            return characterBundle;
        }
        else
        {
            EnemyCharacter enemyCharacter = characterGO.GetComponent<EnemyCharacter>();
            enemyCharacter.SetStartingPosition(enemyCharacter, gridLocation);
            enemyCharacter.FreezeCharacter();
            enemyCharacter.CharacterBlueprint = character;

            CharacterBundle characterBundle = new CharacterBundle(enemyCharacter, characterGO);
            return characterBundle;
        }
    }

    private void SpawnEnemies()
    {
        InGameMazeLevel level = GameManager.Instance.CurrentGameLevel as InGameMazeLevel;

        if (level == null) return;

        for (int i = 0; i < level.EnemyCharacterSpawnpoints.Count; i++)
        {
            CharacterBundle enemy = SpawnCharacter(level.EnemyCharacterSpawnpoints[i].CharacterBlueprint, level.EnemyCharacterSpawnpoints[i].GridLocation);
            enemy.CharacterGO.name = "The Enemy";
        }
    }

    public void ExitCharacter(MazePlayerCharacter player)
    {
        //For now just hide and freeze character, later play animation etc.
        player.Exit();

        // Check if all players are gone. If so, the level is completed;
        int exitedCharacters = 0;
        foreach (KeyValuePair<PlayerNumber, MazePlayerCharacter> p in _players)
        {
            if (p.Value.HasReachedExit)
            {
                exitedCharacters++;
            }
        }

        if (exitedCharacters == _players.Count)
        {
            Logger.Warning("The level is completed!");
            GameManager.Instance.CompleteMazeLevel();
        }
    }

    public void UnloadCharacters()
    {
        foreach (KeyValuePair<PlayerNumber, MazePlayerCharacter> p in _players)
        {
            Destroy(p.Value.gameObject);
        }

        _players.Clear();

        for (int j = 0; j < Enemies.Count; j++)
        {
            Destroy(Enemies[j].gameObject);
        }
        Enemies.Clear();
    }

    public void UnfreezeCharacters()
    {
        foreach (KeyValuePair<PlayerNumber, MazePlayerCharacter> p in _players)
        {
            p.Value.UnfreezeCharacter();
        }

        for (int j = 0; j < Enemies.Count; j++)
        {
            Enemies[j].UnfreezeCharacter();
        }
    }

    public Dictionary<PlayerNumber, MazePlayerCharacter> GetPlayers<MazePlayerCharacter>()
    {
        return _players as Dictionary<PlayerNumber, MazePlayerCharacter>;
    }

    public string GetPrefabNameByCharacter(CharacterBlueprint character)
    {
        return character.CharacterType.GetPrefabPath();
    }

    public Vector3 GetCharacterGridPosition(Vector3 gridVectorLocation)
    {
        return new Vector3(gridVectorLocation.x + GridLocation.OffsetToTileMiddle, gridVectorLocation.y + GridLocation.OffsetToTileMiddle);
    }

    public void PutCharacterOnGrid(GameObject characterGO, Vector3 gridVectorLocation)
    {
        characterGO.transform.position =
            new Vector3(
                gridVectorLocation.x + GridLocation.OffsetToTileMiddle,
                gridVectorLocation.y + GridLocation.OffsetToTileMiddle
            );
    }

    public PlayerCharacter GetPlayerCharacter<T>(PlayerNumber playerNumber) where T : PlayerCharacter
    {
        return _players[playerNumber] as MazePlayerCharacter;
    }

    public void AddPlayer(PlayerNumber playerNumber, PlayerCharacter playerCharacter)
    {
        _players.Add(playerNumber, playerCharacter as MazePlayerCharacter);
    }

    public PlayerNumber GetOurPlayerCharacter()
    {
        PlayerNumber ourPlayerCharacter = PlayerNumber.Player1;

        foreach (KeyValuePair<PlayerNumber, MazePlayerCharacter> p in _players)
        {
            if (p.Value.PhotonView.IsMine) return p.Key;
        }

        return ourPlayerCharacter;
    }
}