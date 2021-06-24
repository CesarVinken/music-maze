using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using CharacterType;
using System.Linq;

public class OverworldCharacterManager : MonoBehaviourPunCallbacks, ICharacterManager
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
            Logger.Log(Logger.Initialisation, "Instantiating Player 1");

            GridLocation spawnLocation = GetSpawnLocation(PlayerNumber.Player1, level);
            //TODO: Set which character spawns with which spawnpoint.
            CharacterBundle PlayerBundle = SpawnCharacter(
                new CharacterBlueprint(new Emmon()),
                spawnLocation);
            Player1GO = PlayerBundle.CharacterGO;
        }
        else if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            //TODO: Set which character spawns with which spawnpoint.
            CharacterBundle Player1Bundle = SpawnCharacter(new CharacterBlueprint(new Emmon()), level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].GridLocation);
            Player1GO = Player1Bundle.CharacterGO;

            CharacterBundle Player2Bundle = SpawnCharacter(new CharacterBlueprint(new Fae()), level.PlayerCharacterSpawnpoints[PlayerNumber.Player2].GridLocation);
            Player2GO = Player2Bundle.CharacterGO;
        }
        else
        {
            Logger.Log(Logger.Initialisation, "Instantiating Player 2");

            GridLocation spawnLocation = GetSpawnLocation(PlayerNumber.Player2, level);
            CharacterBundle PlayerBundle = SpawnCharacter(new CharacterBlueprint(new Fae()), spawnLocation);

            //TODO: Set which character spawns with which spawnpoint.
            //CharacterBundle PlayerBundle = SpawnCharacter(level.PlayerCharacterSpawnpoints[PlayerNumber.Player2].CharacterBlueprint, spawnLocation);
            Player2GO = PlayerBundle.CharacterGO;
        }
    }

    private CharacterBundle SpawnCharacter(CharacterBlueprint character, GridLocation gridLocation)
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
            playerCharacter as Character,
            gridLocation,
            GameManager.Instance.CurrentGameLevel.PlayerCharacterSpawnpoints[playerCharacter.PlayerNumber]
        );

        CharacterBundle characterBundle = new CharacterBundle(playerCharacter, characterGO);
        return characterBundle;
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
        return _players[playerNumber] as OverworldPlayerCharacter;
    }

    public void AddPlayer(PlayerNumber playerNumber, PlayerCharacter playerCharacter)
    {
        Logger.Log($"Added {playerNumber}");
        _players.Add(playerNumber, playerCharacter as OverworldPlayerCharacter);
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