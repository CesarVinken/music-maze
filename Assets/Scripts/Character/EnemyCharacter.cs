using CharacterType;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    private MazeCharacterManager _characterManager;
    private bool _isInitialised = false;
    private bool _isIdling = false;

    private ICharacter _enemyType = null;

    public override void Awake()
    {
        base.Awake();

        _characterManager = GameManager.Instance.CharacterManager as MazeCharacterManager;

        if (_characterManager == null) return;

        _enemyType = new EvilViolin();
        _characterManager.Enemies.Add(this);
    }

    public void Start()
    {
        _pathfinding = new Pathfinding(this);

        GameManager.Instance.CompleteMazeLevelEvent += OnMazeLevelCompleted;

        SetCharacterType(_enemyType);

        CurrentGridLocation = StartingPosition;

        _isInitialised = true;
    }

    public void Update()
    {
        if (EditorManager.InEditor) return;

        if (IsFrozen) return;

        if (!_isInitialised) return;

        if (IsCalculatingPath) return;

        if (!HasCalculatedTarget &&
            !_isIdling &&
            (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer ||
            PhotonView.IsMine))
        {
            SetNextTarget();
        }

        if (HasCalculatedTarget)
        {
            MoveCharacter();
        }
    }

    private void SetNextTarget()
    {
        IsCalculatingPath = true;
        int RandomMax = 20;
        int RandomNumber = UnityEngine.Random.Range(1, RandomMax + 1);
        float targetPlayerChance = 0.25f; // 25% chance to go chase a player

        if (RandomNumber <= targetPlayerChance * RandomMax) 
        {
            TargetPlayer();
        }
        else
        {
            SetRandomTarget();
        }
    }

    private void TargetPlayer()
    {
        //Randomly pick one of the players
        int randomNumber = UnityEngine.Random.Range(0, _characterManager.GetPlayers<MazePlayerCharacter>().Count);

        PlayerCharacter randomPlayer = randomNumber == 0 ?
            _characterManager.GetPlayerCharacter<MazePlayerCharacter>(PlayerNumber.Player1):
            _characterManager.GetPlayerCharacter<MazePlayerCharacter>(PlayerNumber.Player1);

        IsCalculatingPath = true;
        GridLocation playerGridLocation = randomPlayer.CurrentGridLocation;
        Logger.Log($"Set Player target ({playerGridLocation.X}, {playerGridLocation.Y} )");
        PathToTarget = _pathfinding.FindNodePath(CurrentGridLocation, playerGridLocation);

        IsCalculatingPath = false;
        
        if (PathToTarget.Count == 0) // Player cannot be reached. For example, the player is on a playerOnly location.
        {
            StartCoroutine(SpendIdleTimeCoroutine());
        }
        else
        {
            PathToTarget.RemoveAt(0);
            SetHasCalculatedTarget(true);

            if (!_animationHandler.InLocomotion)
                _animationHandler.SetLocomotion(true);
        }

        Logger.Log(Logger.Pathfinding, $"The enemy {gameObject.name} is now going to the location of player {randomPlayer.gameObject.name} at {randomPlayer.CurrentGridLocation.X},{randomPlayer.CurrentGridLocation.Y}");
    }

    private void SetRandomTarget()
    {
        IsCalculatingPath = true;
        GridLocation randomGridLocation = GetRandomTileTarget().GridLocation;
        Logger.Log($"Set random target ({randomGridLocation.X}, {randomGridLocation.Y} )");
        PathToTarget = _pathfinding.FindNodePath(CurrentGridLocation, randomGridLocation);

        IsCalculatingPath = false;
        if (PathToTarget.Count == 0)
        {
            StartCoroutine(SpendIdleTimeCoroutine());
        }
        else
        {
            Logger.Log($"Current Tile: {CurrentGridLocation.X} {CurrentGridLocation.Y}. To remove: {PathToTarget[0].Tile.GridLocation.X}, {PathToTarget[0].Tile.GridLocation.Y}");
             PathToTarget.RemoveAt(0);

            SetHasCalculatedTarget(true);

            if (!_animationHandler.InLocomotion)
                _animationHandler.SetLocomotion(true);
        }
    }

    private Tile GetRandomTileTarget()
    {
        List<Tile> allTiles = MazeLevelGameplayManager.Instance.GetTiles();
        List<Tile> tilesForRandomPick = new List<Tile>();
        
        for (int i = 0; i < allTiles.Count; i++)
        {
            Tile tile = allTiles[i];

            if (!tile.Walkable) continue;

            if (tile.TryGetPlayerOnly()) continue;

            tilesForRandomPick.Add(tile);
        }

        int random = UnityEngine.Random.Range(0, tilesForRandomPick.Count);
        return tilesForRandomPick[random];
    }

    private IEnumerator SpendIdleTimeCoroutine()
    {
        _isIdling = true;
        _animationHandler.SetLocomotion(false);

        yield return new WaitForSeconds(4f);

        _isIdling = false;
        SetNextTarget();
    }

    public override void OnTargetReached()
    {
        Vector3 roundedVectorPosition = new Vector3((float)Math.Round(transform.position.x - GridLocation.OffsetToTileMiddle), (float)Math.Round(transform.position.y - GridLocation.OffsetToTileMiddle));
        transform.position = new Vector3(roundedVectorPosition.x + GridLocation.OffsetToTileMiddle, roundedVectorPosition.y + GridLocation.OffsetToTileMiddle, 0);

        _animationHandler.SetLocomotion(false);
        SetHasCalculatedTarget(false);
    }

    public void OnMazeLevelCompleted()
    {
        //TODO: when maze level is completed, do not stop locomotion, but switch, if it exists, to an enemy idle animation
        FreezeCharacter();
    }
}
