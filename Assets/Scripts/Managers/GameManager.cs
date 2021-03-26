using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum SceneLoadOrigin
{
    Gameplay,
    Editor
}

public static class PersistentGameManager
{
    public static Platform CurrentPlatform;
    public static SceneType CurrentSceneType;
    public static SceneLoadOrigin SceneLoadOrigin = SceneLoadOrigin.Gameplay;
    private static string _currentMazeLevelName  = "";
    private static string _overworldName = "";
    private static string _originMazeLevelName = "";

    public static string CurrentSceneName { get => _currentMazeLevelName; private set => _currentMazeLevelName = value; }
    public static string OverworldName { get => _overworldName; private set => _overworldName = value; }
    public static string LastMazeLevelName { get => _originMazeLevelName; private set => _originMazeLevelName = value; }

    public static void SetCurrentSceneName(string currentMazeLevelName)
    {
        _currentMazeLevelName = currentMazeLevelName;
    }

    public static void SetOverworldName(string overworldName)
    {
        _overworldName = overworldName;
    }

    public static void SetLastMazeLevelName(string originMazeLevelName)
    {
        _originMazeLevelName = originMazeLevelName;
    }
}

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public IPlatformConfiguration Configuration;
    public KeyboardConfiguration KeyboardConfiguration;

    public IEditorLevel CurrentEditorLevel; // can be any editor or game level
    public IInGameLevel CurrentGameLevel;

    public GameObject GridGO;
    
    [SerializeField] private GameObject _mazeLevelManagerPrefab;
    [SerializeField] private GameObject _mazeCharacterManagerPrefab;
    [SerializeField] private GameObject _overworldCharacterManagerPrefab;
    [SerializeField] private GameObject _mazeLevelSpriteManagerPrefab;
    [SerializeField] private GameObject _overworldSpriteManagerPrefab;
    [SerializeField] private GameObject _overworldManagerPrefab;

    [SerializeField] private SceneType _thisSceneType;

    public event Action CompleteMazeLevelEvent;

    public List<string> PlayableLevelNames = new List<string>();

    public ICharacterManager CharacterManager;

    public void Awake()
    {
        Instance = this;
        Logger.Log(Logger.Initialisation, $"Our game mode is {GameRules.GameMode}");
        Guard.CheckIsNull(GridGO, "GridGO", gameObject);

        Guard.CheckIsNull(_mazeLevelManagerPrefab, "MazeLevelManagerPrefab", gameObject);
        Guard.CheckIsNull(_mazeCharacterManagerPrefab, "_mazeCharacterManagerPrefab", gameObject);
        Guard.CheckIsNull(_overworldCharacterManagerPrefab, "_overworldCharacterManagerPrefab", gameObject);
        Guard.CheckIsNull(_mazeLevelSpriteManagerPrefab, "_mazeLevelSpriteManagerPrefab", gameObject);
        Guard.CheckIsNull(_overworldSpriteManagerPrefab, "_overworldSpriteManagerPrefab", gameObject);
        Guard.CheckIsNull(_overworldManagerPrefab, "_overworldManagerPrefab", gameObject);

        InitialiseLoggers();

        if(PhotonNetwork.PlayerList.Length == 0)
        {
            GameRules.SetGamePlayerType(GamePlayerType.SinglePlayer);
        }
        else
        {
            GameRules.SetGamePlayerType(GamePlayerType.Multiplayer);
        }

        PersistentGameManager.CurrentSceneType = _thisSceneType;
        Logger.Warning($"We set the game type to {GameRules.GamePlayerType} in a {PersistentGameManager.CurrentSceneType} scene. The scene loading origin is {PersistentGameManager.SceneLoadOrigin}");

        if (Application.isMobilePlatform)
        {
            PersistentGameManager.CurrentPlatform = Platform.Android;
            Configuration = new AndroidConfiguration();
        }
        else
        {
            PersistentGameManager.CurrentPlatform = Platform.PC;
            Configuration = new PCConfiguration();
        }

        KeyboardConfiguration = new KeyboardConfiguration();

        switch (PersistentGameManager.CurrentSceneType)
        {
            case SceneType.Overworld:
                Instantiate(_overworldSpriteManagerPrefab, transform);
                Instantiate(_overworldCharacterManagerPrefab, transform);
                Instantiate(_overworldManagerPrefab, transform);
                break;
            case SceneType.Maze:
                Instantiate(_mazeLevelSpriteManagerPrefab, transform);
                Instantiate(_mazeCharacterManagerPrefab, transform);
                Instantiate(_mazeLevelManagerPrefab, transform);
                break;
            default:
                Logger.Error($"Scenetype {PersistentGameManager.CurrentSceneType} is not implemented yet");
                break;
        }  
    }

    public void Start()
    {
        CameraController.Instance.SetZoomLevel(Configuration.CameraZoomLevel);

        switch (PersistentGameManager.CurrentSceneType)
        {
            case SceneType.Overworld:
                Logger.Log("instantiate overworld sprites, tiles and characters");
                if (PersistentGameManager.SceneLoadOrigin == SceneLoadOrigin.Gameplay)
                {
                    if(PersistentGameManager.OverworldName == "")
                    {
                        PersistentGameManager.SetOverworldName("overworld");
                    }

                    string overworldName = PersistentGameManager.OverworldName;
                    Logger.Log($"We will load the maze '{overworldName}'");
                    OverworldData startUpOverworldData = OverworldLoader.LoadOverworldData(overworldName);

                    if (startUpOverworldData == null)
                    {
                        Logger.Error("Could not find the default overworld for startup");
                    }

                    OverworldLoader.LoadOverworld(startUpOverworldData);

                    if (OverworldManager.Instance.Overworld == null)
                    {
                        Logger.Log(Logger.Initialisation, "No overworld loaded on startup. Returning");
                        return;
                    }
                    //if (OverworldManager.Instance.Overworld.PlayerCharacterSpawnpoints.Count == 0) return;

                    //PlayableLevelNames = OverworldLoader.GetAllPlayableLevelNames();
                } // We loaded a overworld scene through the editor. Set up an empty grid for in the editor
                else
                {
                    Logger.Log("create empty grid");
                    EditorCanvasUI.Instance.OverworldModificationPanel.GenerateTiles();
                }
                break;
            case SceneType.Maze:
                // We loaded a maze scene through the game. Set up the maze level
                if(PersistentGameManager.SceneLoadOrigin == SceneLoadOrigin.Gameplay)
                {
                    if (PersistentGameManager.CurrentSceneName == "")
                    {
                        PersistentGameManager.SetCurrentSceneName("default");
                    }

                    string mazeName = PersistentGameManager.CurrentSceneName;

                    PersistentGameManager.SetLastMazeLevelName(mazeName);
                    Logger.Log($"We will load the maze '{mazeName}'");
                    MazeLevelData startUpMazeLevelData = MazeLevelLoader.LoadMazeLevelData(mazeName);

                    if (startUpMazeLevelData == null)
                    {
                        Logger.Error($"Could not find the level {mazeName} for startup. Will load defult level instead.");
                        mazeName = "default";
                        startUpMazeLevelData = MazeLevelLoader.LoadMazeLevelData(mazeName);
                    }

                    MazeLevelLoader.LoadMazeLevel(startUpMazeLevelData);

                    if (CurrentGameLevel == null)
                    {
                        Logger.Log(Logger.Initialisation, "No level loaded on startup. Returning");
                        return;
                    }
                    if (CurrentGameLevel.PlayerCharacterSpawnpoints.Count == 0) return;

                    PlayableLevelNames = MazeLevelLoader.GetAllPlayableLevelNames();
                } // We loaded a maze scene through the editor. Set up an empty grid for in the editor
                else
                {
                    Logger.Log("create empty grid");
                    EditorCanvasUI.Instance.MazeModificationPanel.GenerateTiles();
                }
                break;
            default:
                Logger.Error($"Scenetype {PersistentGameManager.CurrentSceneType} is not implemented yet");
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
}
