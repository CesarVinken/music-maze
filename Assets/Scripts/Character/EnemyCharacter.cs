using Photon.Pun;
using System;
using UnityEngine;

public class EnemyCharacter : Character
{
    public void Awake()
    {
        base.Awake();
        _characterPath.CharacterReachesTarget += OnTargetReached;
    }

    public void Start()
    {
        GameManager.Instance.CompleteMazeLevelEvent += OnMazeLevelCompleted;
    }

    public void Update()
    {
        if (EditorManager.InEditor) return;

        if (IsFrozen) return;

        if (!HasCalculatedTarget &&
            (GameManager.Instance.GameType == GameType.SinglePlayer
            || PhotonView.IsMine))
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
        Vector3 randomGridVectorLocation = GridLocation.GridToVector(GetRandomTileTarget().GridLocation);
        //Logger.Log("Set new target for enemy: {0},{1}", randomGridVectorLocation.x, randomGridVectorLocation.y);
        _seeker.StartPath(transform.position, randomGridVectorLocation, _characterPath.OnPathCalculated);
    }

    public void OnTargetReached()
    {
        //Logger.Log("enemy reached target");

        Vector3 roundedVectorPosition = new Vector3((float)Math.Round(transform.position.x - GridLocation.OffsetToTileMiddle), (float)Math.Round(transform.position.y - GridLocation.OffsetToTileMiddle));
        transform.position = new Vector3(roundedVectorPosition.x + GridLocation.OffsetToTileMiddle, roundedVectorPosition.y + GridLocation.OffsetToTileMiddle, 0);

        _animationHandler.SetLocomotion(false);
        SetHasCalculatedTarget(false);
    }

    public void OnMazeLevelCompleted()
    {
        //TODO: when maze level is completed, do not stop locomotion, but switch, if it exists, to an enemy idle animation
        IsFrozen = true;
    }
}
