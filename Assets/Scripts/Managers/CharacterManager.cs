using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterManager : MonoBehaviourPunCallbacks
{
    public static CharacterManager Instance;

    public GameObject Player1GO;
    public GameObject Player2GO;

    public RuntimeAnimatorController Bard1Controller;
    public RuntimeAnimatorController Bard2Controller;

    protected void _awake()
    {
        Instance = this;
    }

    public abstract void SpawnCharacters();

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

    public abstract Dictionary<PlayerNumber, T> GetPlayers<T>() where T : PlayerCharacter;

    public string GetPrefabNameByCharacter(CharacterBlueprint character)
    {
        return character.CharacterType.GetPrefabPath();
    }

    public virtual void UnfreezeCharacters()
    {

    }

    public virtual void UnloadCharacters()
    {

    }
}
