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

        public override bool ValidateTarget(TargetLocation targetLocation)
        {
            if (!GameManager.Instance.CurrentGameLevel.TilesByLocation.TryGetValue(targetLocation.TargetGridLocation, out Tile targetTile))
            {
                return false;
            }

            Direction direction = targetLocation.TargetDirection;
            Tile currentTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[CurrentGridLocation];
            if (targetTile.Walkable)
            {
                if (ControllingFerry != null)
                {
                    ControllingFerry.TryDestroyControlFerryButton();
                    //move if the targetted tile is a ferry route point
                    if (targetTile.TileMainMaterial.GetType() == typeof(GroundMainMaterial))
                    {
                        ToggleFerryControl(ControllingFerry);
                    }
                    return true;
                }
                else
                {

                    if (!ValidateForBridges(direction, currentTile, targetTile))
                    {
                        return false;
                    }

                    if (!ValidateForFerryRoutes(direction, currentTile, targetTile))
                    {
                        return false;
                    }

                    return true;
                }      
            }
            else if (ControllingFerry)
            {
                List<FerryRoutePoint> ferryRoutePoints = ControllingFerry.FerryRoute.GetFerryRoutePoints();
                if(ferryRoutePoints.Any(point => point.Tile.TileId.Equals(targetTile.TileId)))
                {
                    ControllingFerry.TryDestroyControlFerryButton();
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

        private bool ValidateForFerryRoutes(Direction direction, Tile currentTile, Tile targetTile)
        {
            for (int i = 0; i < Ferry.Ferries.Count; i++)
            {
                GridLocation ferryGridLocation = Ferry.Ferries[i].CurrentLocationTile.GridLocation;
                if (ferryGridLocation.X == CurrentGridLocation.X && ferryGridLocation.Y == CurrentGridLocation.Y)
                {
                    if (targetTile.TileMainMaterial.GetType() == typeof(WaterMainMaterial))
                    {
                        return false;
                    }
                }

            }
            return true;
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
