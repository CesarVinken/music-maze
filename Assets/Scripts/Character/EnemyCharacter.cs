using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCharacter : Character
{
    private MazeCharacterManager _characterManager;
    private bool _isInitialised = false;

    public override void Awake()
    {
        base.Awake();
        _characterPath.CharacterReachesTarget += OnTargetReached;

        _characterManager = CharacterManager.Instance as MazeCharacterManager;

        if (_characterManager == null) return;

        _characterManager.Enemies.Add(this);
    }

    public void Start()
    {
        GameManager.Instance.CompleteMazeLevelEvent += OnMazeLevelCompleted;

        SetCharacterType(CharacterType.Enemy);

        _isInitialised = true;
    }

    public void Update()
    {
        if (EditorManager.InEditor) return;

        if (IsFrozen) return;

        if (!_isInitialised) return;

        if (IsCalculatingPath) return;

        if (!HasCalculatedTarget &&
            (GameRules.GamePlayerType == GamePlayerType.SinglePlayer
            || PhotonView.IsMine))
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

        if (!_animationHandler.InLocomotion)
            _animationHandler.SetLocomotion(true);
    }

    private void TargetPlayer()
    {
        //Randomly pick one of the players
        int randomNumber = UnityEngine.Random.Range(0, _characterManager.Players.Count);

        PlayerCharacter randomPlayer = randomNumber == 0 ?
            _characterManager.Players[PlayerNumber.Player1] :
            _characterManager.Players[PlayerNumber.Player2];

        Vector3 playerVectorLocation = GridLocation.GridToVector(randomPlayer.CurrentGridLocation);

        // Known issue: The enemy will not plot a path to the actual location of the player, because the player's pathfinding node on the grid is already taken by the player. Thus the enemy will try to move next to the player 

        _seeker.StartPath(transform.position, playerVectorLocation, _characterPath.OnPathCalculated);
        Logger.Log(Logger.Pathfinding, $"The enemy {gameObject.name} is now going to the location of player {randomPlayer.gameObject.name} at {randomPlayer.CurrentGridLocation.X},{randomPlayer.CurrentGridLocation.Y}");
    }

    private void SetRandomTarget()
    {
        Vector3 randomGridVectorLocation = GridLocation.GridToVector(GetRandomTileTarget().GridLocation);
        //Logger.Log("Set new target for enemy: {0},{1}", randomGridVectorLocation.x, randomGridVectorLocation.y);
        _seeker.StartPath(transform.position, randomGridVectorLocation, _characterPath.OnPathCalculated);
    }

    private Tile GetRandomTileTarget()
    {
        //MazeLevelManager mazeLevelManager = GameManager.Instance.
        List<InGameMazeTile> walkableTiles = MazeLevelManager.Instance.GetTiles().Where(tile => tile.Walkable).ToList();

        //TODO remove current tile from walkable tiles
        //TODO pick random
        int random = UnityEngine.Random.Range(0, walkableTiles.Count);
        //Logger.Warning("Picked random:: {0}", random);
        //Logger.Log("We want a random tile. We found: {0}, {1}", walkableTiles[random].GridLocation.X, walkableTiles[random].GridLocation.Y);
        return walkableTiles[random];
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
        FreezeCharacter();
    }
}
