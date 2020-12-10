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
    public MazeLevel Level;

    public List<CharacterSpawnpoint> PlayerCharacterSpawnpoints = new List<CharacterSpawnpoint>();
    public List<CharacterSpawnpoint> EnemyCharacterSpawnpoints = new List<CharacterSpawnpoint>();
    
    public GameObject TilePrefab;
    public GameObject TileBaseBackgroundPrefab;
    public GameObject TilePathPrefab;
    public GameObject TileObstaclePrefab;
    public GameObject PlayerExitPrefab;
    public GameObject PlayerOnlyPrefab;
    public GameObject PlayerSpawnpointPrefab;
    public GameObject EnemySpawnpointPrefab;


    public int NumberOfUnmarkedTiles = -1;

    public GameObject GetTileAttributePrefab<T>() where T : IMazeTileAttribute
    {
        switch (typeof(T))
        {
            case Type playerExit when playerExit == typeof(PlayerExit):
                return PlayerExitPrefab;
            case Type tileObstacle when tileObstacle == typeof(TileObstacle):
                return TileObstaclePrefab;
            case Type playerOnly when playerOnly == typeof(PlayerOnly):
                return PlayerOnlyPrefab;
            case Type playerSpawnpoint when playerSpawnpoint == typeof(PlayerSpawnpoint):
                return PlayerSpawnpointPrefab;
            case Type enemySpawnpoint when enemySpawnpoint == typeof(EnemySpawnpoint):
                return EnemySpawnpointPrefab;

            default:
                Logger.Error($"Could not find a prefab for the tile attribute type of {typeof(T)}");
                return null ;
        }
    }

    public void Awake()
    {
        Guard.CheckIsNull(TilePrefab, "TilePrefab", gameObject);
        Guard.CheckIsNull(TileBaseBackgroundPrefab, "TileBaseBackgroundPrefab", gameObject);
        Guard.CheckIsNull(TilePathPrefab, "TilePathPrefab", gameObject);
        Guard.CheckIsNull(TileObstaclePrefab, "TileObstaclePrefab", gameObject);
        Guard.CheckIsNull(PlayerExitPrefab, "PlayerExitPrefab", gameObject);
        Guard.CheckIsNull(PlayerOnlyPrefab, "PlayerOnlyPrefab", gameObject);
        Guard.CheckIsNull(PlayerSpawnpointPrefab, "PlayerSpawnpointPrefab", gameObject);
        Guard.CheckIsNull(EnemySpawnpointPrefab, "EnemySpawnpointPrefab", gameObject);

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
        Level = MazeLevel.Create(mazeLevelData);

        InitialiseTileAttributes();

        Logger.Log("Start scan...");
        IEnumerator coroutine = ScanCoroutine();

        StartCoroutine(coroutine);
    }

    public void SetupLevelForEditor(MazeLevelData mazeLevelData)
    {
        Level = MazeLevel.Create(mazeLevelData);

        InitialiseTileBackgrounds();
        InitialiseTileAttributes();

        MainCanvas.Instance.BlackOutSquare.ResetToDefault();
        CameraController.Instance.ResetCamera();
        CameraController.Instance.SetPanLimits(MazeLevelManager.Instance.Level.LevelBounds);
    }

    public IEnumerator ScanCoroutine()
    {
        yield return new WaitForSeconds(.2f); // This waiting time should be dealt with more efficiently. Currently it is there to make sure that the characters are spawned in 
        MainCanvas.Instance.BlackOutSquare.ResetToDefault();
        CharacterManager.Instance.SpawnCharacters();
        CameraController.Instance.SetPanLimits(Level.LevelBounds);
        CameraController.Instance.FocusOnPlayer();

        AstarPath.active.Scan();    // We should only scan once all the tiles are loaded with their correct (walkable) attributes and obstacles
        yield return new WaitForSeconds(.4f);

        // start movement of all actors that depend on the updated pathfinding only after the scan.
        CharacterManager.Instance.UnfreezeCharacters();
    }

    public void UnloadLevel()
    {
        if (Level == null) return;

        if(TilesContainer.Instance)
        {
            Destroy(TilesContainer.Instance.gameObject);
        }

        TilesContainer.Instance = null;

        CharacterManager.Instance.UnloadCharacters();
        SceneObjectManager.Instance.UnloadSceneObjects();

        Logger.Log(Logger.Initialisation, "Unload level {0}", Level);
        Level.Tiles.Clear();
        Level.TilesByLocation.Clear();
        CameraController.Instance.ResetCamera();
        Level = null;

        NumberOfUnmarkedTiles = -1;
    }

    public void InitialiseTileAttributes()
    {
        for (int i = 0; i < Level.Tiles.Count; i++)
        {
            Tile tile = Level.Tiles[i];
            tile.InitialiseTileAttributes();
        }    
    }

    public void InitialiseTileBackgrounds()
    {
        for (int i = 0; i < Level.Tiles.Count; i++)
        {
            Tile tile = Level.Tiles[i];
            tile.InitialiseTileBackgrounds();
        }
    }

    // Previously tried solution with collision detection on all separate clients for all players(instead of events). 
    // But the result was that some tile marking got skipped if the clients skipped walking over them because of a bad connection.
    // This way we can be sure all tiles are getting marked.
    public void SetTileMarker(Tile tile, PlayerCharacter player)
    {
        if (GameManager.Instance.GameType == GameType.SinglePlayer)
        {
            player.LastTile = tile;

            MazeTilePath mazeTilePath = (MazeTilePath)tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTilePath);
            if (mazeTilePath == null) return;

            PlayerMark playerMark = new PlayerMark(mazeTilePath.ConnectionScore);

            HandlePlayerMarkerSprite(tile, player.PlayerNumber, playerMark);
            HandlePlayerTileMarkerEnds(tile);
            HandleNumberOfUnmarkedTiles();

            tile.ResetPlayerMarkEndsRenderer();
        }
        else
        {
            PlayerMarksTileEvent playerMarksTileEvent = new PlayerMarksTileEvent();
            playerMarksTileEvent.SendPlayerMarksTileEvent(tile.GridLocation, player);
        }
    }

    public void LoadNextLevel(string pickedLevel)
    {
        if (GameManager.Instance.GameType == GameType.SinglePlayer)
        {
            JsonMazeLevelFileReader levelReader = new JsonMazeLevelFileReader();
            MazeLevelData levelData = levelReader.ReadLevelData(pickedLevel);

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

            Tile tile = Level.TilesByLocation[tileLocation];

            MazeTilePath mazeTilePath = (MazeTilePath)tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTilePath);
            if (mazeTilePath == null) return;

            PlayerMark playerMark = new PlayerMark(mazeTilePath.ConnectionScore);

            HandlePlayerMarkerSprite(tile, playerNumber, playerMark);
            HandlePlayerTileMarkerEnds(tile);
            HandleNumberOfUnmarkedTiles();

            tile.ResetPlayerMarkEndsRenderer();
        } else if(eventCode == LoadNextMazeLevelEvent.LoadNextMazeLevelEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string pickedLevel = (string)data[0];

            
            MazeLevelData mazeLevelData = MazeLevelLoader.LoadMazeLevelData(pickedLevel);

            if (mazeLevelData == null)
            {
                Logger.Error($"Could not load maze level data for the randomly picked maze level {pickedLevel}");
            }

            MazeLevelLoader.LoadMazeLevel(mazeLevelData);

            ScoreScreenContainer.Instance.CloseScoreScreenPanel();
        }
    }

    public void ValidateSpawnpoints()
    {
        int playerStartLocations = Level.PlayerCharacterSpawnpoints.Count;
        if (playerStartLocations == 0)
        {
            Logger.Warning(Logger.Initialisation, "The level {0} does not have any player starting positions set up while it needs 2.", Level.MazeName);
        }
        else if (playerStartLocations == 1)
        {
            Logger.Warning(Logger.Initialisation, "The level {0} has only 1 player starting position set up while it needs 2.", Level.MazeName);
        }
        else if (playerStartLocations > 2)
        {
            Logger.Warning(Logger.Initialisation, "The level {0} has {1} player starting positions set up. There should be 2 positions.", Level.MazeName, playerStartLocations);
        }
    }

    public void OpenExit()
    {
        for (int i = 0; i < Level.MazeExits.Count; i++)
        {
            PlayerExit exit = Level.MazeExits[i];
            exit.OpenExit();
            // open exit
        }
    }

    private void HandlePlayerMarkerSprite(Tile tile, PlayerNumber playerNumber, PlayerMark playerMark)
    {
        if (playerNumber == PlayerNumber.Player1)
        {
            playerMark.SetOwner(PlayerMarkOwner.Player1);
            tile.PlayerMarkRenderer.sprite = SpriteManager.Instance.Player1TileMarker[playerMark.ConnectionScore - 1];
            tile.PlayerMark = playerMark;
        }
        else
        {
            playerMark.SetOwner(PlayerMarkOwner.Player2);
            tile.PlayerMarkRenderer.sprite = SpriteManager.Instance.Player2TileMarker[playerMark.ConnectionScore - 1];
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

    private void HandlePlayerTileMarkerEnds(Tile tile)
    {
        foreach (KeyValuePair<ObjectDirection, Tile> item in tile.Neighbours)
        {
            Tile neighbour = item.Value;

            MazeTilePath mazeTilePath = (MazeTilePath)neighbour.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTilePath);
            if (mazeTilePath == null) continue;
            
            if (neighbour.PlayerMark != null && neighbour.PlayerMark.Owner != PlayerMarkOwner.None) continue;

            int neighbourConnectionScore = NeighbourTileCalculator.MapNeighbourPlayerMarkEndsOfTile(neighbour);
            neighbour.PlayerMarkEndsRenderer.sprite = SpriteManager.Instance.PlayerTileMarkerEdge[neighbourConnectionScore - 1];
        }

    }
}
