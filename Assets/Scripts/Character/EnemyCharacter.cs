using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    private bool _hasTarget = false;

    public void Update()
    {
        if (IsFrozen) return;

        if (!_hasTarget)
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
        SetLocomotionTargetObject(GridLocation.GridToVector(GetRandomTileTarget().GridLocation));
        _hasTarget = true;
    }

    public override void ReachLocomotionTarget()
    {
        Logger.Log("enemy reached target");
        //IsOnTile = true;
        Vector3 roundedVectorPosition = new Vector3((float)Math.Round(transform.position.x - GridLocation.OffsetToTileMiddle), (float)Math.Round(transform.position.y - GridLocation.OffsetToTileMiddle));
        transform.position = new Vector3(roundedVectorPosition.x + GridLocation.OffsetToTileMiddle, roundedVectorPosition.y + GridLocation.OffsetToTileMiddle, 0);

        AnimationHandler.SetLocomotion(false);
        _hasTarget = false;
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
