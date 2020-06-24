using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterLocomotion CharacterLocomotion;
    public GridLocation StartingPosition;

    public void Awake()
    {
        if (CharacterLocomotion == null)
            Logger.Error(Logger.Initialisation, "Could not find CharacterLocomotion component on character");
    }

    public void SetStartingPosition(GridLocation gridLocation)
    {
        StartingPosition = gridLocation;
    }

    public void ResetCharacterPosition()
    {
        CharacterLocomotion.SetLocomotionTarget(GridLocation.GridToVector(StartingPosition));
        CharacterManager.Instance.PutCharacterOnGrid(gameObject, GridLocation.GridToVector(StartingPosition));
        CharacterLocomotion.ReachLocomotionTarget();
    }
}
