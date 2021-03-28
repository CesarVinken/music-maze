using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeLevelManager : MonoBehaviour, IOnEventCallback
{
    public static MazeLevelManager Instance;
    public InGameMazeLevel Level;
    public EditorMazeLevel EditorLevel;

    public List<CharacterSpawnpoint> PlayerCharacterSpawnpoints = new List<CharacterSpawnpoint>();
    public List<CharacterSpawnpoint> EnemyCharacterSpawnpoints = new List<CharacterSpawnpoint>();

    public GameObject EditorTilePrefab;
    public GameObject InGameTilePrefab;
    [SerializeField] private GameObject _tileBaseGroundPrefab;
    [SerializeField] private GameObject _tileBaseWaterPrefab;
    [SerializeField] private GameObject _tilePathPrefab;
    [SerializeField] private GameObject _tileObstaclePrefab;
    [SerializeField] private GameObject _playerExitPrefab;
    [SerializeField] private GameObject _playerOnlyPrefab;
    [SerializeField] private GameObject _playerSpawnpointPrefab;
    [SerializeField] private GameObject _enemySpawnpointPrefab;

    public int NumberOfUnmarkedTiles = -1;

    public GameObject GetTileAttributePrefab<T>() where T : ITileAttribute
    {
        switch (typeof(T))
        {
            case Type playerExit when playerExit == typeof(PlayerExit):
                return _playerExitPrefab;
            case Type tileObstacle when tileObstacle == typeof(TileObstacle):
                return _tileObstaclePrefab;
            case Type playerOnly when playerOnly == typeof(PlayerOnly):
                return _playerOnlyPrefab;
            case Type playerSpawnpoint when playerSpawnpoint == typeof(PlayerSpawnpoint):
                return _playerSpawnpointPrefab;
            case Type enemySpawnpoint when enemySpawnpoint == typeof(EnemySpawnpoint):
                return _enemySpawnpointPrefab;

            default:
                Logger.Error($"Could not find a prefab for the tile attribute type of {typeof(T)}");
                return null ;
        }
    }

    public GameObject GetTileBackgroundPrefab<T>() where T : ITileBackground
    {
        switch (typeof(T))
        {
            case Type baseGround when baseGround == typeof(MazeTileBaseGround):
                return _tileBaseGroundPrefab;
            case Type baseWater when baseWater == typeof(MazeTileBaseWater):
                return _tileBaseWaterPrefab;
            case Type path when path == typeof(MazeTilePath):
                return _tilePathPrefab;
            default:
                Logger.Error($"Could not find a prefab for the tile background type of {typeof(T)}");
                return null;
        }
    }

    public void Awake()
    {
        Guard.CheckIsNull(EditorTilePrefab, "EditorTilePrefab", gameObject);
        Guard.CheckIsNull(InGameTilePrefab, "InGameTilePrefab", gameObject);
        Guard.CheckIsNull(_tileBaseGroundPrefab, "TileBaseBackgroundPrefab", gameObject);
        Guard.CheckIsNull(_tilePathPrefab, "TilePathPrefab", gameObject);
        Guard.CheckIsNull(_tileObstaclePrefab, "TileObstaclePrefab", gameObject);
        Guard.CheckIsNull(_playerExitPrefab, "PlayerExitPrefab", gameObject);
        Guard.CheckIsNull(_playerOnlyPrefab, "PlayerOnlyPrefab", gameObject);
        Guard.CheckIsNull(_playerSpawnpointPrefab, "PlayerSpawnpointPrefab", gameObject);
        Guard.CheckIsNull(_enemySpawnpointPrefab, "EnemySpawnpointPrefab", gameObject);

        Instance = this;
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SetupLevel(MazeLevelData mazeLevelData)
    {
        Level = InGameMazeLevel.Create(mazeLevelData);

        InitialiseTileAttributes();

        Logger.Log("Start scan...");
        IEnumerator coroutine = ScanCoroutine();

        StartCoroutine(coroutine);
    }

    public void SetupLevelForEditor(MazeLevelData mazeLevelData)
    {
        EditorLevel = EditorMazeLevel.Create(mazeLevelData);

        InitialiseEditorTileBackgrounds();
        InitialiseEditorTileAttributes();

        MainScreenOverlayCanvas.Instance.BlackOutSquare.ResetToDefault();
        CameraManager.Instance.ResetCameras();
        CameraManager.Instance.SetPanLimits(EditorLevel.LevelBounds);
    }

    public IEnumerator ScanCoroutine()
    {
        yield return new WaitForSeconds(.2f); // This waiting time should be dealt with more efficiently. Currently it is there to make sure that the characters are spawned in 
        MainScreenOverlayCanvas.Instance.BlackOutSquare.ResetToDefault();
        GameManager.Instance.CharacterManager.SpawnCharacters();

        CameraManager.Instance.SetPanLimits(Level.LevelBounds);
        CameraManager.Instance.FocusCamerasOnPlayer();

        AstarPath.active.Scan();    // We should only scan once all the tiles are loaded with their correct (walkable) attributes and obstacles
        yield return new WaitForSeconds(.4f);

        // start movement of all actors that depend on the updated pathfinding only after the scan.
        GameManager.Instance.CharacterManager.UnfreezeCharacters();

        HandleSpawnpointMarkability();
    }

    public void UnloadLevel()
    {
        if (Level == null) return;

        if(TilesContainer.Instance)
        {
            Destroy(TilesContainer.Instance.gameObject);
        }

        TilesContainer.Instance = null;

        GameManager.Instance.CharacterManager.UnloadCharacters();
        SceneObjectManager.Instance.UnloadSceneObjects();

        Logger.Log(Logger.Initialisation, "Unload level {0}", Level);
        Level.Tiles.Clear();
        Level.TilesByLocation.Clear();
        CameraManager.Instance.ResetCameras();
        Level = null;

        NumberOfUnmarkedTiles = -1;
    }

    private void InitialiseTileAttributes()
    {
        for (int i = 0; i < Level.Tiles.Count; i++)
        {
            InGameMazeTile tile = Level.Tiles[i];
            tile.InitialiseTileAttributes();
        }
    }

    private void InitialiseEditorTileAttributes()
    {
        for (int i = 0; i < EditorLevel.Tiles.Count; i++)
        {
            EditorMazeTile tile = EditorLevel.Tiles[i];
            tile.InitialiseTileAttributes();
        }
    }

    private void InitialiseEditorTileBackgrounds()
    {
        for (int i = 0; i < EditorLevel.Tiles.Count; i++)
        {
            EditorMazeTile tile = EditorLevel.Tiles[i];
            tile.InitialiseTileBackgrounds();
        }
    }

    // Previously tried solution with collision detection on all separate clients for all players(instead of events). 
    // But the result was that some tile marking got skipped if the clients skipped walking over them because of a bad connection.
    // This way we can be sure all tiles are getting marked.
    public void SetTileMarker(InGameMazeTile tile, PlayerCharacter player)
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            player.LastTile = tile;

            MazeTilePath mazeTilePath = (MazeTilePath)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTilePath);
            if (mazeTilePath == null) return;

            PlayerMark playerMark = new PlayerMark(mazeTilePath.ConnectionScore);

            HandlePlayerMarkerSprite(tile, player.PlayerNumber, playerMark);
            HandlePlayerTileMarkerEnds(tile);
            HandleNumberOfUnmarkedTiles();

            tile.ResetPlayerMarkEndsRenderer();

            tile.TriggerTransformations();
        }
        else
        {
            PlayerMarksTileEvent playerMarksTileEvent = new PlayerMarksTileEvent();
            playerMarksTileEvent.SendPlayerMarksTileEvent(tile.GridLocation, player);
        }
    }

    public void LoadOverworld(string overworldName = "default")
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiPlayer)
        {
            PersistentGameManager.SetLastMazeLevelName(PersistentGameManager.CurrentSceneName);
            PersistentGameManager.SetCurrentSceneName(overworldName);
            PhotonNetwork.LoadLevel("Overworld");
        }
        else
        {
            LoadOverworldEvent loadOverworldEvent = new LoadOverworldEvent();
            loadOverworldEvent.SendLoadOverworldEvent(overworldName);
        }
    }

    public void LoadNextLevel(string pickedLevel)
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiPlayer)
        {
            MazeLevelData levelData = new JsonMazeLevelFileReader().ReadData<MazeLevelData>(pickedLevel);

            if (levelData == null)
            {
                Logger.Error($"Could not load maze level data for the randomly picked maze level {pickedLevel}");
            }

            UnloadLevel();
            SetupLevel(levelData);

            ScoreScreenContainer.Instance.CloseScoreScreenPanel();
        }
        else
        {
            LoadNextMazeLevelEvent loadNextLevelEvent = new LoadNextMazeLevelEvent();
            loadNextLevelEvent.SendLoadNextMazeLevelEvent(pickedLevel);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == PlayerMarksTileEvent.PlayerMarksTileEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            GridLocation tileLocation = new GridLocation((int)data[0], (int)data[1]);
            PlayerNumber playerNumber = (PlayerNumber)data[2];

            InGameMazeTile tile = Level.TilesByLocation[tileLocation] as InGameMazeTile;

            MazeTilePath mazeTilePath = (MazeTilePath)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTilePath);
            if (mazeTilePath == null) return;

            PlayerMark playerMark = new PlayerMark(mazeTilePath.ConnectionScore);

            HandlePlayerMarkerSprite(tile, playerNumber, playerMark);
            HandlePlayerTileMarkerEnds(tile);
            HandleNumberOfUnmarkedTiles();

            tile.ResetPlayerMarkEndsRenderer();

            tile.TriggerTransformations();
        } else if(eventCode == LoadNextMazeLevelEvent.LoadNextMazeLevelEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string pickedLevel = (string)data[0];
   
            MazeLevelData mazeLevelData = MazeLevelLoader.LoadMazeLevelData(pickedLevel);

            if (mazeLevelData == null)
            {
                Logger.Error($"Could not load maze level data for the randomly picked maze level {pickedLevel}");
            }

            PersistentGameManager.SetCurrentSceneName(pickedLevel);
            MazeLevelLoader.LoadMazeLevel(mazeLevelData);

            ScoreScreenContainer.Instance.CloseScoreScreenPanel();
        } else if (eventCode == LoadOverworldEvent.LoadOverworldEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string overworldName = (string)data[0];

            PersistentGameManager.SetLastMazeLevelName(PersistentGameManager.CurrentSceneName);
            PersistentGameManager.SetCurrentSceneName(PersistentGameManager.OverworldName);
            PhotonNetwork.LoadLevel("Overworld");
        }
    }

    public void ValidateSpawnpoints()
    {
        int playerStartLocations = Level.PlayerCharacterSpawnpoints.Count;
        if (playerStartLocations == 0)
        {
            Logger.Warning(Logger.Initialisation, "The level {0} does not have any player starting positions set up while it needs 2.", Level.Name);
        }
        else if (playerStartLocations == 1)
        {
            Logger.Warning(Logger.Initialisation, "The level {0} has only 1 player starting position set up while it needs 2.", Level.Name);
        }
        else if (playerStartLocations > 2)
        {
            Logger.Warning(Logger.Initialisation, "The level {0} has {1} player starting positions set up. There should be 2 positions.", Level.Name, playerStartLocations);
        }
    }

    public void OpenExit()
    {
        for (int i = 0; i < Level.MazeExits.Count; i++)
        {
            PlayerExit exit = Level.MazeExits[i];
            exit.OpenExit();
        }
    }

    public List<InGameMazeTile> GetTiles()
    {
        return Level.Tiles;
    }

    private void HandlePlayerMarkerSprite(MazeTile tile, PlayerNumber playerNumber, PlayerMark playerMark)
    {
        if (playerNumber == PlayerNumber.Player1)
        {
            playerMark.SetOwner(PlayerMarkOwner.Player1);
            tile.PlayerMarkRenderer.sprite = MazeSpriteManager.Instance.Player1TileMarker[playerMark.ConnectionScore - 1];
            tile.PlayerMark = playerMark;
        }
        else
        {
            playerMark.SetOwner(PlayerMarkOwner.Player2);
            tile.PlayerMarkRenderer.sprite = MazeSpriteManager.Instance.Player2TileMarker[playerMark.ConnectionScore - 1];
            tile.PlayerMark = playerMark;
        }
    }

    private void HandleNumberOfUnmarkedTiles()
    {
        NumberOfUnmarkedTiles--;
        Logger.Log(Logger.Level,"{0} unmarked tiles left", NumberOfUnmarkedTiles);

        if (NumberOfUnmarkedTiles == 0)
        {
            OpenExit();
            Logger.Warning(Logger.Level, "Open exits!");
        }
    }

    private void HandlePlayerTileMarkerEnds(MazeTile tile)
    {
        foreach (KeyValuePair<ObjectDirection, Tile> item in tile.Neighbours)
        {
            MazeTile neighbour = item.Value as MazeTile;

            if (!neighbour) continue;

            MazeTilePath mazeTilePath = (MazeTilePath)neighbour.GetBackgrounds().FirstOrDefault(background => background is MazeTilePath);
            if (mazeTilePath == null) continue;
            
            if (neighbour.PlayerMark != null && neighbour.PlayerMark.Owner != PlayerMarkOwner.None) continue;

            TileConnectionScoreInfo neighbourConnectionScoreInfo = NeighbourTileCalculator.MapNeighbourPlayerMarkEndsOfTile(neighbour);
            neighbour.PlayerMarkEndsRenderer.sprite = MazeSpriteManager.Instance.PlayerTileMarkerEdge[neighbourConnectionScoreInfo.SpriteNumber - 1];
        }
    }

    private void HandleSpawnpointMarkability()
    {
        if(GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>().Count < 2)
        {
            MazeTile spawnpoint1Tile = Level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].Tile as MazeTile;
            spawnpoint1Tile.TryMakeMarkable(false);

            MazeTile spawnpoint2Tile = Level.PlayerCharacterSpawnpoints[PlayerNumber.Player2].Tile as MazeTile;
            spawnpoint2Tile.TryMakeMarkable(true);
        }
    }
}
