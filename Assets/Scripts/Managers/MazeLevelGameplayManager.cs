using Character;
using DataSerialisation;
using ExitGames.Client.Photon;
using Gameplay;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class MazeLevelGameplayManager : MonoBehaviour, IOnEventCallback, IGameplayManager
{
    public static MazeLevelGameplayManager Instance;
    public InGameMazeLevel Level;
    public EditorMazeLevel EditorLevel;

    public List<CharacterSpawnpoint> PlayerCharacterSpawnpoints = new List<CharacterSpawnpoint>();
    public List<EnemySpawnpoint> EnemyCharacterSpawnpoints = new List<EnemySpawnpoint>();

    public event Action CompleteMazeLevelEvent;
    public event Action AllPathsAreMarkedEvent;

    public GameObject EditorTilePrefab;
    public GameObject InGameTilePrefab;

    [SerializeField] private GameObject _loopedAnimationEffectPrefab;
    [SerializeField] private GameObject _oneTimeAnimationEffectPrefab;
    [SerializeField] private GameObject _notesPlayMusicEffectPrefab;

    [SerializeField] private GameObject _tileBaseGroundPrefab;
    [SerializeField] private GameObject _tileBaseWaterPrefab;
    [SerializeField] private GameObject _tilePathPrefab;

    [SerializeField] private GameObject _bridgePiecePrefab;
    [SerializeField] private GameObject _bridgeEdgePrefab;
    [SerializeField] private GameObject _enemySpawnpointPrefab;
    [SerializeField] private GameObject _ferryRoutePrefab;
    [SerializeField] private GameObject _musicInstrumentCasePrefab;
    [SerializeField] private GameObject _playerExitPrefab;
    [SerializeField] private GameObject _playerOnlyPrefab;
    [SerializeField] private GameObject _playerSpawnpointPrefab;
    [SerializeField] private GameObject _sheetmusicPrefab;
    [SerializeField] private GameObject _tileObstaclePrefab;

    [SerializeField] private GameObject _cornerFillerPrefab;

    public int NumberOfUnmarkedTiles = -1;

    public GameObject GetEffectAnimationPrefab(AnimationEffect animationEffect)
    {
        switch (animationEffect)
        {
            case AnimationEffect.EmmonCaught:
                return _oneTimeAnimationEffectPrefab;
            case AnimationEffect.ExitOpenExplosion:
                return _oneTimeAnimationEffectPrefab;
            case AnimationEffect.FaeCaught:
                return _oneTimeAnimationEffectPrefab;
            case AnimationEffect.NotesPlayMusic:
                return _notesPlayMusicEffectPrefab;
            case AnimationEffect.StartledSpinner:
                return _loopedAnimationEffectPrefab;
            case AnimationEffect.SmokeExplosion:
                return _oneTimeAnimationEffectPrefab;
            default:
                Logger.Error($"No prefab was set up for animationEffect {animationEffect}");
                return null;
        }
    }

    public GameObject GetTileAttributePrefab<T>() where T : ITileAttribute
    {
        switch (typeof(T))
        {
            case Type bridgePiecePrefab when bridgePiecePrefab == typeof(BridgePiece):
                return _bridgePiecePrefab;
            case Type bridgeEdgePrefab when bridgeEdgePrefab == typeof(BridgeEdge):
                return _bridgeEdgePrefab;
            case Type enemySpawnpoint when enemySpawnpoint == typeof(EnemySpawnpoint):
                return _enemySpawnpointPrefab;
            case Type ferryRoutePrefab when ferryRoutePrefab == typeof(FerryRoute):
                return _ferryRoutePrefab;
            case Type musicInstrumentCasePrefab when musicInstrumentCasePrefab == typeof(MusicInstrumentCase):
                return _musicInstrumentCasePrefab;
            case Type playerExit when playerExit == typeof(PlayerExit):
                return _playerExitPrefab;
            case Type playerOnly when playerOnly == typeof(PlayerOnly):
                return _playerOnlyPrefab;
            case Type playerSpawnpoint when playerSpawnpoint == typeof(PlayerSpawnpoint):
                return _playerSpawnpointPrefab;
            case Type sheetmusicPrefab when sheetmusicPrefab == typeof(Sheetmusic):
                return _sheetmusicPrefab;
            case Type tileObstacle when tileObstacle == typeof(TileObstacle):
                return _tileObstaclePrefab;
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
            case Type cornerFiller when cornerFiller == typeof(TileCornerFiller):
                return _cornerFillerPrefab;
            default:
                Logger.Error($"Could not find a prefab for the tile background type of {typeof(T)}");
                return null;
        }
    }

    public void Awake()
    {
        Guard.CheckIsNull(_oneTimeAnimationEffectPrefab, "_oneTimeAnimationEffectPrefab", gameObject);
        Guard.CheckIsNull(_loopedAnimationEffectPrefab, "_loopedAnimationEffectPrefab", gameObject);
        Guard.CheckIsNull(_notesPlayMusicEffectPrefab, "_notesPlayMusicEffectPrefab", gameObject);

        Guard.CheckIsNull(EditorTilePrefab, "EditorTilePrefab", gameObject);
        Guard.CheckIsNull(InGameTilePrefab, "InGameTilePrefab", gameObject);
        Guard.CheckIsNull(_tileBaseGroundPrefab, "TileBaseBackgroundPrefab", gameObject);
        Guard.CheckIsNull(_tilePathPrefab, "TilePathPrefab", gameObject);
        Guard.CheckIsNull(_tileObstaclePrefab, "TileObstaclePrefab", gameObject);
        Guard.CheckIsNull(_playerExitPrefab, "PlayerExitPrefab", gameObject);
        Guard.CheckIsNull(_playerOnlyPrefab, "PlayerOnlyPrefab", gameObject);
        Guard.CheckIsNull(_playerSpawnpointPrefab, "PlayerSpawnpointPrefab", gameObject);
        Guard.CheckIsNull(_enemySpawnpointPrefab, "EnemySpawnpointPrefab", gameObject);
        Guard.CheckIsNull(_cornerFillerPrefab, "CornerFillerPrefab", gameObject);
        Guard.CheckIsNull(_bridgePiecePrefab, "BridgePiecePrefab", gameObject);
        Guard.CheckIsNull(_musicInstrumentCasePrefab, "MusicInstrumentCasePrefab", gameObject);
        Guard.CheckIsNull(_ferryRoutePrefab, "FerryRoutePrefab", gameObject);
        Guard.CheckIsNull(_bridgeEdgePrefab, "BridgeEdgePrefab", gameObject);

        Instance = this;
        GameManager.Instance.GameplayManager = this;
    }

    public void Start()
    {
        AllPathsAreMarkedEvent += OnAllPathsAreMarked;

        if (!EditorManager.InEditor)
        {
            MazeLevelMainScreenOverlayCanvas.Instance.SpawnCountdownTimer();
        }
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
        IEnumerator coroutine = SetupCoroutine();

        StartCoroutine(coroutine);
    }

    public void SetupLevelForEditor(MazeLevelData mazeLevelData)
    {
        EditorLevel = EditorMazeLevel.Create(mazeLevelData);


        InitialiseEditorTileBackgrounds();
        InitialiseEditorTileAttributes();

        MainScreenOverlayCanvas.Instance.ResetBlackOutSquares();
        CameraManager.Instance.ResetCameras();
        CameraManager.Instance.SetPanLimits();
    }

    public IEnumerator SetupCoroutine()
    {
        yield return new WaitForSeconds(.2f); // This waiting time should be dealt with more efficiently. Currently it is there to make sure that the characters are spawned in 

        MainScreenOverlayCanvas.Instance.BlackOutSquaresToClear();
        GameManager.Instance.CharacterManager.SpawnCharacters();

        CameraManager.Instance.SetPanLimits();
        CameraManager.Instance.FocusCamerasOnPlayer();

        //GameManager.Instance.CharacterManager.UnfreezeCharacters();

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
        Level.FerryRoutes.Clear();
        CameraManager.Instance.ResetCameras();
        Level = null;

        NumberOfUnmarkedTiles = -1;
    }

    private void InitialiseTileAttributes()
    {
        for (int i = 0; i < Level.Tiles.Count; i++)
        {
            InGameMazeTile tile = Level.Tiles[i] as InGameMazeTile;
            tile.InitialiseTileAttributes();
        }
    }

    private void InitialiseEditorTileAttributes()
    {
        for (int i = 0; i < EditorLevel.Tiles.Count; i++)
        {
            EditorMazeTile tile = EditorLevel.Tiles[i] as EditorMazeTile;
            tile.InitialiseTileAttributes();
        }
    }

    private void InitialiseEditorTileBackgrounds()
    {
        for (int i = 0; i < EditorLevel.Tiles.Count; i++)
        {
            EditorMazeTile tile = EditorLevel.Tiles[i] as EditorMazeTile;
            tile.InitialiseTileBackgrounds();
        }
    }

    // Previously tried solution with collision detection on all separate clients for all players(instead of events). 
    // But the result was that some tile marking got skipped if the clients skipped walking over them because of a bad connection.
    // This way we can be sure all tiles are getting marked.
    public void SetTileMarker(InGameMazeTile tile, PlayerCharacter player)
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
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

    public void PlayerCollisionWithMusicInstrumentCase(InGameMazeTile tile, MazePlayerCharacter player, MusicInstrumentCase musicInstrumentCase)
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        { 
            musicInstrumentCase.PlayerCollisionOnTile(player);
        }
        else // network multiplayer
        {
            PlayerCollidesWithMusicInstrumentCaseEvent playerCollidesWithMusicInstrumentCaseEvent = new PlayerCollidesWithMusicInstrumentCaseEvent();
            playerCollidesWithMusicInstrumentCaseEvent.SendPlayerCollidesWithMusicInstrumentCaseEvent(tile.GridLocation, player);
        }
    }

    public void EnemyCollisionWithMusicInstrumentCase(InGameMazeTile tile, EnemyCharacter enemy, MusicInstrumentCase musicInstrumentCase)
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
       GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            musicInstrumentCase.EnemyCollisionOnTile(enemy);
        }
        else // network multiplayer
        {
            EnemyCollidesWithMusicInstrumentCaseEvent enemyCollidesWithMusicInstrumentCaseEvent = new EnemyCollidesWithMusicInstrumentCaseEvent();
            enemyCollidesWithMusicInstrumentCaseEvent.SendEnemyCollidesWithMusicInstrumentCaseEvent(tile.GridLocation, enemy);
        }
    }

    public void PlayerCollisionWithSheetmusic(InGameMazeTile tile, MazePlayerCharacter player, Sheetmusic sheetmusic)
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            sheetmusic.PlayerCollisionOnTile(player);
        }
        else // network multiplayer
        {
            PlayerCollidesWithSheetmusicEvent playerCollidesWithSheetmusicEvent = new PlayerCollidesWithSheetmusicEvent();
            playerCollidesWithSheetmusicEvent.SendPlayerCollidesWithSheetmusicEvent(tile.GridLocation, player);
        }
    }

    public void EnemyCollisionWithSheetmusic(InGameMazeTile tile, EnemyCharacter enemy, Sheetmusic sheetmusic)
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            sheetmusic.EnemyCollisionOnTile(enemy);
        }
        else // network multiplayer
        {
            EnemyCollidesWithSheetmusicEvent enemyCollidesWithSheetmusicEvent = new EnemyCollidesWithSheetmusicEvent();
            enemyCollidesWithSheetmusicEvent.SendEnemyCollidesWithSheetmusicEvent(tile.GridLocation, enemy);
        }
    }

    public void LoadOverworld(string overworldName = "default")
    {
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            PersistentGameManager.SetLastMazeLevelName(PersistentGameManager.CurrentSceneName);
            PersistentGameManager.SetCurrentSceneName(overworldName);

            IEnumerator loadLevelCoroutine = LoadOverworldCoroutine("Overworld");
            StartCoroutine(loadLevelCoroutine);
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
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            MazeLevelData levelData = new JsonMazeLevelFileReader().ReadData<MazeLevelData>(pickedLevel);

            if (levelData == null)
            {
                Logger.Error($"Could not load maze level data for the randomly picked maze level {pickedLevel}");
            }

            IEnumerator loadLevelCoroutine = LoadLevelCoroutine("Maze", levelData);
            StartCoroutine(loadLevelCoroutine);
        }
        else
        {
            LoadNextMazeLevelEvent loadNextLevelEvent = new LoadNextMazeLevelEvent();
            loadNextLevelEvent.SendLoadNextMazeLevelEvent(pickedLevel);
        }
    }

    public void StartNextSceneRoutine(string overworldName)
    {
        IEnumerator loadOverworldCoroutine = LoadOverworldCoroutine(overworldName);
        StartCoroutine(loadOverworldCoroutine);
    }

    private IEnumerator LoadOverworldCoroutine(string overworldName)
    {
        MainScreenOverlayCanvas.Instance.BlackOutSquaresToBlack();

        while (MainScreenOverlayCanvas.Instance.BlackOutSquares[0].BlackStatus != BlackStatus.Black)
        {
            yield return null;
        }

        PhotonNetwork.LoadLevel(overworldName);
    }

    private IEnumerator LoadLevelCoroutine(string levelName, MazeLevelData levelData)
    {
        MainScreenOverlayCanvas.Instance.BlackOutSquaresToBlack();

        while (MainScreenOverlayCanvas.Instance.BlackOutSquares[0].BlackStatus != BlackStatus.Black)
        {
            yield return null;
        }
        UnloadLevel();

        MazeScoreScreenContainer.Instance.CloseScoreScreenPanel();

        SetupLevel(levelData);

        MainScreenOverlayCanvas.Instance.BlackOutSquaresToClear();
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == EventCode.PlayerMarksTileEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            PlayerMarksTileEventHandler playerMarksTileEventHandler = new PlayerMarksTileEventHandler(this);
            playerMarksTileEventHandler.Handle(data);            
        } 
        else if(eventCode == EventCode.LoadNextMazeLevelEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            LoadNextMazeLevelEventHandler loadNextSceneEventHandler = new LoadNextMazeLevelEventHandler(this);
            loadNextSceneEventHandler.Handle(data);
        } 
        else if (eventCode == EventCode.LoadOverworldEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            LoadOverworldEventHandler loadOverworldEventHandler = new LoadOverworldEventHandler(this);
            loadOverworldEventHandler.Handle(data);
        } 
        else if (eventCode == EventCode.PlayerCollidesWithMusicInstrumentCaseEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            PlayerCollidesWithMusicInstrumentCaseEventHandler playerCollidesWithMusicInstrumentCaseEventHandler = new PlayerCollidesWithMusicInstrumentCaseEventHandler(this);
            playerCollidesWithMusicInstrumentCaseEventHandler.Handle(data);
        } 
        else if (eventCode == EventCode.EnemyCollidesWithMusicInstrumentCaseEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            EnemyCollidesWithMusicInstrumentCaseEventHandler enemyCollidesWithMusicInstrumentCaseEventHandler = new EnemyCollidesWithMusicInstrumentCaseEventHandler(this);
            enemyCollidesWithMusicInstrumentCaseEventHandler.Handle(data);
        }
        else if (eventCode == EventCode.PlayerCollidesWithSheetmusicEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            PlayerCollidesWithSheetmusicEventHandler playerCollidesWithSheetmusicEventHandler = new PlayerCollidesWithSheetmusicEventHandler(this);
            playerCollidesWithSheetmusicEventHandler.Handle(data);
        } 
        else if (eventCode == EventCode.EnemyCollidesWithSheetmusicEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            EnemyCollidesWithSheetmusicEventHandler enemyCollidesWithSheetmusicEventHandler = new EnemyCollidesWithSheetmusicEventHandler(this);
            enemyCollidesWithSheetmusicEventHandler.Handle(data);    
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

    public void OnAllPathsAreMarked()
    {
        for (int i = 0; i < Level.MazeExits.Count; i++)
        {
            PlayerExit exit = Level.MazeExits[i];
            exit.OpenExit();
        }
    }

    public List<Tile> GetTiles()
    {
        return Level.Tiles;
    }

    public void HandlePlayerMarkerSprite(MazeTile tile, PlayerNumber playerNumber, PlayerMark playerMark)
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

    public void HandleNumberOfUnmarkedTiles()
    {
        NumberOfUnmarkedTiles--;
//        Logger.Log(Logger.Level,"{0} unmarked tiles left", NumberOfUnmarkedTiles);

        if (NumberOfUnmarkedTiles == 0)
        {
            TriggerEndGame();
            Logger.Warning(Logger.Level, "Open exits!");
        }
    }

    public void HandlePlayerTileMarkerEnds(MazeTile tile)
    {
        foreach (KeyValuePair<Direction, Tile> item in tile.Neighbours)
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

    public void HandleSpawnpointMarkability()
    {
        if(GameManager.Instance.CharacterManager.GetPlayerCount() < 2)
        {
            MazeTile spawnpoint1Tile = Level.PlayerCharacterSpawnpoints[PlayerNumber.Player1].Tile as MazeTile;
            spawnpoint1Tile.TryMakeMarkable(false);

            MazeTile spawnpoint2Tile = Level.PlayerCharacterSpawnpoints[PlayerNumber.Player2].Tile as MazeTile;
            spawnpoint2Tile.TryMakeMarkable(true);
        }
    }

    /// <summary>
    /// EVENT INVOcATION
    /// </summary>

    // end game happens when all paths have been marked
    public void TriggerEndGame()
    {
        AllPathsAreMarkedEvent.Invoke();
    }

    public void CompleteMazeLevel()
    {
        CompleteMazeLevelEvent.Invoke();
    }
}
