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
    [SerializeField] private GameObject _tileBaseGroundPrefab;
    [SerializeField] private GameObject _tileBaseWaterPrefab;
    [SerializeField] private GameObject _tilePathPrefab;
    [SerializeField] private GameObject _playerSpawnpointPrefab;
    [SerializeField] private GameObject _mazeLevelEntryPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorTilePrefab, "EditorTilePrefab", gameObject);
        Guard.CheckIsNull(InGameTilePrefab, "InGameTilePrefab", gameObject);
        Guard.CheckIsNull(_tileBaseGroundPrefab, "TileBaseGroundPrefab", gameObject);
        Guard.CheckIsNull(_tileBaseWaterPrefab, "TileBaseWaterPrefab", gameObject);
        Guard.CheckIsNull(_tilePathPrefab, "TilePathPrefab", gameObject);
        Guard.CheckIsNull(_playerSpawnpointPrefab, "PlayerSpawnpointPrefab", gameObject);
        Guard.CheckIsNull(_mazeLevelEntryPrefab, "MazeLevelEntryPrefab", gameObject);

        MazeLevelInvitation.PendingInvitation = false;
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
        Overworld.MazeEntries.Clear();

        CameraManager.Instance.ResetCameras();
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

        CameraManager.Instance.ResetCameras();
        CameraManager.Instance.SetPanLimits(EditorOverworld.LevelBounds);
    }

    public IEnumerator ScanCoroutine()
    {
        yield return new WaitForSeconds(.2f); // This waiting time should be dealt with more efficiently. Currently it is there to make sure that the characters are spawned in 
        MainScreenOverlayCanvas.Instance.BlackOutSquare.ResetToDefault();
        GameManager.Instance.CharacterManager.SpawnCharacters();
        CameraManager.Instance.SetPanLimits(Overworld.LevelBounds);
        CameraManager.Instance.FocusCamerasOnPlayer();

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
                return _playerSpawnpointPrefab;
            case Type MazeLevelEntry when MazeLevelEntry == typeof(MazeLevelEntry):
                return _mazeLevelEntryPrefab;
            default:
                Logger.Error($"Could not find a prefab for the tile attribute type of {typeof(T)}");
                return null;
        }
    }

    public GameObject GetTileBackgroundPrefab<T>() where T : ITileBackground
    {
        switch (typeof(T))
        {
            case Type baseGround when baseGround == typeof(OverworldTileBaseGround):
                return _tileBaseGroundPrefab;
            case Type baseWater when baseWater == typeof(OverworldTileBaseWater):
                return _tileBaseWaterPrefab;
            case Type path when path == typeof(OverworldTilePath):
                return _tilePathPrefab;
            default:
                Logger.Error($"Could not find a prefab for the tile background type of {typeof(T)}");
                return null;
        }
    }

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
            object[] data = (object[])photonEvent.CustomData;
            string mazeName = (string)data[0];

            PersistentGameManager.SetCurrentSceneName(mazeName);
            Logger.Log("received event to load maze");

            PhotonNetwork.LoadLevel("Maze");

        } else if(eventCode == PlayerSendsMazeLevelInvitationEvent.PlayerSendsMazeLevelInvitationEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string invitorName = (string)data[0];
            string mazeName = (string)data[1];

            Logger.Log($"received event for invitation from {invitorName}");

            MazeLevelInvitation.PendingInvitation = true;

            OverworldMainScreenOverlayCanvas.Instance.ShowMazeInvitation(invitorName, mazeName);
        } else if(eventCode == PlayerRejectsMazeLevelInvitationEvent.PlayerRejectsMazeLevelInvitationEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string rejectorName = (string)data[0];
            string mazeName = (string)data[1];
            Logger.Log($"received event that {rejectorName} rejected the invitation");
            OverworldMainScreenOverlayCanvas.Instance.ShowMazeInvitationRejection(rejectorName, mazeName);
        }
    }
}
