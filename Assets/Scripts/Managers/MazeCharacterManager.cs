using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class MazeCharacterManager : CharacterManager
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

    [Space(10)]
    [Header("Enemies")]
    public List<EnemyCharacter> Enemies = new List<EnemyCharacter>();
    public Dictionary<PlayerNumber, MazePlayerCharacter> Players = new Dictionary<PlayerNumber, MazePlayerCharacter>();

    public void Awake()
    {
        base._awake();

        Guard.CheckIsNull(EnemyCharacterPrefab, "EnemyCharacterPrefab", gameObject);
        Guard.CheckIsNull(PlayerCharacterPrefab, "PlayerCharacterPrefab", gameObject);

        Guard.CheckIsNull(Bard1Controller, "Bard1Controller", gameObject);
        Guard.CheckIsNull(Bard2Controller, "Bard2Controller", gameObject);
        Guard.CheckIsNull(EnemyController, "EnemyController", gameObject);
    }

    public override void SpawnCharacters()
    {
        Logger.Log("Spawn characters...");

        IInGameLevel level = GameManager.Instance.CurrentGameLevel;

        if (level.PlayerCharacterSpawnpoints.Count != 2)
        {
            Logger.Error("Did not find 2, but {0} character startlocations for level", level.PlayerCharacterSpawnpoints.Count);
        }

        if (PhotonNetwork.IsMasterClient || GameManager.GameType == GameType.SinglePlayer)
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
        Logger.Log(prefabName);
        GameObject characterGO = PhotonNetwork.Instantiate(prefabName, startPosition, Quaternion.identity, 0); // TODO solve prefab for single player
        if (character.IsPlayable)
        {
            PlayerCharacter playerCharacter = characterGO.GetComponent<PlayerCharacter>();
            playerCharacter.CharacterBlueprint = character;

            playerCharacter.FreezeCharacter();
            playerCharacter.SetStartingPosition(playerCharacter, gridLocation);

            if (GameManager.CurrentPlatform == Platform.PC)
            {
                if (Players.Count == 0)
                {
                    playerCharacter.KeyboardInput = KeyboardInput.Player1;
                    playerCharacter.PlayerNoInGame = 1;

                }
                else if (Players.Count == 1)
                {
                    playerCharacter.KeyboardInput = KeyboardInput.Player2;
                    playerCharacter.PlayerNoInGame = 2;
                }
                else
                {
                    Logger.Warning("There are {0} players in the level. There can be max 2 players in a level", Players.Count);
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

    public void CharacterExit(MazePlayerCharacter player)
    {
        //For now just hide and freeze character, later play animation etc.
        player.Exit();

        // Check if all players are gone. If so, the level is completed;

        int exitedCharacters = 0;
        foreach (KeyValuePair<PlayerNumber, MazePlayerCharacter> p in Players)
        {
            if (p.Value.HasReachedExit)
                exitedCharacters++;
        }

        if (exitedCharacters == Players.Count)
        {
            Logger.Warning("The level is completed!");
            GameManager.Instance.CompleteMazeLevel();
        }
    }

    public override string GetPrefabNameByCharacter(CharacterBlueprint character)
    {
        switch (character.CharacterType)
        {
            case CharacterType.Bard1:
            case CharacterType.Bard2:
                return "Prefabs/Character/MazePlayerCharacter";
            case CharacterType.Enemy:
                return "Prefabs/Character/EnemyCharacter";
            default:
                Logger.Error(Logger.Initialisation, "Cannot find prefab for character type {0}", character.CharacterType);
                return null;
        }
    }

    public override void UnloadCharacters()
    {
        foreach (KeyValuePair<PlayerNumber, MazePlayerCharacter> p in Players)
        {
            Destroy(p.Value.gameObject);
        }

        Players.Clear();

        for (int j = 0; j < Enemies.Count; j++)
        {
            Destroy(Enemies[j].gameObject);
        }
        Enemies.Clear();
    }

    public override void UnfreezeCharacters()
    {
        foreach (KeyValuePair<PlayerNumber, MazePlayerCharacter> p in Players)
        {
            p.Value.UnfreezeCharacter();
        }

        for (int j = 0; j < Enemies.Count; j++)
        {
            Enemies[j].UnfreezeCharacter();
        }
    }

    public override Dictionary<PlayerNumber, MazePlayerCharacter> GetPlayers<MazePlayerCharacter>()
    {
        return Players as Dictionary<PlayerNumber, MazePlayerCharacter>;
    }
}