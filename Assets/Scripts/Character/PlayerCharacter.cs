using UnityEngine;
using Photon.Pun;
using System.Collections;
using System;

public class PlayerCharacter : Character
{
    [Space(10)]
    [Header("Player specific")]

    public KeyboardInput KeyboardInput = KeyboardInput.None;
    public int PlayerNoInGame = 1;  // in multiplayer on multiple computers there can be a player 1 and player 2, while both use their Player1 keyboard input
    public PlayerNumber PlayerNumber = PlayerNumber.Player1;
    public bool HasReachedExit = false;
    public Tile LastTile;

    [SerializeField] private GameObject _selectionIndicatorPrefab = null;
    [SerializeField] private GameObject _selectionIndicatorGO = null;

    public GridLocation CurrentGridLocation;
    public int TimesCaught = 0;

    private bool _isPressingPointerForSeconds = false;
    private float _pointerPresserTimer = 1;
    private const float _pointerPresserDelay = 0.25f;

    public event Action PlayerExitsEvent;
    public event Action PlayerCaughtEvent;

    public void Awake()
    {
        Guard.CheckIsNull(_selectionIndicatorPrefab, "_selectionIndicatorPrefab", gameObject);

        base.Awake();

        gameObject.name = PhotonView.Owner == null ? "Player 1" : PhotonView.Owner?.NickName;

        if (GameManager.Instance.GameType == GameType.Multiplayer)
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
        CharacterManager.Instance.MazePlayers.Add(PlayerNumber, this);

        _pointerPresserTimer = _pointerPresserDelay;
    }

    public void Start()
    {
        _characterPath.CharacterReachesTarget += OnTargetReached;
        PlayerExitsEvent += OnPlayerExit;
        PlayerCaughtEvent += OnPlayerCaught;

        if (GameManager.Instance.GameType == GameType.SinglePlayer
            || PhotonView.IsMine)
        {
            _selectionIndicatorGO = Instantiate(_selectionIndicatorPrefab, SceneObjectManager.Instance.CharactersGO);

            SelectionIndicator selectionIndicator = _selectionIndicatorGO.GetComponent<SelectionIndicator>();
            selectionIndicator.Setup(transform, this);

            SceneObjectManager.Instance.SceneObjects.Add(_selectionIndicatorGO);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance.GameType == GameType.Multiplayer && !PhotonView.IsMine) return;

        EnemyCharacter enemy = collision.gameObject.GetComponent<EnemyCharacter>();
        if (enemy != null)
        {
            PlayerCaughtEvent?.Invoke();
        }
    }

    public void Update()
    {
        if (EditorManager.InEditor) return;

        if (IsFrozen) return;

        if (Console.Instance && Console.Instance.ConsoleState != ConsoleState.Closed)
            return;

        if (GameManager.Instance.GameType == GameType.SinglePlayer
            || PhotonView.IsMine)
        {
            if (GameManager.Instance.CurrentPlatform == Platform.PC)
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
        if (_characterPath.reachedEndOfPath && IsMoving)
        {
            Logger.Log("player reached target");
            OnTargetReached();
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
        GridLocation targetGridLocation = currentGridLocation;

        //Order character to go to another tile
        _animationHandler.SetDirection(direction);

        switch (direction)
        {
            case ObjectDirection.Down:
                targetGridLocation = new GridLocation(currentGridLocation.X, currentGridLocation.Y - 1);
                break;
            case ObjectDirection.Left:
                targetGridLocation = new GridLocation(currentGridLocation.X - 1, currentGridLocation.Y);
                break;
            case ObjectDirection.Right:
                targetGridLocation = new GridLocation(currentGridLocation.X + 1, currentGridLocation.Y);
                break;
            case ObjectDirection.Up:
                targetGridLocation = new GridLocation(currentGridLocation.X, currentGridLocation.Y + 1);
                break;
            default:
                Logger.Warning("Unhandled locomotion direction {0}", direction);
                break;
        }
        if (!ValidateTarget(targetGridLocation))
        {
            // This prevents the character from displaying locomotion animation when walking into an unwalkable tile
            _animationHandler.SetLocomotion(false);
            return;
        }
        //Logger.Warning("Start path!");
        Vector3 newDestinationTarget = SetNewLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
        IsCalculatingPath = true;

        _seeker.StartPath(transform.position, newDestinationTarget, _characterPath.OnPathCalculated);

        if (!_animationHandler.InLocomotion)
            _animationHandler.SetLocomotion(true);
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
        //if (!IsPressingMovementKey() && (!_isPressingPointerForSeconds || !Input.GetMouseButton(0)))
        {
            _animationHandler.SetLocomotion(false);
        }
        IsMoving = false;
    }

    public void OnPlayerCaught()
    {
        if (GameManager.Instance.GameType == GameType.SinglePlayer)
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
}
