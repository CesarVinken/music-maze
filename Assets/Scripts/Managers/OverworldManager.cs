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
    public GameObject MazeLevelEntryPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorTilePrefab, "EditorTilePrefab", gameObject);
        Guard.CheckIsNull(InGameTilePrefab, "InGameTilePrefab", gameObject);
        Guard.CheckIsNull(TileBaseBackgroundPrefab, "TileBaseBackgroundPrefab", gameObject);
        Guard.CheckIsNull(TilePathPrefab, "TilePathPrefab", gameObject);
        Guard.CheckIsNull(PlayerSpawnpointPrefab, "PlayerSpawnpointPrefab", gameObject);
        Guard.CheckIsNull(MazeLevelEntryPrefab, "MazeLevelEntryPrefab", gameObject);

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
            case Type MazeLevelEntry when MazeLevelEntry == typeof(MazeLevelEntry):
                return MazeLevelEntryPrefab;
            default:
                Logger.Error($"Could not find a prefab for the tile attribute type of {typeof(T)}");
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
