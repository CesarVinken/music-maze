using System;
using UnityEngine;

public class PlayerCharacter : Character
{
    public KeyboardInput KeyboardInput = KeyboardInput.None;
    public int PlayerNoInGame = 1;  // in multiplayer on multiple computers there can be a player 1 and player 2, while both use their Player1 keyboard input
    private bool _hasTarget = false;

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

        if (_hasTarget)
        {
            MoveCharacter();
        }
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
        Vector3 roundedVectorPosition = new Vector3((float)Math.Round(transform.position.x - GridLocation.OffsetToTileMiddle), (float)Math.Round(transform.position.y - GridLocation.OffsetToTileMiddle));
        transform.position = new Vector3(roundedVectorPosition.x + GridLocation.OffsetToTileMiddle, roundedVectorPosition.y + GridLocation.OffsetToTileMiddle, 0);

        _hasTarget = false;
        AnimationHandler.SetLocomotion(false);
    }
}
