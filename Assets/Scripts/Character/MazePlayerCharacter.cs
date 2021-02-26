using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class MazePlayerCharacter : PlayerCharacter
{
    private MazeCharacterManager _characterManager;
    
    public bool HasReachedExit = false;
    public event Action PlayerExitsEvent;
    public event Action PlayerCaughtEvent;

    public int TimesCaught = 0;

    public override void Awake()
    {
        base.Awake();

        _characterManager = CharacterManager.Instance as MazeCharacterManager;

        if (_characterManager == null) return;

        _characterManager.Players.Add(PlayerNumber, this);
    }

    public override void Start()
    {
        base.Start();

        PlayerExitsEvent += OnPlayerExit;
        PlayerCaughtEvent += OnPlayerCaught;

        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer
    || PhotonView.IsMine)
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
        if (GameRules.GamePlayerType == GamePlayerType.Multiplayer && !PhotonView.IsMine) return;

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

        CharacterBody.SetActive(false);
    }

    public void OnPlayerCaught()
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            PunRPCCaughtByEnemy();
        }
        else
        {
            PhotonView.RPC("PunRPCCaughtByEnemy", RpcTarget.All);
        }

        float freezeTime = 2.0f;

        IEnumerator coroutine = this.RespawnCharacter(this, freezeTime);
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
}