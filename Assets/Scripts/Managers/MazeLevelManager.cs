using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MazeLevelManager : MonoBehaviour, IOnEventCallback
{
    public static MazeLevelManager Instance;
    public MazeLevel Level;

    public GameObject TilePrefab;
    public GameObject TileObstaclePrefab;
    public GameObject PlayerExitPrefab;
    public GameObject PlayerSpawnpointPrefab;
    public GameObject EnemySpawnpointPrefab;

    public void Awake()
    {
        Guard.CheckIsNull(TilePrefab, "TilePrefab", gameObject);
        Guard.CheckIsNull(TileObstaclePrefab, "TileObstaclePrefab", gameObject);
        Guard.CheckIsNull(PlayerExitPrefab, "PlayerExitPrefab", gameObject);
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

    public void LoadLevel(MazeLevelData mazeLevelData)
    {
        Level = MazeLevel.Create(mazeLevelData);
    }

    public void UnloadLevel()
    {
        if(TilesContainer.Instance)
        {
            Destroy(TilesContainer.Instance.gameObject);
        }

        TilesContainer.Instance = null;

        CharacterManager.Instance.UnloadCharacters();
        SceneObjectManager.Instance.UnloadSceneObjects();

        if(Level != null)
        {
            Logger.Log(Logger.Initialisation, "Unload level {0}", Level);
            Level.Tiles.Clear();
            Level.TilesByLocation.Clear();
            CameraController.Instance.ResetCamera();
            Level = null;
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

            if (player.PlayerNumber == PlayerNumber.Player1)
            {
                tile.PlayerMark.sprite = MainCanvas.Instance.Player1TileMarker;
            }
            else
            {
                tile.PlayerMark.sprite = MainCanvas.Instance.Player2TileMarker;
            }

            HandleNumberOfUnmarkedTiles();
        }
        else
        {
            PlayerMarksTileEvent playerMarksTileEvent = new PlayerMarksTileEvent();
            playerMarksTileEvent.SendPlayerMarksTileEvent(tile.GridLocation, player);
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

            Tile tile = Level.TilesByLocation[tileLocation]; // add check

            if (playerNumber == PlayerNumber.Player1)
            {
                tile.PlayerMark.sprite = MainCanvas.Instance.Player1TileMarker;
            }
            else
            {
                tile.PlayerMark.sprite = MainCanvas.Instance.Player2TileMarker;
            }

            HandleNumberOfUnmarkedTiles();
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

    private void HandleNumberOfUnmarkedTiles()
    {
        Level.NumberOfUnmarkedTiles--;
        Logger.Log(Logger.Level,"{0} unmarked tiles left", Level.NumberOfUnmarkedTiles);

        if (Level.NumberOfUnmarkedTiles == 0)
        {
            OpenExit();
            Logger.Warning(Logger.Level, "Open exit!");
        }
    }
}
