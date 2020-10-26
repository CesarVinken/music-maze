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

        Guard.CheckIsNull(GridGO, "GridGO", gameObject);
        Guard.CheckIsNull(AstarGO, "AstarGO", gameObject);

        Guard.CheckIsNull(MazeLevelManagerPrefab, "MazeLevelManagerPrefab", gameObject);
        Guard.CheckIsNull(CharacterManagerPrefab, "CharacterManagerPrefab", gameObject);

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

        MazeLevelManager.Instance.LoadLevel(MazeName.CameraBoundsTest);

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
        CameraController.Instance.FocusOnPlayer();
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
        Logger.Level.enableLogs = false;
        Logger.Locomotion.enableLogs = false;
        Logger.Pathfinding.enableLogs = false;
        Logger.Initialisation.enableLogs = true;
        Logger.Character.enableLogs = false;
        Logger.UI.enableLogs = false;
        Logger.Datawriting.enableLogs = true;
        Logger.Datareading.enableLogs = true;
    }
}
