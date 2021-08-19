using Character;
using Character.CharacterType;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class OverworldCharacterManager : MonoBehaviourPunCallbacks, ICharacterManager
{
    [SerializeField] private GameObject _player1GO;
    [SerializeField] private GameObject _player2GO;

    [SerializeField] private RuntimeAnimatorController _bard1Controller;
    [SerializeField] private RuntimeAnimatorController _bard2Controller;

    private Dictionary<PlayerNumber, OverworldPlayerCharacter> _players = new Dictionary<PlayerNumber, OverworldPlayerCharacter>();

    public GameObject Player1GO { get => _player1GO; set => _player1GO = value; }
    public GameObject Player2GO { get => _player2GO; set => _player2GO = value; }

    [SerializeField] private GameObject _playerCharacterPrefab;

    public RuntimeAnimatorController Bard1Controller { get => _bard1Controller; set => _bard1Controller = value; }
    public RuntimeAnimatorController Bard2Controller { get => _bard2Controller; set => _bard2Controller = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_bard1Controller, "Bard1Controller", gameObject);
        Guard.CheckIsNull(_bard2Controller, "Bard2Controller", gameObject);

        Guard.CheckIsNull(_playerCharacterPrefab, "PlayerCharacterPrefab", gameObject);

        GameManager.Instance.CharacterManager = this;
    }

    public void SpawnCharacters()
    {
        Logger.Log("Spawn characters...");

        InGameOverworld level = GameManager.Instance.CurrentGameLevel as InGameOverworld;

        if (level == null) return;

        if (level.PlayerCharacterSpawnpoints.Count != 2)
        {
            Logger.Error("Did not find 2, but {0} character startlocations for level", level.PlayerCharacterSpawnpoints.Count);
        }

        if (PhotonNetwork.IsMasterClient || GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            string characterName = PersistentGameManager.PlayerCharacterNames[PlayerNumber.Player1];
            Logger.Log(Logger.Initialisation, $"Instantiating '{characterName}' Player 1");

            GridLocation spawnLocation = GetSpawnLocation(PlayerNumber.Player1, level);
            //TODO: Set which character spawns with which spawnpoint.
            SpawnPlayerCharacter(
                new CharacterBlueprint(CharacterSpawner.GetCharacterToSpawn(characterName)),
                spawnLocation,
                PlayerNumber.Player1);
        }
        else if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            //TODO: Set which character spawns with which spawnpoint.
            SpawnPlayerCharacter(new CharacterBlueprint(new Emmon()),
                level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].GridLocation,
                PlayerNumber.Player1);

            SpawnPlayerCharacter(new CharacterBlueprint(new Fae()),
                level.PlayerCharacterSpawnpoints[PlayerNumber.Player2].GridLocation,
                PlayerNumber.Player2);
        }
        else
        {
            string characterName = PersistentGameManager.PlayerCharacterNames[PlayerNumber.Player2];
            Logger.Log(Logger.Initialisation, $"Instantiating '{characterName}' Player 2");

            GridLocation spawnLocation = GetSpawnLocation(PlayerNumber.Player2, level);

            SpawnPlayerCharacter(new CharacterBlueprint(CharacterSpawner.GetCharacterToSpawn(characterName)),
                spawnLocation,
                PlayerNumber.Player2);
        }
    }

    public PlayerCharacter SpawnPlayerCharacter(CharacterBlueprint character, GridLocation gridLocation, PlayerNumber playerNumber)
    {
        string prefabName = GetPrefabNameByCharacter(character);
        Vector2 startPosition = GetCharacterGridPosition(GridLocation.GridToVector(gridLocation)); // start position is grid position plus grid tile offset

        GameObject characterGO = null;
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            // todo: get prefab by string, like in network game case
            characterGO = GameObject.Instantiate(_playerCharacterPrefab, startPosition, Quaternion.identity);
        }
        else
        {
            Logger.Log($"We want to instantiate a character with the name {prefabName}");
            characterGO = PhotonNetwork.Instantiate(prefabName, startPosition, Quaternion.identity, 0);
        }

        PlayerCharacter playerCharacter = characterGO.GetComponent<PlayerCharacter>();
        playerCharacter.CharacterBlueprint = character;

        playerCharacter.FreezeCharacter();
        playerCharacter.SetStartingPoint(
            playerCharacter as Character.Character,
            gridLocation,
            GameManager.Instance.CurrentGameLevel.PlayerCharacterSpawnpoints[playerCharacter.PlayerNumber]
        );

        switch (playerNumber)
        {
            case PlayerNumber.Player1:
                Player1GO = characterGO;
                break;
            case PlayerNumber.Player2:
                Player2GO = characterGO;
                break;
            default:
                break;
        }
        return playerCharacter;
    }

    public void UnloadCharacters()
    {
        foreach (KeyValuePair<PlayerNumber, OverworldPlayerCharacter> p in _players)
        {
            Destroy(p.Value.gameObject);
        }

        _players.Clear();
    }

    public void UnfreezeCharacters()
    {
        foreach (KeyValuePair<PlayerNumber, OverworldPlayerCharacter> p in _players)
        {
            p.Value.UnfreezeCharacter();
        }
    }

    public void ExitCharacter(MazePlayerCharacter player)
    {
        Logger.Log("Exit player character");
    }

    public Dictionary<PlayerNumber, PlayerCharacter> GetPlayers<PlayerCharacter>()
    {
        return _players as Dictionary<PlayerNumber, PlayerCharacter>;
    }

    public Dictionary<PlayerNumber, string> GetPlayerNames()
    {
        Dictionary<PlayerNumber, string> playerNamesDictionary = new Dictionary<PlayerNumber, string>();
        foreach (var item in _players)
        {
            playerNamesDictionary.Add(item.Key, item.Value.Name);
        }
        return playerNamesDictionary;
    }

    public PlayerCharacter GetPlayerCharacter<T>(PlayerNumber playerNumber) where T : PlayerCharacter
    {
        if (_players.ContainsKey(playerNumber)) return _players[playerNumber] as OverworldPlayerCharacter;
        else return null;
    }

    public void AddPlayer(PlayerNumber playerNumber, PlayerCharacter playerCharacter)
    {
        Logger.Log($"Added {playerNumber}");
        _players.Add(playerNumber, playerCharacter as OverworldPlayerCharacter);
    }

    public void RemovePlayer(PlayerNumber playerNumber)
    {
        _players.Remove(playerNumber);
        if(playerNumber == PlayerNumber.Player1)
        {
            GameObject.Destroy(Player1GO);
        }
        else
        {
            GameObject.Destroy(Player2GO);
        }
        Logger.Log($"_players.count {_players.Count}");
    }

    public int GetPlayerCount()
    {
        return _players.Count;
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

    public PlayerNumber GetOurPlayerCharacter()
    {
        PlayerNumber ourPlayerCharacter = PlayerNumber.Player1;

        foreach (KeyValuePair<PlayerNumber, OverworldPlayerCharacter> p in _players)
        {
            if (p.Value.PhotonView.IsMine) return p.Key;
        }

        return ourPlayerCharacter;
    }

    private GridLocation GetSpawnLocation(PlayerNumber playerNumber, InGameOverworld level)
    {
        for (int i = 0; i < OverworldGameplayManager.Instance.Overworld.MazeEntries.Count; i++)
        {
            MazeLevelEntry mazeLevelEntry = OverworldGameplayManager.Instance.Overworld.MazeEntries[i];
            if (mazeLevelEntry.MazeLevelName == PersistentGameManager.LastMazeLevelName)
            {
                return mazeLevelEntry.Tile.GridLocation;
            }
        }

        // If we do not find a location for the last level, spawn charactes at the defaul player character spawnpoint.
        if (playerNumber == PlayerNumber.Player1)
        {
            return level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].GridLocation;
        }

        return level.PlayerCharacterSpawnpoints[PlayerNumber.Player2].GridLocation;
    }
}

public static class CharacterSpawner
{
    public static ICharacter GetCharacterToSpawn(string characterName)
    {
        switch (characterName)
        {
            case "Emmon":
                return new Emmon();
            case "Fae":
                return new Fae();
            default:
                Logger.Error($"Cannot spawn a character with the name {characterName}");
                return null;
        }
    }
}