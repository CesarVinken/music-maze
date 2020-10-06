using UnityEngine;
using Photon.Pun;

public class PlayerCharacter : Character
{
    public KeyboardInput KeyboardInput = KeyboardInput.None;
    public int PlayerNoInGame = 1;  // in multiplayer on multiple computers there can be a player 1 and player 2, while both use their Player1 keyboard input
    private bool _hasTarget = false;
    private Vector2 _mobileFingerDownPosition;

    public void Awake()
    {
        base.Awake();

        gameObject.name = GetComponent<PhotonView>().Owner?.NickName;
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
        base.Update();

        if (Console.Instance && Console.Instance.ConsoleState != ConsoleState.Closed)
            return;

        if (GameManager.Instance.CurrentPlatform == Platform.PC)
            CheckKeyboardInput();

        CheckPointerInput();

        if (_hasTarget)
        {
            MoveCharacter();
        }

        if (CharacterPath.reachedEndOfPath)
        {
            ReachTarget();
            CharacterPath.isStopped = true;
            CharacterPath.canSearch = false;

            // TODO: character gets stuck on pathfinding nodes in between tile centres that should not be stopping points.
        }

        //CharacterPath.isStopped = CharacterPath.reachedEndOfPath;
        //CharacterPath.canSearch = !CharacterPath.isStopped;
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
        Logger.Log("The closest grid tile is {0},{1}", targetGridLocation.X, targetGridLocation.Y);
        if (!ValidateTarget(targetGridLocation)) return;

        Vector2 gridVectorTarget = GridLocation.GridToVector(targetGridLocation);

        GridLocation currentGridLocation = GridLocation.FindClosestGridTile(transform.position);

        if (currentGridLocation.X == targetGridLocation.X && currentGridLocation.Y == targetGridLocation.Y) return;

        SetLocomotionTargetObject(gridVectorTarget);

        if (!AnimationHandler.InLocomotion)
            AnimationHandler.SetLocomotion(true);

        _hasTarget = true;

        if (!CharacterPath.canSearch)
        {
            CharacterPath.isStopped = false;
            CharacterPath.canSearch = true;
        }
        
        CharacterPath.SearchPath();
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
        if (_hasTarget)
        {
            // if already in locomotion, it means that we are between tiles and we are moving. Return.
            return;
        }

        if (!CharacterPath.canSearch)
        {
            CharacterPath.isStopped = false;
            CharacterPath.canSearch = true;
        }

        CharacterPath.SearchPath();
        CharacterPath.enabled = true;

        GridLocation currentGridLocation = GridLocation.VectorToGrid(transform.position);
        GridLocation targetGridLocation;

        //Order character to go to another tile
        AnimationHandler.SetDirection(direction);

        switch (direction)
        {
            case ObjectDirection.Down:
                targetGridLocation = new GridLocation(currentGridLocation.X, currentGridLocation.Y - 1);

                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTargetObject(GridLocation.GridToVector(targetGridLocation));
                break;
            case ObjectDirection.Left:
                targetGridLocation = new GridLocation(currentGridLocation.X - 1, currentGridLocation.Y);
                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTargetObject(GridLocation.GridToVector(targetGridLocation));
                break;
            case ObjectDirection.Right:
                targetGridLocation = new GridLocation(currentGridLocation.X + 1, currentGridLocation.Y);
                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTargetObject(GridLocation.GridToVector(targetGridLocation));
                break;
            case ObjectDirection.Up:
                targetGridLocation = new GridLocation(currentGridLocation.X, currentGridLocation.Y + 1);

                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTargetObject(GridLocation.GridToVector(targetGridLocation));
                break;
            default:
                Logger.Warning("Unhandled locomotion direction {0}", direction);
                break;
        }

     

        if (!AnimationHandler.InLocomotion)
            AnimationHandler.SetLocomotion(true);

        _hasTarget = true;
    }

    public void ReachTarget()
    {
        Logger.Warning("ReachTarget");
        Logger.Log("DestinationSetter.target.transform.position.y - transform.position.y is {0}", Vector2.Distance(DestinationSetter.target.position, transform.position));

        _hasTarget = false;
        //SetTransformToTarget();
        AnimationHandler.SetLocomotion(false);
        //DestinationSetter.target = null;
        CharacterPath.SetPath(null);

    }

    //public override void ReachLocomotionTarget()
    //{
    //    Logger.Log("ReachLocomotionTarget");
    //    Logger.Log("CharacterPath.pathPending {0}", CharacterPath.pathPending);
    //    ////Vector3 roundedVectorPosition = new Vector3((float)Math.Round(transform.position.x - GridLocation.OffsetToTileMiddle), (float)Math.Round(transform.position.y - GridLocation.OffsetToTileMiddle));
    //    ////transform.position = new Vector3(roundedVectorPosition.x + GridLocation.OffsetToTileMiddle, roundedVectorPosition.y + GridLocation.OffsetToTileMiddle, 0);
    //    Logger.Log("DestinationSetter.target.transform.position.y - transform.position.y is {0}", Vector2.Distance(DestinationSetter.target.position, transform.position));
    //    //if (Vector2.Distance(DestinationSetter.target.position, transform.position) > 0.2f) {
    //        //CharacterPath.reachedDestination = false;
    //        _hasTarget = false;
    //    if (Vector2.Distance(DestinationSetter.target.position, transform.position) < 0.1f)
    //    {
    //        SetTransformToTarget();
    //        AnimationHandler.SetLocomotion(false);
    //    }
    ////}
    //    //else
    //    //{
    //    //    Logger.Warning("We claimed to have reached the target even though the distance is still {0}", Vector2.Distance(DestinationSetter.target.position, transform.position));
    //    //}

    //}

    private void SetTransformToTarget()
    {
        transform.position = new Vector3(Target.x, Target.y, 0);
    }
}
