using UnityEngine;
using Photon.Pun;
using System.Collections;
using CharacterType;

public struct TargetLocation
{
    public GridLocation TargetGridLocation;
    public ObjectDirection TargetDirection;

    public TargetLocation(GridLocation targetGridLocation, ObjectDirection targetDirection)
    {
        TargetGridLocation = targetGridLocation;
        TargetDirection = targetDirection;
    }
}

public class PlayerCharacter : Character
{
    [Space(10)]

    public KeyboardInput KeyboardInput = KeyboardInput.None;
    public PlayerNumber PlayerNumber = PlayerNumber.Player1;
    public Tile LastTile;
    public string Name;
    protected TargetLocation TargetGridLocation;

    [SerializeField] protected GameObject _selectionIndicatorPrefab = null;
    [SerializeField] protected GameObject _selectionIndicatorGO = null;

    protected bool _isPressingPointerForSeconds = false;
    protected float _pointerPresserTimer = 1;
    protected const float _pointerPresserDelay = 0.25f;

    public override void Awake()
    {
        Guard.CheckIsNull(_selectionIndicatorPrefab, "_selectionIndicatorPrefab", gameObject);
        SetPlayerNumber();

        GameManager.Instance.CharacterManager.AddPlayer(PlayerNumber, this); // do here and not in manager.

        // TODO: character type should not depend on Player Number, but on which character the player chose when starting the game
        AssignCharacterType(); // relies on player number
        SetPlayerName(); // relies on player type
        SetPlayerKeyboardInput();

        base.Awake();

        _pointerPresserTimer = 0;        
    }

    public virtual void Start()
    {
        _pathfinding = new Pathfinding(this);
    }

    public virtual void Update()
    {
        if (EditorManager.InEditor) return;

        if (IsFrozen) return;

        if (Console.Instance && Console.Instance.ConsoleState != ConsoleState.Closed)
            return;

        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer ||
            (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && PhotonView.IsMine)
            )
        {
            if (PersistentGameManager.CurrentPlatform == Platform.PC)
                HandleKeyboardInput();

            if (Input.GetMouseButtonUp(0))
            {
                _isPressingPointerForSeconds = false;
                _pointerPresserTimer = 0;

                if (!IsPressingMovementKey() && !HasCalculatedTarget)
                {
                    _animationHandler.SetLocomotion(false);
                    IsMoving = false;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (!HasCalculatedTarget)
                {
                    _animationHandler.SetLocomotion(false);
                    IsMoving = false;
                }
            }

            if (Input.GetMouseButtonDown(0) && _pointerPresserTimer == 0)
            {
                StartCoroutine(RunPointerPresserTimer());
            }

            if (_isPressingPointerForSeconds) //only check after we are pressing for x seconds
            {
                if (!Input.GetMouseButton(0))
                {
                    _isPressingPointerForSeconds = false;
                    _pointerPresserTimer = 0;

                    if (!IsPressingMovementKey())
                    {
                        _animationHandler.SetLocomotion(false);
                    }
                    IsMoving = false;
                }
                else
                {
                    CheckPointerInput();
                }
            }

            if (HasCalculatedTarget)
            {
                MoveCharacter();
            }
        }
        else // Case: Clients in multiplayer network game
        {
            if (HasCalculatedTarget)
            {
                MoveCharacter();
            }
        }

        if (PathToTarget.Count == 0 && IsMoving)
        {
            Logger.Log("player reached target");
            OnTargetReached();
        }
    }

    public void AssignCharacterType()
    {
        switch (PlayerNumber)
        {
            case PlayerNumber.Player1:
                SetCharacterType(new Emmon());
                break;
            case PlayerNumber.Player2:
                SetCharacterType(new Fae());
                break;
            default:
                break;
        }
    }

    private AnimationEffect GetAnimationEffectForPlayerChaught()
    {
        if(_characterType is Emmon)
        {
            return AnimationEffect.EmmonCaught;
        }
        else if(_characterType is Fae)
        {
            return AnimationEffect.FaeCaught;
        }
        Logger.Error($"Could not find animation for characterType {_characterType}");
        return AnimationEffect.EmmonCaught;
    }

