﻿using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Platform CurrentPlatform;
    public IPlatformConfiguration Configuration;
    public KeyboardConfiguration KeyboardConfiguration;

    public GameObject GridGO;
    public GameObject AstarGO;

    public GameObject MazeLevelManagerPrefab;
    public GameObject CharacterManagerPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(GridGO, "Could not find GridGO prefab");
        Guard.CheckIsNull(AstarGO, "AstarGO");

        Guard.CheckIsNull(MazeLevelManagerPrefab, "Could not find MazeLevelManagerPrefab");
        Guard.CheckIsNull(CharacterManagerPrefab, "Could not find CharacterManagerPrefab");

        InitialiseLoggers();

        if (Application.isMobilePlatform)
        {
            CurrentPlatform = Platform.Android;
            Configuration = new AndroidConfiguration();
        }
        else
        {
            CurrentPlatform = Platform.PC;
            Configuration = new PCConfiguration();
        }

        KeyboardConfiguration = new KeyboardConfiguration();

        Instantiate(MazeLevelManagerPrefab, transform);
        Instantiate(CharacterManagerPrefab, transform);

        MazeLevelManager.Instance.LoadLevel(MazeName.PathfindingTest);
        CharacterManager.Instance.SpawnCharacters();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyboardConfiguration.Console))
        {
            ConsoleContainer.Instance.ToggleConsole();
        }
    }

    private void InitialiseLoggers()
    {
        Logger.General.enableLogs = true;
        Logger.Time.enableLogs = false;
        Logger.Locomotion.enableLogs = false;
        Logger.Building.enableLogs = true;
        Logger.Pathfinding.enableLogs = false;
        Logger.Initialisation.enableLogs = true;
        Logger.Character.enableLogs = false;
        Logger.UI.enableLogs = false;
    }
}