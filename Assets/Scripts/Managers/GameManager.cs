using Photon.Pun;
using System;
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

    public event Action CompleteMazeLevelEvent;

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

        JsonMazeLevelFileReader fileReader = new JsonMazeLevelFileReader();
        MazeLevelData startUpLevelData = fileReader.LoadLevel("default");

        if(startUpLevelData == null)
        {
            Logger.Error("Could not find the default level for startup");
        }
        MazeLevelManager.Instance.SetupLevel(startUpLevelData);
    }

    public void Start()
    {
        // Temporarily turned off for testing purposes
        //if (!PhotonNetwork.IsConnected)
        //{
        //    SceneManager.LoadScene("Launcher");
        //    return;
        //}
        if (MazeLevelManager.Instance.Level == null)
        {
            Logger.Log(Logger.Initialisation, "No level loaded on startup. Returning");
            return;
        }
        if (MazeLevelManager.Instance.Level.PlayerCharacterSpawnpoints.Count == 0) return;
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
        Logger.Character.enableLogs = false;
        Logger.Datawriting.enableLogs = true;
        Logger.Datareading.enableLogs = true;
        Logger.General.enableLogs = true;
        Logger.Initialisation.enableLogs = true;
        Logger.Level.enableLogs = false;
        Logger.Locomotion.enableLogs = false;
        Logger.Pathfinding.enableLogs = false;
        Logger.Score.enableLogs = true;
        Logger.Time.enableLogs = false;
        Logger.UI.enableLogs = false;
    }

    public void CompleteMazeLevel()
    {
        CompleteMazeLevelEvent.Invoke();
    }
}
