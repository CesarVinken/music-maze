using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Platform CurrentPlatform;
    public IPlatformConfiguration Configuration;

    public GameObject GridGO;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(GridGO, "Could not find GridGO prefab");

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

        gameObject.AddComponent<MazeLevelManager>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
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