    protected IEnumerator RespawnPlayerCharacter(PlayerCharacter character)
    {
        // play caught animation
        AnimationEffect animationEffect = GetAnimationEffectForPlayerChaught();

        GameObject emmonCaughtPrefab = MazeLevelGameplayManager.Instance.GetEffectAnimationPrefab(animationEffect);
        GameObject emmonCaughtPGO = GameObject.Instantiate(emmonCaughtPrefab, SceneObjectManager.Instance.transform);
        Vector3 emmonCaughtSpawnPosition = character.transform.position;
        emmonCaughtPGO.transform.position = emmonCaughtSpawnPosition;

        EffectController emmonCaughtPGOEffectController = emmonCaughtPGO.GetComponent<EffectController>();

        emmonCaughtPGOEffectController.PlayEffect(animationEffect);

        //int blackScreenNo = 0;
        //if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer &&
        //character.PlayerNumber == PlayerNumber.Player2)
        //{
        //    blackScreenNo = 1;
        //}
        //BlackOutSquare blackOutSquare = MainScreenOverlayCanvas.Instance.BlackOutSquares[blackScreenNo];

        //if (blackOutSquare == null)
        //{
        //    Logger.Error($"Could not find blackout square for player {blackScreenNo + 1}");
        //}

        character.FreezeCharacter();
        CharacterBody.SetActive(false); // TODO make character animation for appearing and disappearing of character, rather than turning the GO off and on

        ResetCharacterPosition();

        float waitTime = 1.4f;
        yield return new WaitForSeconds(waitTime);

        //// Screen to black
        //IEnumerator toBlackCoroutine = blackOutSquare.ToBlack();

        //StartCoroutine(toBlackCoroutine);
        //while (blackOutSquare.BlackStatus == BlackStatus.InTransition)
        //{
        //    yield return null;
        //}
        CharacterBody.SetActive(true);

        //Screen back to clear
        //IEnumerator toClearCoroutine = blackOutSquare.ToClear();

        //StartCoroutine(toClearCoroutine);
        //while (blackOutSquare.BlackStatus == BlackStatus.InTransition)
        //{
        //    yield return null;
        //}
        character.UnfreezeCharacter();
    }

    private IEnumerator RunPointerPresserTimer()
    {
        while (_pointerPresserTimer < _pointerPresserDelay)
        {
            yield return null;
            _pointerPresserTimer += Time.deltaTime;
        }

        _isPressingPointerForSeconds = true;
        _pointerPresserTimer = 0;
    }

    private void CheckPointerInput()
    {
        if (HasCalculatedTarget) return;

        Vector2 tempFingerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridLocation closestGridLocation = GridLocation.FindClosestGridTile(tempFingerPosition);

        if (closestGridLocation.X == CurrentGridLocation.X && closestGridLocation.Y == CurrentGridLocation.Y) return;

        GridLocation newLocomotionTarget = CurrentGridLocation;
        Vector2 direction = tempFingerPosition - (Vector2)transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, direction) * -1;

