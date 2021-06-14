using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class MazePlayerCharacter : PlayerCharacter
{
    public bool HasReachedExit = false;
    public bool FinishedFirstBonus = false;

    public event Action PlayerExitsEvent;
    public event Action PlayerCaughtEvent;

    public int TimesCaught = 0;

    public override void Awake()
    {
        base.Awake();

        HasReachedExit = false;
    }

    public override void Start()
    {
        base.Start();

        PlayerExitsEvent += OnPlayerExit;
        PlayerCaughtEvent += OnPlayerCaught;

        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer || 
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer ||
            PhotonView.IsMine)
        {
            _selectionIndicatorGO = Instantiate(_selectionIndicatorPrefab, SceneObjectManager.Instance.CharactersGO);

            SelectionIndicator selectionIndicator = _selectionIndicatorGO.GetComponent<SelectionIndicator>();
            selectionIndicator.Setup(transform, this);

            SceneObjectManager.Instance.SceneObjects.Add(_selectionIndicatorGO);
        }

        //transform the player's starting tile and surrounding tiles
        InGameMazeTile currentTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[StartingPosition] as InGameMazeTile;

        if(currentTile == null)
        {
            Logger.Error($"Current tile at {StartingPosition.X},{StartingPosition.Y} is null");
        }

        currentTile.TriggerTransformations();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !PhotonView.IsMine) return;

        EnemyCharacter enemy = collision.gameObject.GetComponent<EnemyCharacter>();
        if (enemy != null)
        {
            PlayerCaughtEvent?.Invoke();
        }
    }

    public void Exit()
    {
        PlayerExitsEvent?.Invoke();
    }

    private void OnPlayerExit()
    {
        FreezeCharacter();
        HasReachedExit = true;
        Logger.Log($"{gameObject.name} reached the exit");
        CharacterBody.SetActive(false);
    }

    public void OnPlayerCaught()
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            PunRPCCaughtByEnemy();
        }
        else
        {
            PhotonView.RPC("PunRPCCaughtByEnemy", RpcTarget.All);
        }

        float freezeTime = 2.0f;

        IEnumerator coroutine = this.RespawnPlayerCharacter(this, freezeTime);
        StartCoroutine(coroutine);

        _isPressingPointerForSeconds = false;
    }

    [PunRPC] // the part all clients need to be informed about
    private void PunRPCCaughtByEnemy()
    {
        TimesCaught++;
    }

    public override bool ValidateTarget(GridLocation targetGridLocation, ObjectDirection direction)
    {
        if (MazeLevelGameplayManager.Instance.Level.TilesByLocation.TryGetValue(targetGridLocation, out Tile targetTile))
        {
            if (targetTile.Walkable)
            {
                Tile currentTile = MazeLevelGameplayManager.Instance.Level.TilesByLocation[CurrentGridLocation];
                BridgePiece bridgePieceOnCurrentTile = currentTile.TryGetBridgePiece();
                BridgePiece bridgePieceOnTarget = targetTile.TryGetBridgePiece(); // optimisation: keep bridge locations of the level in a separate list, so we don't have to go over all the tiles in the level

                // there are no bridges involved
                if (bridgePieceOnCurrentTile == null && bridgePieceOnTarget == null)
                {
                    return true;
                }

                // We go in the correct bridge direction
                if (bridgePieceOnCurrentTile && bridgePieceOnTarget)
                {
                    if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Horizontal &&
                        bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Horizontal &&
                        (direction == ObjectDirection.Left || direction == ObjectDirection.Right))
                    {
                        return true;
                    }

                    if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Vertical &&
                        bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Vertical &&
                        (direction == ObjectDirection.Up || direction == ObjectDirection.Down))
                    {
                        return true;
                    }
                    return false;
                }

                if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Horizontal ||
                    bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Horizontal) &&
                    (direction == ObjectDirection.Left || direction == ObjectDirection.Right))
                {
                    return true;
                }

                if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Vertical ||
                    bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Vertical) &&
                    (direction == ObjectDirection.Up || direction == ObjectDirection.Down))
                {
                    return true;
                }
            }
        }
        return false;
    }
}