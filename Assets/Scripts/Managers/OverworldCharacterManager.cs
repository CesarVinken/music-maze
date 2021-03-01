using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class OverworldCharacterManager : CharacterManager
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

    public Dictionary<PlayerNumber, OverworldPlayerCharacter> Players = new Dictionary<PlayerNumber, OverworldPlayerCharacter>();

    public void Awake()
    {
        base._awake();

        Guard.CheckIsNull(Bard1Controller, "Bard1Controller", gameObject);
        Guard.CheckIsNull(Bard2Controller, "Bard2Controller", gameObject);
    }

    public override void SpawnCharacters()
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


    public override void UnloadCharacters()
    {
        foreach (KeyValuePair<PlayerNumber, OverworldPlayerCharacter> p in Players)
        {
            Destroy(p.Value.gameObject);
        }

        Players.Clear();
    }

    public override void UnfreezeCharacters()
    {
        foreach (KeyValuePair<PlayerNumber, OverworldPlayerCharacter> p in Players)
        {
            p.Value.UnfreezeCharacter();
        }

    }

    public override Dictionary<PlayerNumber, OverworldPlayerCharacter> GetPlayers<OverworldPlayerCharacter>()
    {
        return Players as Dictionary<PlayerNumber, OverworldPlayerCharacter>;
    }
}