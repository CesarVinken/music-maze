using CharacterType;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCharacter : Character
{
    private MazeCharacterManager _characterManager;
    private bool _isInitialised = false;
    private GridLocation _currentNodeLocation;

    private ICharacter _enemyType = null;

    public override void Awake()
    {
        base.Awake();
        _characterPath.CharacterReachesTarget += OnTargetReached;

        _characterManager = GameManager.Instance.CharacterManager as MazeCharacterManager;

        if (_characterManager == null) return;

        _enemyType = new EvilViolin();
        _characterManager.Enemies.Add(this);
    }

    public void Start()
    {
        GameManager.Instance.CompleteMazeLevelEvent += OnMazeLevelCompleted;

        SetCharacterType(_enemyType);

        _isInitialised = true;
    }

    public void Update()
    {
        if (EditorManager.InEditor) return;

        if (IsFrozen) return;

        if (!_isInitialised) return;

        if (IsCalculatingPath) return;

        if (!HasCalculatedTarget &&
            (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer ||
            PhotonView.IsMine))
        {
            SetNextTarget();

            Pathfinding.GraphNode nextNode = _characterPath.GetNextNode();

            if (nextNode != null)
            {
                Vector3 nextNodeVectorPosition = (Vector3)nextNode.position;
                _currentNodeLocation = new GridLocation((int)nextNodeVectorPosition.x, (int)nextNodeVectorPosition.y);
            }
        }

        if (HasCalculatedTarget)
        {
            ////Validate if movement to current target is valid.Eg, should not walk up on a horizontal bridge.
            Pathfinding.GraphNode currentNode = _characterPath.GetCurrentNode();
            Vector3 currentNodeVectorPosition = (Vector3)currentNode.position;
            GridLocation currentNodeGridLocation = GridLocation.VectorToGrid(currentNodeVectorPosition);

            if (currentNodeGridLocation.X != CurrentGridLocation.X || currentNodeGridLocation.Y != CurrentGridLocation.Y) // we have a new current grid location: we moved to a new node!
            {
                CurrentGridLocation = currentNodeGridLocation;
                Logger.Warning($"CurrentGridLocation {CurrentGridLocation.X}, {CurrentGridLocation.Y}");
                Pathfinding.GraphNode nextNode = _characterPath.GetNextNode();

                if (nextNode == null) return;

                Vector3 nextNodeVectorPosition = (Vector3)nextNode.position;
                GridLocation nextNodeGridLocation = new GridLocation((int)nextNodeVectorPosition.x, (int)nextNodeVectorPosition.y);

                ObjectDirection moveDirection = ObjectDirection.Down;
                if (nextNodeGridLocation.X > currentNodeGridLocation.X)
                {
                    moveDirection = ObjectDirection.Right;
                }
                else if (nextNodeGridLocation.X < currentNodeGridLocation.X)
                {
                    moveDirection = ObjectDirection.Left;
                }
                else if (nextNodeGridLocation.Y > currentNodeGridLocation.Y)
                {
                    moveDirection = ObjectDirection.Up;
                }
                else if (nextNodeGridLocation.Y > currentNodeGridLocation.Y)
                {
                    moveDirection = ObjectDirection.Down;
                }

                // if the next node is not a valid target, we need the enemy to go somewhere else.
                if (!ValidateTarget(new TargetLocation(nextNodeGridLocation, moveDirection)))
                {
                    Logger.Warning("Could not validate target. CAnnot cross bridge like that");
                    //SetNextTarget();

                    // force enemy to go change path by going to the place where it already is
                    _seeker.StartPath(transform.position, new Vector2(CurrentGridLocation.X, CurrentGridLocation.Y), _characterPath.OnPathCalculated);
                    return;
                }
            }
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
        int randomNumber = UnityEngine.Random.Range(0, _characterManager.GetPlayers<MazePlayerCharacter>().Count);

        PlayerCharacter randomPlayer = randomNumber == 0 ?
            _characterManager.GetPlayerCharacter<MazePlayerCharacter>(PlayerNumber.Player1):
            _characterManager.GetPlayerCharacter<MazePlayerCharacter>(PlayerNumber.Player1);

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
        List<InGameMazeTile> walkableTiles = MazeLevelGameplayManager.Instance.GetTiles().Where(tile => tile.Walkable).ToList();

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

    public override bool ValidateTarget(TargetLocation targetLocation)
    {
        if (MazeLevelGameplayManager.Instance.Level.TilesByLocation.TryGetValue(targetLocation.TargetGridLocation, out Tile targetTile))
        {
            ObjectDirection direction = targetLocation.TargetDirection;

            if (targetTile.Walkable)
            {
                Tile currentTile = MazeLevelGameplayManager.Instance.Level.TilesByLocation[CurrentGridLocation];
                BridgePiece bridgePieceOnCurrentTile = currentTile.TryGetBridgePiece();
                BridgePiece bridgePieceOnTarget = targetTile.TryGetBridgePiece(); // optimisation: keep bridge locations of the level in a separate list, so we don't have to go over all the tiles in the level
                //Logger.Log($"CURRENT TILE {currentTile.GridLocation.X}, {currentTile.GridLocation.Y}. TARGET TILE {targetTile.GridLocation.X}, {targetTile.GridLocation.Y}");
                // there are no bridges involved
                if (bridgePieceOnCurrentTile == null && bridgePieceOnTarget == null)
                {
                    //Logger.Log("both bridges are null");
                    return true;
                }

                bool isBridgePieceOnCurrentTile = bridgePieceOnCurrentTile != null;
                bool isBridgePieceOnTarget = bridgePieceOnTarget != null;
                Logger.Log($"CURRENT TILE {currentTile.GridLocation.X}, {currentTile.GridLocation.Y}. Bridge? {isBridgePieceOnCurrentTile}. TARGET TILE {targetTile.GridLocation.X}, {targetTile.GridLocation.Y}. Bridge? {isBridgePieceOnTarget}. Direction: {direction}");
                // Make sure we go in the correct bridge direction
                if (bridgePieceOnCurrentTile && bridgePieceOnTarget)
                {
                    //Logger.Log($"Current tile location is a bridge: {bridgePieceOnCurrentTile.Tile.GridLocation.X}, {bridgePieceOnCurrentTile.Tile.GridLocation.Y}. Next bridge tile is at {bridgePieceOnTarget.Tile.GridLocation.X}, {bridgePieceOnTarget.Tile.GridLocation.Y}");
                    if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Horizontal &&
                        bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Horizontal &&
                        (direction == ObjectDirection.Left || direction == ObjectDirection.Right))
                    {
                        Logger.Log("return trye gereee");
                        return true;
                    }

                    if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Vertical &&
                        bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Vertical &&
                        (direction == ObjectDirection.Up || direction == ObjectDirection.Down))
                    {
                        //Logger.Log("return trye hereee");
                        return true;
                    }
                    Logger.Log("not allowed.");
                    return false;
                }

                if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Horizontal ||
                    bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Horizontal) &&
                    (direction == ObjectDirection.Left || direction == ObjectDirection.Right))
                {
                    //Logger.Log("return TRUE gereee");
                    return true;
                }

                if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Vertical ||
                    bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Vertical) &&
                    (direction == ObjectDirection.Up || direction == ObjectDirection.Down))
                {
                    //Logger.Log("return TRUE Hereee");
                    return true;
                }
                //Logger.Log("return false");
                return false;
            }
        }
        return false;
    }
}
