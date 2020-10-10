using UnityEngine;
using Photon.Pun;
using Pathfinding;

public class PlayerCharacter : Character
{
    public KeyboardInput KeyboardInput = KeyboardInput.None;
    public int PlayerNoInGame = 1;  // in multiplayer on multiple computers there can be a player 1 and player 2, while both use their Player1 keyboard input
    public bool HasCalculatedPath = false;

    private Vector2 _mobileFingerDownPosition;
    [SerializeField] private GameObject _selectionIndicatorPrefab;
    [SerializeField] private GameObject _selectionIndicatorGO;

    public void Awake()
    {
        Guard.CheckIsNull(_selectionIndicatorPrefab, "Could not find _selectionIndicatorPrefab");

        base.Awake();

        gameObject.name = _photonView.Owner == null ? "Player 1" : _photonView.Owner?.NickName;
    }

    public void Start()
    {
        if (GameManager.Instance.GameType == GameType.SinglePlayer
            || _photonView.IsMine)
        {
            _selectionIndicatorGO = Instantiate(_selectionIndicatorPrefab, SceneObjectManager.Instance.CharactersGO);
            _selectionIndicatorGO.GetComponent<SelectionIndicator>().SelectedObject = transform;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyCharacter enemy = collision.gameObject.GetComponent<EnemyCharacter>();
        if (enemy != null)
        {
            Logger.Warning("Collider enter player!");
            enemy.CatchPlayer(this);
        }
    }

    public void Update()
    {
        if (IsFrozen) return;

        if (Console.Instance && Console.Instance.ConsoleState != ConsoleState.Closed)
            return;

        if (GameManager.Instance.GameType == GameType.SinglePlayer
            || _photonView.IsMine)
        {
            if (GameManager.Instance.CurrentPlatform == Platform.PC)
                CheckKeyboardInput();

            CheckPointerInput();
        }

        if (HasCalculatedTarget)
        {
            MoveCharacter();
        }

        if (_characterPath.reachedEndOfPath)
        {
            ReachTarget();
        }
    }

    private void CheckPointerInput()
    {
        if (GameManager.Instance.CurrentPlatform == Platform.PC)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SetPointerLocomotinTarget(target);
            }
        }
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _mobileFingerDownPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                Vector2 releaseDistance = _mobileFingerDownPosition - touch.position;

                if ((releaseDistance.x < 12 && releaseDistance.x > -12) && (releaseDistance.y < 12 && releaseDistance.y > -12))  // tapping start position is roughly the same as the release position
                {
                    Vector2 target = Camera.main.ScreenToWorldPoint(touch.position);
                    SetPointerLocomotinTarget(target);
                }
                else
                {
                    Logger.Log("{0} and {1}", _mobileFingerDownPosition, touch.position);
                }
            }
        }
    }

    private void SetPointerLocomotinTarget(Vector2 target)
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
        _seeker.StartPath(transform.position, newDestinationTarget, _characterPath.OnPathCalculated);
    }

    private void CheckKeyboardInput()
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

        if (!_characterPath.canSearch)
        {
            _characterPath.isStopped = false;
            _characterPath.canSearch = true;
        }

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
        if (!ValidateTarget(targetGridLocation)) return;

        Vector3 newDestinationTarget = SetNewLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
        _seeker.StartPath(transform.position, newDestinationTarget, _characterPath.OnPathCalculated);

        if (!_animationHandler.InLocomotion)
            _animationHandler.SetLocomotion(true);
        Logger.Log("CharacterPath.transform.position {0},{1}", _characterPath.transform.position.x, _characterPath.transform.position.y);
        Logger.Log("newDestinationTarget {0},{1}", newDestinationTarget.x, newDestinationTarget.y);
    }

    public void ReachTarget()
    { 
        SetHasCalculatedTarget(false);
        _animationHandler.SetLocomotion(false);
    }
}
