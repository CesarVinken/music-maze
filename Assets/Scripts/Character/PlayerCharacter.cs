using UnityEngine;
using Photon.Pun;
using System.Collections;
using CharacterType;
using System.Collections.Generic;

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
    public int PlayerNoInGame = 1;  // in multiplayer on multiple computers there can be a player 1 and player 2, while both use their Player1 keyboard input
    public PlayerNumber PlayerNumber = PlayerNumber.Player1;
    public Tile LastTile;
    public string Name;

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
        int playerCount = GameManager.Instance.CharacterManager.GetPlayerCount();

        // TODO: character type should not depend on Player Number, but on which character the player chose when starting the game
        AssignCharacterType(); // relies on player number
        SetPlayerName(); // relies on player type

        if (PersistentGameManager.CurrentPlatform == Platform.PC)
        {
            if (playerCount == 1)
            {
                KeyboardInput = KeyboardInput.Player1;
                PlayerNoInGame = 1; // Q: Does it make sense to keep variable PlayerNoInGame?
            }
            else if (playerCount == 2)
            {
                KeyboardInput = KeyboardInput.Player2;
                PlayerNoInGame = 2;
            }
            else
            {
                Logger.Warning("There are {0} players in the level. There can be max 2 players in a level", playerCount);
            }
        }

        base.Awake();

        _pointerPresserTimer = _pointerPresserDelay;        
    }

    public virtual void Start()
    {
        _characterPath.CharacterReachesTarget += OnTargetReached;
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

        if (_characterPath.reachedEndOfPath && IsMoving)
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
        if (angle <= -135)  // go down
        {
            newLocomotionTarget = new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y - 1);
        }
        else if (angle <= -45) // go left
        {
            newLocomotionTarget = new GridLocation(CurrentGridLocation.X - 1, CurrentGridLocation.Y);
        }
        else if (angle <= 45) // go up
        {
            newLocomotionTarget = new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y + 1);
        }
        else if (angle <= 135) // go right
        {
            newLocomotionTarget = new GridLocation(CurrentGridLocation.X + 1, CurrentGridLocation.Y);
        }
        else // go down
        {
            newLocomotionTarget = new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y - 1);
        }

        SetPointerLocomotionTarget(GridLocation.GridToVector(newLocomotionTarget));
    }

    private void SetPointerLocomotionTarget(Vector2 target)
    {
        GridLocation targetGridLocation = GridLocation.FindClosestGridTile(target);
        //Logger.Log("The closest grid tile is {0},{1}", targetGridLocation.X, targetGridLocation.Y);
        if (!ValidateTarget(targetGridLocation)) return;

        Vector2 gridVectorTarget = GridLocation.GridToVector(targetGridLocation);

        GridLocation currentGridLocation = GridLocation.FindClosestGridTile(transform.position);

        if (currentGridLocation.X == targetGridLocation.X && currentGridLocation.Y == targetGridLocation.Y) return;

        if (!_animationHandler.InLocomotion)
            _animationHandler.SetLocomotion(true);

        if (!_characterPath.canSearch)
        {
            _characterPath.isStopped = false;
            _characterPath.canSearch = true;
        }

        Vector3 newDestinationTarget = SetNewLocomotionTarget(gridVectorTarget);
        IsCalculatingPath = true;
        _seeker.StartPath(transform.position, newDestinationTarget, _characterPath.OnPathCalculated);
    }

    private void HandleKeyboardInput()
    {
        if (PlayerNoInGame == 1)
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

        if (PlayerNoInGame == 2)
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
                TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X, currentGridLocation.Y - 1), ObjectDirection.Down);
                break;
            case ObjectDirection.Left:
                TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X - 1, currentGridLocation.Y), ObjectDirection.Left);
                break;
            case ObjectDirection.Right:
                TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X + 1, currentGridLocation.Y), ObjectDirection.Right);
                break;
            case ObjectDirection.Up:
                TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X, currentGridLocation.Y + 1), ObjectDirection.Up);
                break;
            default:
                Logger.Warning("Unhandled locomotion direction {0}", direction);
                return;
        }
        if (!ValidateTarget(TargetGridLocation.TargetGridLocation))
        {
            // This prevents the character from displaying locomotion animation when walking into an unwalkable tile
            _animationHandler.SetLocomotion(false);
            return;
        }
        //Logger.Warning("Start path!");
        Vector3 newDestinationTarget = SetNewLocomotionTarget(GridLocation.GridToVector(TargetGridLocation.TargetGridLocation));
        IsCalculatingPath = true;

        _seeker.StartPath(transform.position, newDestinationTarget, _characterPath.OnPathCalculated);

        if (!_animationHandler.InLocomotion)
            _animationHandler.SetLocomotion(true);
    }

    public void UpdateCurrentGridLocation(GridLocation gridLocation)
    {
        CurrentGridLocation = gridLocation;
    }

    private bool IsPressingMovementKey()
    {
        if (PlayerNoInGame == 1)
        {
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Up)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Right)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Down)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Left)) return true;
        }
        if (PlayerNoInGame == 2)
        {
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Up)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Right)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Down)) return true;
            if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Left)) return true;
        }

        return false;
    }

    public void OnTargetReached()
    {
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
        Logger.Log($"We set the player number to {PlayerNumber}");
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
        Logger.Warning($"Set name of character to {Name}");
    }
}
