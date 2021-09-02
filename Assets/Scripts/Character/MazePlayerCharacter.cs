﻿using Photon.Pun;
using System;
using System.Collections;
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

                if (currentTile.TryGetAttribute<MusicInstrumentCase>() || currentTile.TryGetAttribute<Sheetmusic>())
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
