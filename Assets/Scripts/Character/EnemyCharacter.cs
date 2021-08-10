using CharacterType;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCharacter : Character
{
    public string Id;
    private MazeCharacterManager _characterManager;
    private bool _isInitialised = false;
    private bool _isIdling = false;

    private ICharacter _enemyType = null;
    private ChasingState _chasingState = ChasingState.Loitering;

    private List<TileArea> _tileAreas = new List<TileArea>();
    private List<Tile> _accessibleTiles = new List<Tile>();
    private PlayerAsTarget _playerAsTarget = null;

    const float STARTLED_FULL_STRENGTH_LIFETIME = 5f;
    const float BLINKING_SPEED = 0.4f;
    const float BLINKING_LIFETIME = 5f;

    public ChasingState ChasingState { get => _chasingState; private set => _chasingState = value; }

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

        _playerAsTarget = null;
    }

    public void Start()
    {
        _pathfinding = new Pathfinding(this);

        MazeLevelGameplayManager.Instance.CompleteMazeLevelEvent += OnMazeLevelCompleted;

        SetCharacterType(_enemyType);
        SetCurrentGridLocation(StartingPosition);

        _isInitialised = true;

        AssignAccessibleTiles();

        SetChasingState(ChasingState.Active);
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

    public void SetChasingState(ChasingState chasingState)
    {
        ChasingState = chasingState;
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

            // a player is not reachable if they already finished
            if (item.Value.HasReachedExit) continue;

            // A player is not reachable if it is on the same tile as the enemy (for example, after a player was just caught)
            if (CurrentGridLocation.X == currentPlayerLocation.X && CurrentGridLocation.Y == currentPlayerLocation.Y) continue;


            if (_accessibleTiles.Contains(currentPlayerLocationTile))
            {
                reachablePlayers.Add(item.Key);
            }

            // We had a player as target, but the player is no longer reachable. In that case, empty _playerAsTarget so that it does not affect choises in regards to the next target chodie
            if (_playerAsTarget != null && !reachablePlayers.Contains(_playerAsTarget.TargettedPlayer.PlayerNumber)) {
                _playerAsTarget = null;
            }
        }
        //Logger.Log($"Number of reachable players is: {reachablePlayers.Count}");
        if(ChasingState == ChasingState.Startled ||
            reachablePlayers.Count == 0)
        {
            SetRandomTarget();
            return;
        }

        int randomOutOfHundred = UnityEngine.Random.Range(1, 101);

        if (_playerAsTarget != null)
        {
            int targetPlayerAgainChance = 75; // if we just chased a player, 75% chance to go chase a player again!
            if (randomOutOfHundred <= targetPlayerAgainChance)
            {
                // If we get herem we established earlier that the targetPlayer is still reachable. In that case, only offer that player as follow up target
                List<PlayerNumber> currentlyTargettedPlayer = new List<PlayerNumber> { _playerAsTarget.TargettedPlayer.PlayerNumber };
                TargetPlayer(currentlyTargettedPlayer);
                //Logger.Warning("Target player AGAIN");
            }
            else
            {
                SetRandomTarget();
            }
        }
        else
        {
            int targetPlayerChance = 30; // 30% chance to go chase a player

            if (randomOutOfHundred <= targetPlayerChance)
            {
                TargetPlayer(reachablePlayers);
                //Logger.Warning("Target player FOR THE FIRST TIME!");
            }
            else
            {
                SetRandomTarget();
            }
        }
    }

    public void BecomeStartled()
    {
        StartCoroutine(StartleCoroutine());
    }

    private IEnumerator StartleCoroutine()
    {
        SetChasingState(ChasingState.Startled);

        GameObject startledSpinnerPrefab = MazeLevelGameplayManager.Instance.GetEffectAnimationPrefab(AnimationEffect.StartledSpinner);
        GameObject startledSpinningEffectGO = GameObject.Instantiate(startledSpinnerPrefab, transform);
        Vector3 spawnPosition = new Vector3(startledSpinningEffectGO.transform.parent.position.x - 0.5f, startledSpinningEffectGO.transform.parent.position.y + 0.16f, startledSpinningEffectGO.transform.parent.position.z);
        startledSpinningEffectGO.transform.position = spawnPosition;

        EffectController startledSpinningEffectController = startledSpinningEffectGO.GetComponent<EffectController>();
        startledSpinningEffectController.PlayEffectLoop(AnimationEffect.StartledSpinner);
        SpriteRenderer spriteRenderer = startledSpinningEffectController.SpriteRenderer;

        yield return new WaitForSeconds(STARTLED_FULL_STRENGTH_LIFETIME);

        float blinkingTimer = 0;
        float alphaValue = 0;
        
        while (blinkingTimer <= BLINKING_LIFETIME)
        {
            alphaValue = spriteRenderer.color.a == 0 ? 1 : 0;
            Color changedAlphaColour = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alphaValue);
            spriteRenderer.color = changedAlphaColour;

            yield return new WaitForSeconds(BLINKING_SPEED);
            blinkingTimer++;
        }

        Destroy(startledSpinningEffectController);
        Destroy(startledSpinningEffectGO);
        SetChasingState(ChasingState.Active); //TODO: if in the meantime the level finishes, interrupt coroutine and set player to loitering
    }

    private void TargetPlayer(List<PlayerNumber> reachablePlayers)
    {
        //Randomly pick one of the players
        int randomNumber = UnityEngine.Random.Range(0, reachablePlayers.Count);

        PlayerCharacter randomPlayer = _characterManager.GetPlayerCharacter<MazePlayerCharacter>(reachablePlayers[randomNumber]);

        IsCalculatingPath = true;
        GridLocation playerGridLocation = randomPlayer.CurrentGridLocation;
        Logger.Log($"Set target for enemy {gameObject.name} to Player {randomPlayer.PlayerNumber} at ({playerGridLocation.X}, {playerGridLocation.Y} )");
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

        _playerAsTarget = new PlayerAsTarget(randomPlayer, playerGridLocation);

        Logger.Log(Logger.Pathfinding, $"The enemy {gameObject.name} is now going to the location of player {randomPlayer.gameObject.name} at {randomPlayer.CurrentGridLocation.X},{randomPlayer.CurrentGridLocation.Y}");
    }

    private void SetRandomTarget()
    {
        _playerAsTarget = null;
        IsCalculatingPath = true;
        GridLocation randomGridLocation = GetRandomTileTarget().GridLocation;

        Logger.Log($"current location of {gameObject.name} is ({CurrentGridLocation.X}, {CurrentGridLocation.Y} ). Set random target ({randomGridLocation.X}, {randomGridLocation.Y} )  ");

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
        List<Tile> possibleRandomTargets = new List<Tile>(_accessibleTiles.ToList());
        
        Tile currentEnemyLocationTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[CurrentGridLocation];

        possibleRandomTargets.Remove(currentEnemyLocationTile);

        int random = UnityEngine.Random.Range(0, possibleRandomTargets.Count);
        return possibleRandomTargets[random];
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
        //Logger.Log($"{gameObject.name} reached target. Current grid location is {CurrentGridLocation.X}, {CurrentGridLocation.Y}. Rounded position is {roundedVectorPosition.x}, {roundedVectorPosition.y}");

        SetHasCalculatedTarget(false);
    }

    public void OnMazeLevelCompleted()
    {
        FreezeCharacter();
        _animationHandler.SetLocomotion(false);
    }
}
