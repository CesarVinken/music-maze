using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;

public class OverworldManager : MonoBehaviour, IOnEventCallback
{
    public static OverworldManager Instance;

    public InGameOverworld Overworld;
    public EditorOverworld EditorOverworld;

    public GameObject EditorTilePrefab;
    public GameObject InGameTilePrefab;
    public GameObject TileBaseBackgroundPrefab;
    public GameObject TilePathPrefab;
    public GameObject PlayerSpawnpointPrefab;
    public GameObject MazeEntryPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorTilePrefab, "EditorTilePrefab", gameObject);
        Guard.CheckIsNull(InGameTilePrefab, "InGameTilePrefab", gameObject);
        Guard.CheckIsNull(TileBaseBackgroundPrefab, "TileBaseBackgroundPrefab", gameObject);
        Guard.CheckIsNull(TilePathPrefab, "TilePathPrefab", gameObject);
        Guard.CheckIsNull(PlayerSpawnpointPrefab, "PlayerSpawnpointPrefab", gameObject);
        Guard.CheckIsNull(MazeEntryPrefab, "MazeEntryPrefab", gameObject);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public void UnloadOverworld()
    {
        if (Overworld == null) return;

        if (TilesContainer.Instance)
        {
            Destroy(TilesContainer.Instance.gameObject);
        }

        TilesContainer.Instance = null;

        GameManager.Instance.CharacterManager.UnloadCharacters();
        SceneObjectManager.Instance.UnloadSceneObjects();

        Logger.Log(Logger.Initialisation, "Unload Overworld {0}", Overworld);
        Overworld.Tiles.Clear();
        Overworld.TilesByLocation.Clear();
        CameraController.Instance.ResetCamera();
        Overworld = null;
    }

    public void SetupOverworld(OverworldData overworldData)
    {
        Overworld = InGameOverworld.Create(overworldData);

        InitialiseTileAttributes();

        Logger.Log("Start scan...");
        IEnumerator coroutine = ScanCoroutine();

        StartCoroutine(coroutine);
    }


    public void SetupOverworldForEditor(OverworldData overworldData)
    {
        EditorOverworld = EditorOverworld.Create(overworldData);

        InitialiseEditorTileBackgrounds();
        InitialiseEditorTileAttributes();

        MainScreenOverlayCanvas.Instance.BlackOutSquare.ResetToDefault();

        CameraController.Instance.ResetCamera();
        CameraController.Instance.SetPanLimits(EditorOverworld.LevelBounds);
    }

    public IEnumerator ScanCoroutine()
    {
        yield return new WaitForSeconds(.2f); // This waiting time should be dealt with more efficiently. Currently it is there to make sure that the characters are spawned in 
        MainScreenOverlayCanvas.Instance.BlackOutSquare.ResetToDefault();
        GameManager.Instance.CharacterManager.SpawnCharacters();
        CameraController.Instance.SetPanLimits(Overworld.LevelBounds);
        CameraController.Instance.FocusOnPlayer();

        AstarPath.active.Scan();    // We should only scan once all the tiles are loaded with their correct (walkable) attributes and obstacles
        yield return new WaitForSeconds(.4f);

        // start movement of all actors that depend on the updated pathfinding only after the scan.
        GameManager.Instance.CharacterManager.UnfreezeCharacters();
    }

    private void InitialiseTileAttributes()
    {
        for (int i = 0; i < Overworld.Tiles.Count; i++)
        {
            InGameOverworldTile tile = Overworld.Tiles[i];
            tile.InitialiseTileAttributes();
        }
    }

    private void InitialiseEditorTileAttributes()
    {
        for (int i = 0; i < EditorOverworld.Tiles.Count; i++)
        {
            EditorOverworldTile tile = EditorOverworld.Tiles[i];
            tile.InitialiseTileAttributes();
        }
    }

    private void InitialiseEditorTileBackgrounds()
    {
        for (int i = 0; i < EditorOverworld.Tiles.Count; i++)
        {
            EditorOverworldTile tile = EditorOverworld.Tiles[i];
            tile.InitialiseTileBackgrounds();
        }
    }

    public GameObject GetTileAttributePrefab<T>() where T : ITileAttribute
    {
        switch (typeof(T))
        {
            //case Type tileObstacle when tileObstacle == typeof(TileObstacle):
            //    return TileObstaclePrefab;
            case Type playerSpawnpoint when playerSpawnpoint == typeof(PlayerSpawnpoint):
                return PlayerSpawnpointPrefab;
            case Type mazeEntry when mazeEntry == typeof(MazeEntry):
                return MazeEntryPrefab;
            default:
                Logger.Error($"Could not find a prefab for the tile attribute type of {typeof(T)}");
                return null;
        }
    }

    //TODO: pick which maze to load
    public void LoadMaze()
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            PhotonNetwork.LoadLevel("Maze");
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
            Logger.Log("received event to load maze");
        // TODO: Make work with particular maze level names
            PhotonNetwork.LoadLevel("Maze");
            //    object[] data = (object[])photonEvent.CustomData;
            //    string pickedLevel = (string)data[0];

            //    MazeLevelData mazeLevelData = MazeLevelLoader.LoadMazeLevelData(pickedLevel);

            //    if (mazeLevelData == null)
            //    {
            //        Logger.Error($"Could not load maze level data for the randomly picked maze level {pickedLevel}");
            //    }

            //    MazeLevelLoader.LoadMazeLevel(mazeLevelData);
        }
    }
}
