using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayerCharacter : PlayerCharacter
{
    public bool HasReachedExit = false;
    public event Action PlayerExitsEvent;
    public event Action PlayerCaughtEvent;

    public int TimesCaught = 0;

    public override void Awake()
    {
        Dictionary<PlayerNumber, MazePlayerCharacter> players = GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>();

        SetGameObjectName(players);
        SetPlayerNumber(players);

        base.Awake();

        HasReachedExit = false;

        GameManager.Instance.CharacterManager.AddPlayer(PlayerNumber, this);
    }

    public override void Start()
    {
        base.Start();

        PlayerExitsEvent += OnPlayerExit;
        PlayerCaughtEvent += OnPlayerCaught;

        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer || 
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiPlayer ||
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
        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiPlayer && !PhotonView.IsMine) return;

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
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiPlayer)
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

    public override bool ValidateTarget(GridLocation targetGridLocation)
    {
        if (MazeLevelManager.Instance.Level.TilesByLocation.TryGetValue(targetGridLocation, out Tile tile))
        {
            if (tile.Walkable)
                return true;
        }
        return false;
    }

    private void SetGameObjectName(Dictionary<PlayerNumber, MazePlayerCharacter> players)
    {
        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiPlayer)
        {
            gameObject.name = PhotonView.Owner == null ? "Player 1" : PhotonView.Owner?.NickName;
        }
        else if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            gameObject.name = CharacterBlueprint.CharacterType.ToString();
        }
        else
        {
            if (players.Count == 0)
            {
                gameObject.name = "Player 1";
            }
            else
            {
                gameObject.name = "Player 2";
            }
        }
    }

    private void SetPlayerNumber(Dictionary<PlayerNumber, MazePlayerCharacter> players)
    {
        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (PhotonView.IsMine)
                    PlayerNumber = PlayerNumber.Player1;
                else
                    PlayerNumber = PlayerNumber.Player2;
            }
            else
            {
                if (PhotonView.IsMine)
                    PlayerNumber = PlayerNumber.Player2;
                else
                    PlayerNumber = PlayerNumber.Player1;
            }
        }
        else if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            PlayerNumber = PlayerNumber.Player1;
        }
        else
        {
            if (players.Count == 0)
            {
                PlayerNumber = PlayerNumber.Player1;
            }
            else
            {
                PlayerNumber = PlayerNumber.Player2;
            }
        }
    }
}