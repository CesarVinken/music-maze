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

    private List<TileArea> _tileAreas = new List<TileArea>();
    private List<Tile> _accessibleTiles = new List<Tile>();

    public void SetTileAreas(List<TileArea> tileAreas)
    {
        _tileAreas = tileAreas;
    }

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

        AssignAccessibleTiles();
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

    public void SetSpawnpoint(Character character, EnemySpawnpoint spawnpoint)
    {
        character.Spawnpoint = spawnpoint;
        character.StartingPosition = spawnpoint.GridLocation;
    }

    public void SetTileAreas()
    {
        EnemySpawnpoint spawnpoint = Spawnpoint as EnemySpawnpoint;
        if (spawnpoint == null)
        {
            Logger.Error("spawnpoint is not an enemy spawnpoint");
        }

        _tileAreas.Clear();

        for (int i = 0; i < spawnpoint.TileAreas.Count; i++)
        {
            _tileAreas.Add(spawnpoint.TileAreas[i]);
        }
    }

    private void SetNextTarget()
    {
        IsCalculatingPath = true;

        // EVALUATE:: 
        // maybe the random choice to follow the player or not should happen BEFORE checking if the players are reachable

        // Check if any of the players is in an area that can be reached by this enemy
        Dictionary<PlayerNumber, MazePlayerCharacter> playerCharacters = GameManager.Instance.CharacterManager.GetPlayers< MazePlayerCharacter>();

        List<PlayerNumber> reachablePlayers = new List<PlayerNumber>();

        foreach (KeyValuePair<PlayerNumber, MazePlayerCharacter> item in playerCharacters)
        {
            GridLocation currentPlayerLocation = item.Value.CurrentGridLocation;
            Tile currentPlayerLocationTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[currentPlayerLocation];

            if (_accessibleTiles.Contains(currentPlayerLocationTile))
            {
                reachablePlayers.Add(item.Key);
            }
        }
        //Logger.Log($"Number of reachable players is: {reachablePlayers.Count}");
        if(reachablePlayers.Count == 0)
        {
            SetRandomTarget();
            return;
        }

        int RandomMax = 20;
        int RandomNumber = UnityEngine.Random.Range(1, RandomMax + 1);

        float targetPlayerChance = 0.30f; // 30% chance to go chase a player

        if (RandomNumber <= targetPlayerChance * RandomMax) 
        {
            TargetPlayer(reachablePlayers);
        }
        else
        {
            SetRandomTarget();
        }
    }

    private void TargetPlayer(List<PlayerNumber> reachablePlayers)
    {
        //Randomly pick one of the players
        int randomNumber = UnityEngine.Random.Range(0, reachablePlayers.Count);

        PlayerCharacter randomPlayer = _characterManager.GetPlayerCharacter<MazePlayerCharacter>(reachablePlayers[randomNumber]);

        IsCalculatingPath = true;
        GridLocation playerGridLocation = randomPlayer.CurrentGridLocation;
        Logger.Log($"Set Player {randomPlayer.PlayerNumber} target ({playerGridLocation.X}, {playerGridLocation.Y} )");
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
            //Logger.Log($"Current Tile: {CurrentGridLocation.X} {CurrentGridLocation.Y}. To remove: {PathToTarget[0].Tile.GridLocation.X}, {PathToTarget[0].Tile.GridLocation.Y}");
             PathToTarget.RemoveAt(0);

            SetHasCalculatedTarget(true);

            if (!_animationHandler.InLocomotion)
                _animationHandler.SetLocomotion(true);
        }
    }

    private void AssignAccessibleTiles()
    {
        List<Tile> allTiles = MazeLevelGameplayManager.Instance.GetTiles();
        List<Tile> accessibleTiles = new List<Tile>(); ;

        for (int i = 0; i < allTiles.Count; i++)
        {
            Tile tile = allTiles[i];

            if (!tile.Walkable) continue;

            if (tile.TryGetPlayerOnly()) continue;

            // if no tile areas are assigned to an enemy, pick random from ALL tiles
            if (_tileAreas.Count == 0)
            {
                accessibleTiles.Add(tile);
            }
            else
            {
                for (int j = 0; j < _tileAreas.Count; j++)
                {
                    List<TileArea> tileAreasOnTile = tile.GetTileAreas();
                    if (tileAreasOnTile.Contains(_tileAreas[j]))
                    {
                        accessibleTiles.Add(tile);
                    }
                }
            }
        }
        _accessibleTiles = accessibleTiles;
    }

    private Tile GetRandomTileTarget()
    {
        int random = UnityEngine.Random.Range(0, _accessibleTiles.Count);
        return _accessibleTiles[random];
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
