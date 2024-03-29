﻿using UnityEngine;

public class KeyboardConfiguration
{
    public KeyCode Console;
    public KeyCode Player1Up;
    public KeyCode Player1Right;
    public KeyCode Player1Down;
    public KeyCode Player1Left;
    public KeyCode Player1Action;

    public KeyCode Player2Up;
    public KeyCode Player2Right;
    public KeyCode Player2Down;
    public KeyCode Player2Left;
    public KeyCode Player2Action;

    public KeyboardConfiguration()
    {
        Console = KeyCode.F1;

        Player1Up = KeyCode.W;
        Player1Right = KeyCode.D;
        Player1Down = KeyCode.S;
        Player1Left = KeyCode.A;

        Player1Action = KeyCode.Return;
        if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer) Player1Action = KeyCode.H;
        
        Player2Up = KeyCode.UpArrow;
        Player2Right = KeyCode.RightArrow;
        Player2Down = KeyCode.DownArrow;
        Player2Left = KeyCode.LeftArrow;
        Player2Action = KeyCode.KeypadEnter;
    }
}
