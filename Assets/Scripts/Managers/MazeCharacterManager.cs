using Character;
using Character.CharacterType;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class MazeCharacterManager : MonoBehaviourPunCallbacks, ICharacterManager
{
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

            SpawnPlayerCharacter(new CharacterBlueprint(new Emmon()), level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].GridLocation, PlayerNumber.Player1);
            SpawnEnemies();
        }
        else if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            Debug.Log("Instantiating Players");

            SpawnPlayerCharacter(new CharacterBlueprint(new Emmon()), level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].GridLocation, PlayerNumber.Player1);
            SpawnPlayerCharacter(new CharacterBlueprint(new Fae()), level.PlayerCharacterSpawnpoints[PlayerNumber.Player2].GridLocation, PlayerNumber.Player2);

            SpawnEnemies();
        }
        else
        {
            Debug.Log("Instantiating Player 2");

            SpawnPlayerCharacter(new CharacterBlueprint(new Fae()), level.PlayerCharacterSpawnpoints[PlayerNumber.Player2].GridLocation, PlayerNumber.Player2);
        }
    }

    public PlayerCharacter SpawnPlayerCharacter(CharacterBlueprint character, GridLocation gridLocation, PlayerNumber playerNumber)
    {
        string prefabName = GetPrefabNameByCharacter(character);
        Logger.Log($"prefab name is {prefabName}");
        Logger.Log($"prefab name is {character.CharacterType}");
        Vector2 startPosition = GetCharacterGridPosition(GridLocation.GridToVector(gridLocation)); // start position is grid position plus grid tile offset

        GameObject characterGO = null;

        //if (character.IsPlayable)
        //{
            if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
                GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
            {
                // todo: get prefab by string, like in network game case
                characterGO = GameObject.Instantiate(PlayerCharacterPrefab, startPosition, Quaternion.identity);
            }
            else
            {
                characterGO = PhotonNetwork.Instantiate(prefabName, startPosition, Quaternion.identity, 0);
            }

            PlayerCharacter playerCharacter = characterGO.GetComponent<PlayerCharacter>();
            playerCharacter.CharacterBlueprint = character;

            playerCharacter.FreezeCharacter();
            playerCharacter.SetStartingPoint(
                playerCharacter, 
                gridLocation,
                GameManager.Instance.CurrentGameLevel.PlayerCharacterSpawnpoints[playerCharacter.PlayerNumber]);
        //}
        //else
        //{
        //    if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
        //        GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        //    {
        //        characterGO = GameObject.Instantiate(EnemyCharacterPrefab, startPosition, Quaternion.identity);
        //    }
        //    else
        //    {
        //        characterGO = PhotonNetwork.Instantiate(prefabName, startPosition, Quaternion.identity, 0);
        //    }

        //    EnemyCharacter enemyCharacter = characterGO.GetComponent<EnemyCharacter>();
        //    enemyCharacter.SetSpawnpoint(enemyCharacter, spawnpoint as EnemySpawnpoint);
        //    enemyCharacter.SetTileAreas();
        //    enemyCharacter.FreezeCharacter();
        //    enemyCharacter.CharacterBlueprint = character;
        //}

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

    private void SpawnEnemies()
    {
        InGameMazeLevel level = GameManager.Instance.CurrentGameLevel as InGameMazeLevel;

        if (level == null) return;

        for (int i = 0; i < level.EnemyCharacterSpawnpoints.Count; i++)
        {
            SpawnEnemyCharacter(level.EnemyCharacterSpawnpoints[i].CharacterBlueprint, level.EnemyCharacterSpawnpoints[i]);
        }
    }

    public void SpawnEnemyCharacter(CharacterBlueprint character, CharacterSpawnpoint spawnpoint)
    {
        string prefabName = GetPrefabNameByCharacter(character);
        GameObject characterGO = null;
        Vector2 startPosition = GetCharacterGridPosition(GridLocation.GridToVector(spawnpoint.GridLocation)); // start position is grid position plus grid tile offset

        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
                GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            characterGO = GameObject.Instantiate(EnemyCharacterPrefab, startPosition, Quaternion.identity);
        }
        else
        {
            characterGO = PhotonNetwork.Instantiate(prefabName, startPosition, Quaternion.identity, 0);
        }

        EnemyCharacter enemyCharacter = characterGO.GetComponent<EnemyCharacter>();
        enemyCharacter.SetSpawnpoint(enemyCharacter, spawnpoint as EnemySpawnpoint);
        enemyCharacter.SetTileAreas();
        enemyCharacter.FreezeCharacter();
        enemyCharacter.CharacterBlueprint = character;

        characterGO.name = $"The Enemy {character.CharacterType}";
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

        if (exitedCharacters == 1 && GameRules.GamePlayerType != GamePlayerType.SinglePlayer)
        {
            player.FinishedFirstBonus = true;
        }

        if (exitedCharacters == _players.Count)
        {
            Logger.Warning("The level is completed!");
            MazeLevelGameplayManager.Instance.CompleteMazeLevel();
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

    public Dictionary<PlayerNumber, string> GetPlayerNames()
    {
        Dictionary<PlayerNumber, string> playerNamesDictionary = new Dictionary<PlayerNumber, string>();
        foreach (var item in _players)
        {
            playerNamesDictionary.Add(item.Key, item.Value.Name);
        }
        return playerNamesDictionary;
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

    public PlayerCharacter GetPlayerCharacter<T>(PlayerNumber playerNumber) where T : PlayerCharacter
    {
        if (_players.ContainsKey(playerNumber)) return _players[playerNumber] as MazePlayerCharacter;
        else return null;
    }

    public void AddPlayer(PlayerNumber playerNumber, PlayerCharacter playerCharacter)
    {
        _players.Add(playerNumber, playerCharacter as MazePlayerCharacter);
    }

    public void RemovePlayer(PlayerNumber playerNumber)
    {
        _players.Remove(playerNumber);
        if (playerNumber == PlayerNumber.Player1)
        {
            GameObject.Destroy(Player1GO);
        }
        else
        {
            GameObject.Destroy(Player2GO);
        }
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