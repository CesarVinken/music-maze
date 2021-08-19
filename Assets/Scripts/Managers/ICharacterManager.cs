using System.Collections.Generic;
using UnityEngine;

public interface ICharacterManager
{
    GameObject Player1GO { get; set; }
    GameObject Player2GO { get; set; }

    RuntimeAnimatorController Bard1Controller { get; set; }
    RuntimeAnimatorController Bard2Controller { get; set; }

    void SpawnCharacters();
    void UnloadCharacters();
    void UnfreezeCharacters();

    void ExitCharacter(MazePlayerCharacter player);
    void SpawnPlayerCharacter(CharacterBlueprint character, GridLocation gridLocation, PlayerNumber playerNumber);

    void PutCharacterOnGrid(GameObject characterGO, Vector3 gridVectorLocation);

    Vector3 GetCharacterGridPosition(Vector3 gridVectorLocation);
    string GetPrefabNameByCharacter(CharacterBlueprint character);

    Dictionary<PlayerNumber, T> GetPlayers<T>();
    Dictionary<PlayerNumber, string> GetPlayerNames();
    int GetPlayerCount();
    void AddPlayer(PlayerNumber playerNumber, PlayerCharacter playerCharacter);
    void RemovePlayer(PlayerNumber playerNumber);
    PlayerCharacter GetPlayerCharacter<T>(PlayerNumber playerNumber) where T : PlayerCharacter;
    PlayerNumber GetOurPlayerCharacter();
}