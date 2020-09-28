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
        SetLocomotionTarget(GridLocation.GridToVector(GetRandomTileTarget().GridLocation));
        _hasTarget = true;
        CharacterPath.SearchPath();
    }

    public override void ReachLocomotionTarget()
    {
        Vector3 roundedVectorPosition = new Vector3((float)Math.Round(transform.position.x - GridLocation.OffsetToTileMiddle), (float)Math.Round(transform.position.y - GridLocation.OffsetToTileMiddle));
        transform.position = new Vector3(roundedVectorPosition.x + GridLocation.OffsetToTileMiddle, roundedVectorPosition.y + GridLocation.OffsetToTileMiddle, 0);

        AnimationHandler.SetLocomotion(false);
        _hasTarget = false;
    }

    public void CatchPlayer(PlayerCharacter playerCharacter)
    {
        float freezeTime = 2.0f;
        IEnumerator coroutine = playerCharacter.FreezeCharacter(playerCharacter, freezeTime);

        playerCharacter.ResetCharacterPosition();
        StartCoroutine(coroutine);
    }
}
