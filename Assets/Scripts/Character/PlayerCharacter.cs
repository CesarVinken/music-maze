using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerCharacter : Character
{
    public KeyboardInput KeyboardInput = KeyboardInput.None;
    public int PlayerNoInGame = 1;  // in multiplayer on multiple computers there can be a player 1 and player 2, while both use their Player1 keyboard input
    public PlayerNumber PlayerNumber = PlayerNumber.Player1;
    public bool HasCalculatedPath = false;
    public bool HasReachedExit = false;
    public Tile LastTile;
    private PathDrawer _pathDrawer;

    private Vector2 _mobileFingerDownPosition;
    [SerializeField] private GameObject _selectionIndicatorPrefab = null;
    [SerializeField] private GameObject _selectionIndicatorGO = null;

    public GridLocation CurrentGridLocation;
    private List<GridLocation> _drawnPath = new List<GridLocation>();

    public void Awake()
    {
        Guard.CheckIsNull(_selectionIndicatorPrefab, "_selectionIndicatorPrefab", gameObject);

        base.Awake();

        gameObject.name = PhotonView.Owner == null ? "Player 1" : PhotonView.Owner?.NickName;

        if(GameManager.Instance.GameType == GameType.Multiplayer)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                if (PhotonView.IsMine)
                    PlayerNumber = PlayerNumber.Player1;
                else
                    PlayerNumber = PlayerNumber.Player2;
            } else
            {
                if (PhotonView.IsMine)
                    PlayerNumber = PlayerNumber.Player2;
                else
                    PlayerNumber = PlayerNumber.Player1;
            }
        }
    }

    public void Start()
    {
        if (GameManager.Instance.GameType == GameType.SinglePlayer
            || PhotonView.IsMine)
        {
            _selectionIndicatorGO = Instantiate(_selectionIndicatorPrefab, SceneObjectManager.Instance.CharactersGO);
            _selectionIndicatorGO.GetComponent<SelectionIndicator>().SelectedObject = transform;
            SceneObjectManager.Instance.SceneObjects.Add(_selectionIndicatorGO);

            _pathDrawer = CameraController.Instance.PathDrawer;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyCharacter enemy = collision.gameObject.GetComponent<EnemyCharacter>();
        if (enemy != null)
        {
            if(GameManager.Instance.GameType == GameType.SinglePlayer)
            {
                CaughtByEnemy();
            }
            else
            {
                PhotonView.RPC("CaughtByEnemy", RpcTarget.All);
            }

            PathDrawer.DisablePathDrawer(_pathDrawer);
        }
    }

    [PunRPC]
    public void CaughtByEnemy()
    {
        float freezeTime = 2.0f;

        IEnumerator coroutine = this.RespawnCharacter(this, freezeTime);
        StartCoroutine(coroutine);
    }

    public void Update()
    {
        if (IsFrozen) return;

        if (Console.Instance && Console.Instance.ConsoleState != ConsoleState.Closed)
            return;

        if (GameManager.Instance.GameType == GameType.SinglePlayer
            || PhotonView.IsMine)
        {
            if (GameManager.Instance.CurrentPlatform == Platform.PC)
                HandleKeyboardInput();

            // To be replaced with pathdrawing system.
            //CheckPointerInputOld();
            CheckPointerInput();

            if (HasCalculatedTarget)
            {
                MoveCharacter();
            }
        }
        if (_characterPath.reachedEndOfPath && IsMoving)
        {
            Logger.Log("reach target");
            ReachTarget();
        }
    }

    private void CheckPointerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 tempFingerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GridLocation tempGridLocation = GridLocation.FindClosestGridTile(tempFingerPosition);

            if(tempGridLocation.X == CurrentGridLocation.X && tempGridLocation.Y == CurrentGridLocation.Y)
            {
                PathDrawer.EnablePathDrawer(_pathDrawer);
            }
        }

        if (_pathDrawer.IsDrawingPath && Input.GetMouseButtonUp(0))
        {
            _drawnPath = _pathDrawer.GetDrawnPath().ToList();
            Logger.Log("retrieved path length i s {0}", _drawnPath.Count);

            PathDrawer.DisablePathDrawer(_pathDrawer);
            if (_drawnPath.Count > 1)
            {
                _drawnPath.RemoveAt(0);
            }

            SetPointerLocomotionTarget(GridLocation.GridToVector(_drawnPath[0]));
        }
    }

    private void CheckPointerInputOld()
    {
        if (GameManager.Instance.CurrentPlatform == Platform.PC)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SetPointerLocomotionTarget(target);
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
                    SetPointerLocomotionTarget(target);
                }
                else
                {
                    Logger.Log("{0} and {1}", _mobileFingerDownPosition, touch.position);
                }
            }
        }
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
        Logger.Warning("Start path!");
        Vector3 newDestinationTarget = SetNewLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
        IsCalculatingPath = true;

        _seeker.StartPath(transform.position, newDestinationTarget, _characterPath.OnPathCalculated);

        if (!_animationHandler.InLocomotion)
            _animationHandler.SetLocomotion(true);
    }

    public void ReachExit()
    {
        IsFrozen = true;
        HasReachedExit = true;

        CharacterBody.SetActive(false);
    }

    public void UpdateCurrentGridLocation(GridLocation gridLocation)
    {
        CurrentGridLocation = gridLocation;

        //Update the drawn path because it may be influenced by the new grid location.
        //if(_drawnPath.Count > 1)
        //{
        if(_pathDrawer && _pathDrawer.isActiveAndEnabled)
            _pathDrawer.PlayerCurrentGridLocationUpdated(gridLocation);
        //}
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

    public override void ReachTarget()
    {

        if (_drawnPath.Count == 0) // This happens when character was moved through keyboard
        {
            if (!IsPressingMovementKey())
            {
                _animationHandler.SetLocomotion(false);
            }
            SetHasCalculatedTarget(false);
            IsMoving = false;
            //_characterPath.isStopped = true;
            //_characterPath.canMove = false;
            //_characterPath.canSearch = false;
            //_seeker.CancelCurrentPathRequest();
            //_characterPath.
            //_characterPath.SetPath(null);
            //_characterPath.destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            return;
        }

        _drawnPath.RemoveAt(0);
        Logger.Log("_drawn path Count : {0} ", _drawnPath.Count);

        // Reach the end of a drawn path
        if (_drawnPath.Count == 0)
        {
            _animationHandler.SetLocomotion(false);
            SetHasCalculatedTarget(false);
            return;
        }

        // We are in a drawn path
        SetPointerLocomotionTarget(GridLocation.GridToVector(_drawnPath[0]));

        //SetHasCalculatedTarget(false);
        //_animationHandler.SetLocomotion(false);
    }
}
