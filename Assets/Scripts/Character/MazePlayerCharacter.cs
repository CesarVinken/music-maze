using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character
{
    public class MazePlayerCharacter : PlayerCharacter
    {
        public bool HasReachedExit = false;
        public bool FinishedFirstBonus = false;

        public event Action PlayerExitsEvent;
        public event Action PlayerCaughtEvent;

        public int TimesCaughtByEnemy = 0;
        public int TimesMadeEnemyListenToMusicInstrument = 0;
        public int TimesMadeEnemyReadSheetmusic = 0;

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

            //transform the player's starting tile and surrounding tiles
            InGameMazeTile currentTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[StartingPosition] as InGameMazeTile;

            if (currentTile == null)
            {
                Logger.Error($"Current tile at {StartingPosition.X},{StartingPosition.Y} is null");
            }

            currentTile.TriggerTransformations();
            SetCurrentGridLocation(currentTile.GridLocation);

            MazeLevelGameplayManager.Instance.SetTileMarker(currentTile, this);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !PhotonView.IsMine) return;

            EnemyCharacter enemy = collision.gameObject.GetComponent<EnemyCharacter>();
            if (enemy != null)
            {
                if (enemy.ChasingState == ChasingState.Startled)
                {
                    return;
                }

                Tile currentTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[CurrentGridLocation];

                if (currentTile.TryGetScoreAttribute() != null)
                {
                    return;
                }
                PlayerCaughtEvent?.Invoke();
            }
        }

        public void Exit()
        {
            PlayerExitsEvent?.Invoke();
        }

        public override bool ValidateTarget(Direction direction, Tile targetTile)
        {
            Tile currentTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[CurrentGridLocation];
            if (targetTile.Walkable)
            {
                if (BoardedFerry != null && BoardedFerry.ControllingPlayerCharacter != null && BoardedFerry.ControllingPlayerCharacter == this)
                {
                    BoardedFerry.TryDestroyControlFerryButton();
                    //We are controlling a ferry, but now walking onto a Ground tile. Leave the ferry
                    if (targetTile.TileMainMaterial.GetType() == typeof(GroundMainMaterial))
                    {
                        ToggleFerryControl(BoardedFerry, false);
                    }
                    return true;
                }
                else
                {
                    if (!ValidateForBridges(direction, currentTile, targetTile))
                    {
                        Logger.Log("ValidateForBridges false"); 
                        return false;
                    }

                    // we are on the ferry and someone else is controlling
                    if(GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer &&
                        BoardedFerry != null && (BoardedFerry.ControllingPlayerCharacter == null || BoardedFerry.ControllingPlayerCharacter != this))
                    {                 
                        if (BoardedFerry.TimeSinceLastTileChange < Ferry.MovementOffFerryThresholdTime) return false;
                    }
                    //we are on a ferry but not the controlling player. 
                    if (BoardedFerry != null && BoardedFerry?.ControllingPlayerCharacter != this &&
                        !ValidateForFerryRoutes(direction, currentTile, targetTile))
                    {
                        Logger.Log("ValidateForFerryRoutes false");
                        return false;
                    }

                    for (int i = 0; i < Ferry.Ferries.Count; i++)
                    {
                        if (Ferry.Ferries[i].ControllingPlayerCharacter == null) continue;

                        for (int j = 0; j < Ferry.Ferries[i].PlayersOnFerry.Count; j++)
                        {
                            PlayerCharacter playerOnFerry = Ferry.Ferries[i].PlayersOnFerry[j];
                            // We are a player on a ferry trying to move, but we are not in control.
                            // Issues might occur if the ferry route is only 2 tiles, when the player thinks their current location is still the tile where they boarded the ferry
                            if (playerOnFerry == this)
                            {
                                if (Ferry.Ferries[i].IsMoving)
                                {
                                    playerOnFerry.SetCurrentGridLocation(Ferry.Ferries[i].CurrentLocationTile.GridLocation);
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }      
            }
            else if (BoardedFerry?.ControllingPlayerCharacter != null && BoardedFerry?.ControllingPlayerCharacter == this)
            {
                //move if the targetted tile is a ferry route point
                List<FerryRoutePoint> ferryRoutePoints = BoardedFerry.FerryRoute.GetFerryRoutePoints();
                if(ferryRoutePoints.Any(point => point.Tile.TileId.Equals(targetTile.TileId)))
                {
                    BoardedFerry.TryDestroyControlFerryButton();
                    return true;
                }
            }

            return false;
        }

        private bool ValidateForBridges(Direction direction, Tile currentTile, Tile targetTile)
        {
            BridgePiece bridgePieceOnCurrentTile = currentTile.TryGetAttribute<BridgePiece>();
            BridgePiece bridgePieceOnTarget = targetTile.TryGetAttribute<BridgePiece>(); // optimisation: keep bridge locations of the level in a separate list, so we don't have to go over all the tiles in the level

            // there are no bridges involved
            if (bridgePieceOnCurrentTile == null && bridgePieceOnTarget == null)
            {
                return true;
            }

            // Make sure we go in the correct bridge direction
            if (bridgePieceOnCurrentTile && bridgePieceOnTarget)
            {
                if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Horizontal &&
                    bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Horizontal &&
                    (direction == Direction.Left || direction == Direction.Right))
                {
                    return true;
                }

                if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Vertical &&
                    bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Vertical &&
                    (direction == Direction.Up || direction == Direction.Down))
                {
                    return true;
                }

                return false;
            }

            if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Horizontal ||
                bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Horizontal) &&
                (direction == Direction.Left || direction == Direction.Right))
            {
                return true;
            }

            if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Vertical ||
                bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Vertical) &&
                (direction == Direction.Up || direction == Direction.Down))
            {
                return true;
            }
            return false;
        }

        // If we are on a ferry but not controlling, we should not be able to walk around, except for when we move off or on the ferry
        private bool ValidateForFerryRoutes(Direction direction, Tile currentTile, Tile targetTile)
        {
            if(targetTile.TileMainMaterial == null || targetTile.TileMainMaterial.GetType() != typeof(WaterMainMaterial))
            {
                return true;
            }

            if(targetTile.TryGetAttribute<BridgePiece>())
            {
                return true;
            }

            for (int i = 0; i < Ferry.Ferries.Count; i++)
            {
                GridLocation ferryGridLocation = Ferry.Ferries[i].CurrentLocationTile.GridLocation;

                if (!Ferry.Ferries[i].IsMoving && ferryGridLocation.X == targetTile.GridLocation.X && ferryGridLocation.Y == targetTile.GridLocation.Y)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnPlayerExit()
        {
            FreezeCharacter();
            HasReachedExit = true;

            Logger.Log($"{gameObject.name} reached the exit");

            CharacterBody.SetActive(false);
            _playerCollider.enabled = false;
        }

        public void OnPlayerCaught()
        {
            if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
                GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
            {
                //PunRPCCaughtByEnemy();
                TimesCaughtByEnemy++;

                IEnumerator coroutine = this.RespawnPlayerCharacter(this);
                StartCoroutine(coroutine);
            }
            else
            {
                PhotonView.RPC("PunRPCCaughtByEnemy", RpcTarget.All);
            }


            _isPressingPointerForSeconds = false;
        }

        [PunRPC] // the part all clients need to be informed about
        private void PunRPCCaughtByEnemy()
        {
            TimesCaughtByEnemy++;

            IEnumerator coroutine = this.RespawnPlayerCharacter(this);
            StartCoroutine(coroutine);
        }

        public void MadeEnemyListenToMusicInstrument()
        {
            TimesMadeEnemyListenToMusicInstrument++;
        }

        public void MadeEnemyReadSheetmusic()
        {
            TimesMadeEnemyReadSheetmusic++;
        }
    }
}
