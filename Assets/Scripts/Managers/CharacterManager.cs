using Photon.Pun;
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
    public List<EnemyCharacter> Enemies = new List<EnemyCharacter>();

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EnemyCharacterPrefab, "EnemyCharacterPrefab", gameObject);
        Guard.CheckIsNull(PlayerCharacterPrefab, "PlayerCharacterPrefab", gameObject);
    }

    public void SpawnCharacters()
    {
        if (PlayerManager.LocalPlayerInstance == null)
        {
            MazeLevel level = MazeLevelManager.Instance.Level;

            if(level.PlayerCharacterSpawnpoints.Count != 2)
            {
                Logger.Error("Did not find 2, but {0} character startlocations for level", level.PlayerCharacterSpawnpoints.Count);
            }
            
            if (PhotonNetwork.IsMasterClient || GameManager.Instance.GameType == GameType.SinglePlayer)
            {
                Debug.Log("Instantiating Player 1");
                CharacterBundle PlayerBundle = SpawnCharacter(level.PlayerCharacterSpawnpoints[0].CharacterBlueprint, level.PlayerCharacterSpawnpoints[0].GridLocation);
                Player1GO = PlayerBundle.CharacterGO;
                PlayerCharacter player = PlayerBundle.Character as PlayerCharacter;

                SpawnEnemies();
            }
            else
            {
                Debug.Log("Instantiating Player 2");

                CharacterBundle PlayerBundle = SpawnCharacter(level.PlayerCharacterSpawnpoints[1].CharacterBlueprint, level.PlayerCharacterSpawnpoints[1].GridLocation);
                Player2GO = PlayerBundle.CharacterGO;
                PlayerCharacter player = PlayerBundle.Character as PlayerCharacter;
            }
        }
    }

    private void SpawnEnemies()
    {
        MazeLevel level = MazeLevelManager.Instance.Level;
        for (int i = 0; i < level.EnemyCharacterSpawnpoints.Count; i++)
        {
            CharacterBundle enemy = SpawnCharacter(level.EnemyCharacterSpawnpoints[i].CharacterBlueprint, level.EnemyCharacterSpawnpoints[i].GridLocation);
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
            Enemies.Add(enemyCharacter);

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

    public void CharacterExit(PlayerCharacter player)
    {
        //For now just hide and freeze character, later play animation etc.
        player.Exit();

        // Check if all players are gone. If so, the level is completed;

        int exitedCharacters = 0;
        for (int i = 0; i < MazePlayers.Count; i++)
        {
            PlayerCharacter p = MazePlayers[i];

            if(p.HasReachedExit)
                exitedCharacters++;
        }

        if(exitedCharacters == MazePlayers.Count)
        {
            Logger.Warning("The level is completed!");
        }
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

    public void UnloadCharacters()
    {
        for (int i = 0; i < MazePlayers.Count; i++)
        {
            Destroy(MazePlayers[i].gameObject);
        }
        MazePlayers.Clear();

        for (int j = 0; j < Enemies.Count; j++)
        {
            Destroy(Enemies[j].gameObject);
        }
        Enemies.Clear();

    }
}
