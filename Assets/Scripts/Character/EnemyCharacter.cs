using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    public void Update()
    {
        if (IsFrozen) return;

        if (!HasCalculatedTarget)
        {
            SetRandomTarget();
        }
        else
        {
            MoveCharacter();
        }
    }

    public void SetRandomTarget()
    {
        Logger.Log("Set new target for enemy");
        Vector3 randomGridVectorLocation = GridLocation.GridToVector(GetRandomTileTarget().GridLocation);
        _seeker.StartPath(transform.position, randomGridVectorLocation, _characterPath.OnPathCalculated);

        //SetHasCalculatedTarget(true);
    }

    public override void ReachLocomotionTarget()
    {
        Logger.Log("enemy reached target");

        Vector3 roundedVectorPosition = new Vector3((float)Math.Round(transform.position.x - GridLocation.OffsetToTileMiddle), (float)Math.Round(transform.position.y - GridLocation.OffsetToTileMiddle));
        transform.position = new Vector3(roundedVectorPosition.x + GridLocation.OffsetToTileMiddle, roundedVectorPosition.y + GridLocation.OffsetToTileMiddle, 0);

        _animationHandler.SetLocomotion(false);
        SetHasCalculatedTarget(false);
        //if (!CharacterGO.CharacterBlueprint.IsPlayable)
        //{
        //    IEnumerator coroutine = FreezeCharacter(CharacterGO, 1f);
        //    StartCoroutine(coroutine);
        //}
    }

    public void CatchPlayer(PlayerCharacter playerCharacter)
    {
        float freezeTime = 2.0f;
        IEnumerator coroutine = playerCharacter.FreezeCharacter(playerCharacter, freezeTime);
        StartCoroutine(coroutine);
    }
}
