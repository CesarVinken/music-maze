using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameType
{
    SinglePlayer,
    Multiplayer
}

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public Platform CurrentPlatform;
    public IPlatformConfiguration Configuration;
    public KeyboardConfiguration KeyboardConfiguration;
    public GameType GameType;

    //public Pathfinding

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

        GameType = PhotonNetwork.PlayerList.Length == 0 ? GameType.SinglePlayer : GameType.Multiplayer;

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

        MazeLevelManager.Instance.LoadLevel(MazeName.LargerLevelTest);

        AstarGO.SetActive(true);    // triggers pathfinding grid scan
    }

    public void Start()
    {
        // Temporarily turned off for testing purposes
        //if (!PhotonNetwork.IsConnected)
        //{
        //    SceneManager.LoadScene("Launcher");
        //    return;
        //}
        
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
