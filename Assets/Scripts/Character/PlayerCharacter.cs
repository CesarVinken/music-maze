using UnityEngine;

public class PlayerCharacter : Character
{
    public KeyboardInput KeyboardInput = KeyboardInput.None;
    public int PlayerNoInGame = 1;  // in multiplayer on multiple computers there can be a player 1 and player 2, while both use their Player1 keyboard input
    private bool _hasTarget = false;
    private Vector2 _mobileFingerDownPosition;

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

        if (hasAuthority) // only check input for this client's characters
        {
            if (GameManager.Instance.CurrentPlatform == Platform.PC)
                CheckKeyboardInput();

            CheckPointerInput();
        }

        if (_hasTarget)
        {
            MoveCharacter();
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
        GridLocation gridLocation = GridLocation.FindClosestGridTile(target);

        if (!ValidateTarget(gridLocation)) return;

        Vector2 gridTarget = GridLocation.GridToVector(gridLocation);


        SetLocomotionTarget(gridTarget);

        if (!AnimationHandler.InLocomotion)
            AnimationHandler.SetLocomotion(true);

        _hasTarget = true;

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

        CharacterPath.SearchPath();
        //CharacterPath.transform.position = transform.position;
        //CharacterPath.transform.rotation = Quaternion.identity;
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

                SetLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
                break;
            case ObjectDirection.Left:
                targetGridLocation = new GridLocation(currentGridLocation.X - 1, currentGridLocation.Y);
                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
                break;
            case ObjectDirection.Right:
                targetGridLocation = new GridLocation(currentGridLocation.X + 1, currentGridLocation.Y);
                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
                break;
            case ObjectDirection.Up:
                targetGridLocation = new GridLocation(currentGridLocation.X, currentGridLocation.Y + 1);

                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
                break;
            default:
                Logger.Warning("Unhandled locomotion direction {0}", direction);
                break;
        }

        if (!AnimationHandler.InLocomotion)
            AnimationHandler.SetLocomotion(true);

        _hasTarget = true;
    }

    public override void ReachLocomotionTarget()
    {
        _hasTarget = false;
        AnimationHandler.SetLocomotion(false);    
    }
}
