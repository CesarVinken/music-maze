using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

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

    public RuntimeAnimatorController Bard1Controller { get => _bard1Controller; set => _bard1Controller = value; }
    public RuntimeAnimatorController Bard2Controller { get => _bard2Controller; set => _bard2Controller = value; }
    
    public void Awake()
    {
        Guard.CheckIsNull(_bard1Controller, "Bard1Controller", gameObject);
        Guard.CheckIsNull(_bard2Controller, "Bard2Controller", gameObject);

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
            Debug.Log("Instantiating Player 1");

            CharacterBundle PlayerBundle = SpawnCharacter(
                level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].CharacterBlueprint,
                level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].GridLocation);
            Player1GO = PlayerBundle.CharacterGO;
            PlayerCharacter player = PlayerBundle.Character as PlayerCharacter;
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
        Logger.Log(character.CharacterType.GetType());

        string prefabName = GetPrefabNameByCharacter(character);
        Vector2 startPosition = GetCharacterGridPosition(GridLocation.GridToVector(gridLocation)); // start position is grid position plus grid tile offset

        GameObject characterGO = PhotonNetwork.Instantiate(prefabName, startPosition, Quaternion.identity, 0); // TODO solve prefab for single player

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

    public Dictionary<PlayerNumber, OverworldPlayerCharacter> GetPlayers<OverworldPlayerCharacter>()
    {
        return _players as Dictionary<PlayerNumber, OverworldPlayerCharacter>;
    }

    public PlayerCharacter GetPlayerCharacter<T>(PlayerNumber playerNumber) where T : PlayerCharacter
    {
        return _players[playerNumber] as OverworldPlayerCharacter;
    }

    public void AddPlayer(PlayerNumber playerNumber, PlayerCharacter playerCharacter)
    {
        _players.Add(playerNumber, playerCharacter as OverworldPlayerCharacter);
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
}