        new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y);
        ObjectDirection moveDirection = ObjectDirection.Right;

        if (angle <= -135)  // go down
        {
            newLocomotionTarget = new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y - 1);
            moveDirection = ObjectDirection.Down;
        }
        else if (angle <= -45) // go left
        {
            newLocomotionTarget = new GridLocation(CurrentGridLocation.X - 1, CurrentGridLocation.Y);
            moveDirection = ObjectDirection.Left;
        }
        else if (angle <= 45) // go up
        {
            newLocomotionTarget = new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y + 1);
            moveDirection = ObjectDirection.Up;
        }
        else if (angle <= 135) // go right
        {
            newLocomotionTarget = new GridLocation(CurrentGridLocation.X + 1, CurrentGridLocation.Y);
            moveDirection = ObjectDirection.Right;
        }
        else // go down
        {
            newLocomotionTarget = new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y - 1);
            moveDirection = ObjectDirection.Down;
        }

        SetPointerLocomotionTarget(GridLocation.GridToVector(newLocomotionTarget), moveDirection);
    }

    private void SetPointerLocomotionTarget(Vector2 target, ObjectDirection moveDirection)
    {
        //Logger.Warning($"SetPointerLocomotionTarget to target {target.x}, {target.y} in the direction {moveDirection}");
        GridLocation targetGridLocation = GridLocation.FindClosestGridTile(target);

        if (!ValidateTarget(new TargetLocation(targetGridLocation, moveDirection))) return;

        Vector2 gridVectorTarget = GridLocation.GridToVector(targetGridLocation);

        if (CurrentGridLocation.X == targetGridLocation.X && CurrentGridLocation.Y == targetGridLocation.Y) return;

        if (!_animationHandler.InLocomotion)
            _animationHandler.SetLocomotion(true);

        IsCalculatingPath = true;

        PathToTarget = _pathfinding.FindNodePath(CurrentGridLocation, targetGridLocation);
        PathToTarget.RemoveAt(0);

        IsCalculatingPath = false;
        SetHasCalculatedTarget(true);
    }

    private void HandleKeyboardInput()
    {
        if (KeyboardInput == KeyboardInput.Player1)
        {
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Up))
            {
                TryStartCharacterMovement(ObjectDirection.Up);
            }
            else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Right))
            {
                TryStartCharacterMovement(ObjectDirection.Right);
            }
            else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Down))
            {
                TryStartCharacterMovement(ObjectDirection.Down);
            }
            else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Left))
            {
                TryStartCharacterMovement(ObjectDirection.Left);
            }
        }
        else if (KeyboardInput == KeyboardInput.Player2)
        {
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Up))
            {
                TryStartCharacterMovement(ObjectDirection.Up);
            }
            else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Right))
            {
                TryStartCharacterMovement(ObjectDirection.Right);
            }
            else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Down))
            {
                TryStartCharacterMovement(ObjectDirection.Down);
            }
            else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Left))
            {
                TryStartCharacterMovement(ObjectDirection.Left);
            }
        }
        else
        {

        }
    }

    public void TryStartCharacterMovement(ObjectDirection direction)
    {
        // check if character is in tile position, if so, start movement in direction.
        if (HasCalculatedTarget)
        {
            // if already in locomotion, it means that we are between tiles and we are moving. Return.
            return;
        }

        if (IsCalculatingPath) return;

        GridLocation currentGridLocation = GridLocation.VectorToGrid(transform.position);

        //Order character to go to another tile
        _animationHandler.SetDirection(direction);

        switch (direction)
        {
            case ObjectDirection.Down:
                TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X, currentGridLocation.Y - 1), direction);
                break;
            case ObjectDirection.Left:
                TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X - 1, currentGridLocation.Y), direction);
                break;
            case ObjectDirection.Right:
                TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X + 1, currentGridLocation.Y), direction);
                break;
            case ObjectDirection.Up:
                TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X, currentGridLocation.Y + 1), direction);
                break;
            default:
                Logger.Warning("Unhandled locomotion direction {0}", direction);
                return;
        }

        if (!ValidateTarget(TargetGridLocation))
        {
            // This prevents the character from displaying locomotion animation when walking into an unwalkable tile
            _animationHandler.SetLocomotion(false);
            return;
        }

        //Logger.Warning("Start path!");
        //Vector3 newDestinationTarget = SetNewLocomotionTarget(GridLocation.GridToVector(TargetGridLocation.TargetGridLocation));
        IsCalculatingPath = true;
        Logger.Log($"TryStartCharacterMovement. {CurrentGridLocation.X},{CurrentGridLocation.Y} to {TargetGridLocation.TargetGridLocation.X}, {TargetGridLocation.TargetGridLocation.Y}");
        PathToTarget = _pathfinding.FindNodePath(CurrentGridLocation, TargetGridLocation.TargetGridLocation);
        PathToTarget.RemoveAt(0);

        //_seeker.StartPath(transform.position, newDestinationTarget, _characterPath.OnPathCalculated);
        IsCalculatingPath = false;
        SetHasCalculatedTarget(true);

        if (!_animationHandler.InLocomotion)
            _animationHandler.SetLocomotion(true);
    }

    private bool IsPressingMovementKey()
    {
        if (KeyboardInput == KeyboardInput.Player1)
        {
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Up)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Right)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Down)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Left)) return true;
        }
        if (KeyboardInput == KeyboardInput.Player2)
        {
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Up)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Right)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Down)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Left)) return true;
        }

        return false;
    }

    public override void OnTargetReached()
    {
        //Logger.Warning("Target reached.");
        SetHasCalculatedTarget(false);

        if (!IsPressingMovementKey() && !_isPressingPointerForSeconds)
        {
            _animationHandler.SetLocomotion(false);
        }
        IsMoving = false;
    }

    // The player number needs to be set from the player character, and not from the manager.
    // The reason is that in a network game, the character that is spawned by the other client will not be spawned here through the manager. However, it will go through the player character's Awake/Start function.
    protected void SetPlayerNumber()
    {
        ICharacterManager characterManager = GameManager.Instance.CharacterManager;

        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
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
            if (characterManager.GetPlayerCount() == 0)
            {
                PlayerNumber = PlayerNumber.Player1;
            }
            else
            {
                PlayerNumber = PlayerNumber.Player2;
            }
        }
        Logger.Log(Logger.Initialisation, $"We set the player number to {PlayerNumber}");
    }

    // Should be here and not in manager.
    private void SetPlayerName()
    {
        ICharacterManager characterManager = GameManager.Instance.CharacterManager;
        
        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
        {
            Name = PhotonView.Owner == null ? "Player 1" : PhotonView.Owner?.NickName;
        }
        else if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            Name = _characterType.ToString().Split('.')[1];
        }
        else // split screen
        {
            int playerCount = characterManager.GetPlayerCount();
            if (playerCount == 1)
            {
                Name = "Player 1";
            }
            else if (playerCount == 2)
            {
                Name = "Player 2";
            }
            else
            {
                Logger.Error($"Unexpected number of players: {playerCount}");
            }
        }

        gameObject.name = Name;
        Logger.Warning($"Set name of {PlayerNumber} character to {Name}");
    }

    public bool ValidateTarget(TargetLocation targetLocation)
    {
        if (GameManager.Instance.CurrentGameLevel.TilesByLocation.TryGetValue(targetLocation.TargetGridLocation, out Tile targetTile))
        {
            ObjectDirection direction = targetLocation.TargetDirection;

            if (targetTile.Walkable)
            {
                Tile currentTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[CurrentGridLocation];
                BridgePiece bridgePieceOnCurrentTile = currentTile.TryGetBridgePiece();
                BridgePiece bridgePieceOnTarget = targetTile.TryGetBridgePiece(); // optimisation: keep bridge locations of the level in a separate list, so we don't have to go over all the tiles in the level

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
                        (direction == ObjectDirection.Left || direction == ObjectDirection.Right))
                    {
                        return true;
                    }

                    if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Vertical &&
                        bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Vertical &&
                        (direction == ObjectDirection.Up || direction == ObjectDirection.Down))
                    {
                        return true;
                    }

                    return false;
                }

                if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Horizontal ||
                    bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Horizontal) &&
                    (direction == ObjectDirection.Left || direction == ObjectDirection.Right))
                {
                    return true;
                }

                if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Vertical ||
                    bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Vertical) &&
                    (direction == ObjectDirection.Up || direction == ObjectDirection.Down))
                {
                    return true;
                }
                return false;
            }
        }
        return false;
    }

    private void SetPlayerKeyboardInput()
    {
        int playerCount = GameManager.Instance.CharacterManager.GetPlayerCount();
        if (PersistentGameManager.CurrentPlatform == Platform.PC)
        {
            if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
            {
                KeyboardInput = KeyboardInput.Player1;
            }
            else if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
            {
                KeyboardInput = KeyboardInput.Player1;
            }
            else if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
            {

                if (playerCount == 1)
                {
                    KeyboardInput = KeyboardInput.Player1;
                }
                else if (playerCount == 2)
                {
                    KeyboardInput = KeyboardInput.Player2;
                }
            }

            else
            {
                Logger.Warning($"There are {playerCount} players in the level. There can be max 2 players in a level");
            }
        }
    }
}
