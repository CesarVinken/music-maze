using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviourPunCallbacks
{
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

            if(level.CharacterStartLocations.Count != 2)
            {
                Logger.Error("Did not find 2, but {0} character startlocations for level", level.CharacterStartLocations.Count);
            }

            Logger.Warning("Loop over names");
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Logger.Log(PhotonNetwork.PlayerList[i].NickName);
                //Logger.Log(PhotonNetwork.PlayerList[i].UserId);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Instantiating Player 1");
                CharacterStartLocation characterStart = level.CharacterStartLocations[0];
                //Vector2 startPosition = GridLocation.GridToVector(characterStart.GridLocation);
                Player1GO = SpawnCharacter(characterStart.Character, characterStart.GridLocation);
                //Player1GO.name = PhotonNetwork.PlayerList[0].NickName;
                //ball = PhotonNetwork.Instantiate("Ball", ballSpawnTransform.transform.position, ballSpawnTransform.transform.rotation, 0);
                //ball.name = "Ball";
            }
            else
            {
                Debug.Log("Instantiating Player 2");

                CharacterStartLocation characterStart = level.CharacterStartLocations[1];
                //Vector2 startPosition = GridLocation.GridToVector(characterStart.GridLocation);
                Player2GO = SpawnCharacter(characterStart.Character, characterStart.GridLocation);
                //Player2GO.name = PhotonNetwork.PlayerList[1].NickName;
            }
        }


        //MazeLevel level = MazeLevelManager.Instance.Level;

        //for (int i = 0; i < level.CharacterStartLocations.Count; i++)
        //{
        //    CharacterStartLocation characterStart = level.CharacterStartLocations[i];
        //    SpawnCharacter(characterStart.Character, characterStart.GridLocation);
        //}
    }

    public GameObject SpawnCharacter(CharacterBlueprint character, GridLocation gridLocation)
    {
        string prefabName = GetPrefabNameByCharacter(character);
        Vector2 startPosition = GetCharacterGridPosition(GridLocation.GridToVector(gridLocation)); // start position is grid position plus grid tile offset
        GameObject characterGO = PhotonNetwork.Instantiate(prefabName, startPosition, Quaternion.identity, 0);
        //Vector3 gridVectorLocation = GridLocation.GridToVector(gridLocation);

        if(character.IsPlayable)
        {
            PlayerCharacter playerCharacter = characterGO.GetComponent<PlayerCharacter>();
            playerCharacter.CharacterBlueprint = character;
            playerCharacter.SetStartingPosition(gridLocation);
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
        }
        else
        {
            EnemyCharacter enemyCharacter = characterGO.GetComponent<EnemyCharacter>();
            enemyCharacter.SetStartingPosition(gridLocation);
            enemyCharacter.CharacterBlueprint = character;
        }

        return characterGO;
    }

    public Vector3 GetCharacterGridPosition(Vector3 gridVectorLocation)
    {
        return new Vector3(gridVectorLocation.x + GridLocation.OffsetToTileMiddle, gridVectorLocation.y + GridLocation.OffsetToTileMiddle);
    }

    public void PutCharacterOnGrid(GameObject characterGO, Vector3 gridVectorLocation)
    {
        Logger.Log("it was {0},{1}", characterGO.transform.position.x, characterGO.transform.position.y);
        characterGO.transform.position = new Vector3(gridVectorLocation.x + GridLocation.OffsetToTileMiddle, gridVectorLocation.y + GridLocation.OffsetToTileMiddle);
        Logger.Log("{0} will be now {1},{2}", characterGO.name, characterGO.transform.position.x, characterGO.transform.position.y);
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
