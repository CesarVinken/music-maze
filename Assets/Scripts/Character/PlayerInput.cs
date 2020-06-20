using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : CharacterLocomotion
{
    public void Update()
    {
        base.Update();
        if (Console.Instance && Console.Instance.ConsoleState == ConsoleState.Large)
            return;

        if(GameManager.Instance.CurrentPlatform == Platform.PC)
            CheckKeyboardInput();

        //CheckPointerInput();
        //if (TargetObject != null)
        //{
        //    HandleMovement();
        //}
    }

    private void CheckKeyboardInput()
    {
        // TODO: make controls dynamic for players, not hardcoded keys
        if (Input.GetKey(KeyCode.W))
        {
            Logger.Log("Move up for player 1");
            TryStartCharacterMovement(ObjectDirection.Up);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Logger.Log("Move right for player 1");
            TryStartCharacterMovement(ObjectDirection.Right);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Logger.Log("Move down for player 1");
            TryStartCharacterMovement(ObjectDirection.Down);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Logger.Log("Move left for player 1");
            TryStartCharacterMovement(ObjectDirection.Left);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Logger.Log("Move up for player 2");
            TryStartCharacterMovement(ObjectDirection.Up);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Logger.Log("Move right for player 2");
            TryStartCharacterMovement(ObjectDirection.Right);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Logger.Log("Move down for player 2");
            TryStartCharacterMovement(ObjectDirection.Down);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Logger.Log("Move left for player 2");
            TryStartCharacterMovement(ObjectDirection.Left);
        }
    }
}
