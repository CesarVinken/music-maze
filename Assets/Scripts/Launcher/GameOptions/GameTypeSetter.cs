using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTypeSetter : MonoBehaviour
{
    [SerializeField] private LaunchGameUI _launchGameUI;

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void HandleInputValue(int currentValue)
    {
        if(currentValue == 0)
        {
            Logger.Log("Set game mode to Campaign");
            GameRules.SetGameMode(GameMode.Campaign);
        }
        else if(currentValue == 1)
        {
            Logger.Log("Set game mode to Random Maze");
            GameRules.SetGameMode(GameMode.RandomMaze);
        }
        else
        {
            Logger.Error($"Unknown game type value {currentValue}");
        }

        UpdateGameModeEvent updateGameModeEvent = new UpdateGameModeEvent();
        updateGameModeEvent.SendUpdateGameModeEvent(GameRules.GameMode);
    }
}
