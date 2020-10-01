using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public Platform CurrentPlatform;
    public IPlatformConfiguration Configuration;
    public KeyboardConfiguration KeyboardConfiguration;

    public GameObject GridGO;
    public GameObject PathfindingSystemGO;

    [Header("Prefabs")]
    public GameObject MazeLevelManagerPrefab;
    public GameObject CharacterManagerPrefab;
    public GameObject GridPrefab;
    public GameObject PathfindingSystemPrefab;
    public GameObject PlayerSpawnSystemPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(MazeLevelManagerPrefab, "Could not find MazeLevelManagerPrefab");
        Guard.CheckIsNull(CharacterManagerPrefab, "Could not find CharacterManagerPrefab");
        Guard.CheckIsNull(GridPrefab, "Could not find GridPrefab");
        Guard.CheckIsNull(PathfindingSystemPrefab, "Could not find PathfindingSystemPrefab");
        Guard.CheckIsNull(PlayerSpawnSystemPrefab, "Could not find PlayerSpawnSystemPrefab");

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
        Instantiate(CharacterManagerPrefab, transform); // rework character spawning
        //Instantiate(PlayerSpawnSystemPrefab, transform);
        GridGO = Instantiate(GridPrefab, transform);
        PathfindingSystemGO = Instantiate(PathfindingSystemPrefab, transform);

        Guard.CheckIsNull(GridGO, "Could not find GridGO");
        Guard.CheckIsNull(PathfindingSystemGO, "PathfindingSystemGO");

        MazeLevelManager.Instance.LoadLevel(MazeName.PathfindingTest);
        //CharacterManager.Instance.SpawnCharacters();
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
