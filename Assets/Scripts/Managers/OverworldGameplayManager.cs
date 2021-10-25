using DataSerialisation;
using ExitGames.Client.Photon;
using Gameplay;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UI;
using UnityEngine;

public class OverworldGameplayManager : MonoBehaviour, IOnEventCallback, IGameplayManager
{
    public static OverworldGameplayManager Instance;

    public InGameOverworld Overworld;
    public EditorOverworld EditorOverworld;

    public GameObject EditorTilePrefab;
    public GameObject InGameTilePrefab;

    [SerializeField] private GameObject _tileBaseGroundPrefab;
    [SerializeField] private GameObject _tileBaseWaterPrefab;
    [SerializeField] private GameObject _tilePathPrefab;
    [SerializeField] private GameObject _playerSpawnpointPrefab;
    [SerializeField] private GameObject _mazeLevelEntryPrefab;
    [SerializeField] private GameObject _cornerFillerPrefab;

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
        Guard.CheckIsNull(_cornerFillerPrefab, "CornerFillerPrefab", gameObject);

        GameManager.Instance.GameplayManager = this;
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
        if (Overworld != null)
        {
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
        else if(EditorOverworld != null)
        {
            ScreenSpaceOverworldEditorElements.Instance.CleanOut();

            EditorOverworld = null;
        }
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
        EditorOverworld.Create(overworldData);

        InitialiseEditorTileBackgrounds();
        InitialiseEditorTileAttributes();

        MainScreenOverlayCanvas.Instance.ResetBlackOutSquares();

        CameraManager.Instance.ResetCameras();
        CameraManager.Instance.SetLevelBounds();
        CameraManager.Instance.SetPanLimits();
    }

    public IEnumerator ScanCoroutine()
    {
        GameManager.Instance.CharacterManager.SpawnCharacters(); //Q? Can we move this up here?
        yield return new WaitForSeconds(.2f); // This waiting time should be dealt with more efficiently. Currently it is there to make sure that the characters are spawned in
        MainScreenOverlayCanvas.Instance.BlackOutSquaresToClear();
        CameraManager.Instance.SetLevelBounds();
        CameraManager.Instance.SetPanLimits();
        CameraManager.Instance.FocusCamerasOnPlayer();

        GameManager.Instance.CharacterManager.UnfreezeCharacters();
    }

    private void InitialiseTileAttributes()
    {
        for (int i = 0; i < Overworld.Tiles.Count; i++)
        {
            InGameOverworldTile tile = Overworld.Tiles[i] as InGameOverworldTile;
            tile.InitialiseTileAttributes();
        }
    }

    private void InitialiseEditorTileAttributes()
    {
        for (int i = 0; i < EditorOverworld.Tiles.Count; i++)
        {
            EditorOverworldTile tile = EditorOverworld.Tiles[i] as EditorOverworldTile;
            tile.InitialiseTileAttributes();
        }
    }

    private void InitialiseEditorTileBackgrounds()
    {
        for (int i = 0; i < EditorOverworld.Tiles.Count; i++)
        {
            EditorOverworldTile tile = EditorOverworld.Tiles[i] as EditorOverworldTile;
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
            case Type cornerFiller when cornerFiller == typeof(TileCornerFiller):
                return _cornerFillerPrefab;
            default:
                Logger.Error($"Could not find a prefab for the tile background type of {typeof(T)}");
                return null;
        }
    }

    public void LoadMaze()
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            IEnumerator loadLevelCoroutine = LoadSceneCoroutine("Maze");
            StartCoroutine(loadLevelCoroutine);
        }
        else
        {
            LoadNextMazeLevelEvent loadNextLevelEvent = new LoadNextMazeLevelEvent();
            loadNextLevelEvent.SendLoadNextMazeLevelEvent("default");
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        MainScreenOverlayCanvas.Instance.BlackOutSquaresToBlack();

        while (MainScreenOverlayCanvas.Instance.BlackOutSquares[0].BlackStatus != BlackStatus.Black)
        {
            yield return null;
        }

        PhotonNetwork.LoadLevel(sceneName);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCode.LoadNextMazeLevelEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            LoadNextMazeLevelEventHandler loadNextSceneEventHandler = new LoadNextMazeLevelEventHandler(this);
            loadNextSceneEventHandler.Handle(data);
        } else if(eventCode == EventCode.PlayerSendsMazeLevelInvitationEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            PlayerSendsMazeLevelInvitationEventHandler playerSendsMazeLevelInvitationEventHandler = new PlayerSendsMazeLevelInvitationEventHandler(this);
            playerSendsMazeLevelInvitationEventHandler.Handle(data);
        }
        else if(eventCode == EventCode.PlayerRejectsMazeLevelInvitationEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            PlayerRejectsMazeLevelInvitationEventHandler playerRejectsMazeLevelInvitationEventHandler = new PlayerRejectsMazeLevelInvitationEventHandler(this);
            playerRejectsMazeLevelInvitationEventHandler.Handle(data);
        }
    }


    public void StartNextSceneRoutine(string levelName)
    {
        IEnumerator loadLevelCoroutine = LoadSceneCoroutine(levelName);
        StartCoroutine(loadLevelCoroutine);
    }
}
