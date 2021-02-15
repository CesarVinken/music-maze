using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneLoadOrigin
{
    Gameplay,
    Editor
}

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static GameManager Instance;

    public static Platform CurrentPlatform;
    public static SceneType CurrentSceneType;
    public static GameType GameType;
    public static SceneLoadOrigin SceneLoadOrigin = SceneLoadOrigin.Gameplay;

    public IPlatformConfiguration Configuration;
    public KeyboardConfiguration KeyboardConfiguration;

    public GameObject GridGO;
    
    [SerializeField] private GameObject _mazeLevelManagerPrefab;
    [SerializeField] private GameObject _characterManagerPrefab;
    [SerializeField] private GameObject _mazeLevelSpriteManagerPrefab;
    [SerializeField] private GameObject _overworldSpriteManagerPrefab;
    [SerializeField] private GameObject _overworldManagerPrefab;

    [SerializeField] private SceneType _thisSceneType;

    public event Action CompleteMazeLevelEvent;

    public List<string> PlayableLevelNames = new List<string>();

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(GridGO, "GridGO", gameObject);

        Guard.CheckIsNull(_mazeLevelManagerPrefab, "MazeLevelManagerPrefab", gameObject);
        Guard.CheckIsNull(_characterManagerPrefab, "CharacterManagerPrefab", gameObject);
        Guard.CheckIsNull(_mazeLevelSpriteManagerPrefab, "MazeLevelSpriteManagerPrefab", gameObject);
        Guard.CheckIsNull(_overworldSpriteManagerPrefab, "_overworldSpriteManagerPrefab", gameObject);
        Guard.CheckIsNull(_overworldManagerPrefab, "OverworldManagerPrefab", gameObject);

        InitialiseLoggers();

        GameType = PhotonNetwork.PlayerList.Length == 0 ? GameType.SinglePlayer : GameType.Multiplayer;
        CurrentSceneType = _thisSceneType;
        Logger.Warning($"We set the game type to {GameType} in a {CurrentSceneType} scene. The scene loading origin is {SceneLoadOrigin}");

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

        switch (CurrentSceneType)
        {
            case SceneType.Overworld:
                Instantiate(_overworldManagerPrefab, transform);
                Instantiate(_overworldSpriteManagerPrefab, transform);
                break;
            case SceneType.Maze:
                Instantiate(_mazeLevelManagerPrefab, transform);
                Instantiate(_characterManagerPrefab, transform);
                Instantiate(_mazeLevelSpriteManagerPrefab, transform);
                break;
            default:
                Logger.Error($"Scenetype {CurrentSceneType} is not implemented yet");
                break;
        }  
    }

    public void Start()
    {
        CameraController.Instance.SetZoomLevel(Configuration.CameraZoomLevel);

        switch (CurrentSceneType)
        {
            case SceneType.Overworld:
                Logger.Log("instantiate overworld sprites, tiles and characters");
                break;
            case SceneType.Maze:
                // We loaded a maze scene through the game. Set up the maze level
                if(SceneLoadOrigin == SceneLoadOrigin.Gameplay)
                {
                    MazeLevelData startUpMazeLevelData = MazeLevelLoader.LoadMazeLevelData("default");

                    if (startUpMazeLevelData == null)
                    {
                        Logger.Error("Could not find the default level for startup");
                    }

                    MazeLevelLoader.LoadMazeLevel(startUpMazeLevelData);

                    if (MazeLevelManager.Instance.Level == null)
                    {
                        Logger.Log(Logger.Initialisation, "No level loaded on startup. Returning");
                        return;
                    }
                    if (MazeLevelManager.Instance.Level.PlayerCharacterSpawnpoints.Count == 0) return;

                    PlayableLevelNames = MazeLevelLoader.GetAllPlayableLevelNames();
                } // We loaded a maze scene through the editor. Set up an empty grid for in the editor
                else
                {
                    Logger.Log("create empty grid");
                    EditorCanvasUI.Instance.MazeModificationPanel.GenerateTiles();
                }
                break;
            default:
                Logger.Error($"Scenetype {CurrentSceneType} is not implemented yet");
                break;
        }
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
        Logger.Level.enableLogs = true;
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

    // THIS FUNCTION IS TEMPORARY
    public void ToMazeFromOverworld()
    {
        if (GameManager.GameType == GameType.SinglePlayer)
        {
            SceneManager.LoadScene("Maze");
        }
        else
        {
            LoadNextMazeLevelEvent loadNextLevelEvent = new LoadNextMazeLevelEvent();
            loadNextLevelEvent.SendLoadNextMazeLevelEvent("default");
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == LoadNextMazeLevelEvent.LoadNextMazeLevelEventCode)
        {
            SceneManager.LoadScene("Maze");
        }
    }
}
