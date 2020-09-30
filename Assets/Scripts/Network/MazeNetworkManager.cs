using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class MazeNetworkManager : NetworkManager
{
    [SerializeField] private int _minPlayers = 2;
    [SerializeField] private int _maxPlayers = 2;
    [Scene] [SerializeField] private string _menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer _roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer _gamePlayerPrefab = null;
    [SerializeField] private GameObject _playerSpawnSystem = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    public List<NetworkRoomPlayer> RoomPlayers { get; } = new List<NetworkRoomPlayer>();
    public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();

    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach(var prefab in spawnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if(SceneManager.GetActiveScene().path != _menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == _menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayer roomPlayerLobbyInstance = Instantiate(_roomPlayerPrefab);

            roomPlayerLobbyInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerLobbyInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            NetworkRoomPlayer player = conn.identity.GetComponent<NetworkRoomPlayer>();

            RoomPlayers.Remove(player);
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
        GamePlayers.Clear();
    }

    public void TryStartGame()
    {
        NetworkRoomPlayer playerLeader = null;

        foreach(var player in RoomPlayers)
        {
            if (IsGameReadyToStart())
            {
                if (player.GetLeaderRoomPlayer())
                {
                    playerLeader = player;
                    break;
                }
            }
        }
        if (playerLeader)
        {
            Logger.Log("Trigger Start game command");
            playerLeader.CmdStartGame();
        }
    }

    public bool IsGameReadyToStart()
    {
        if(numPlayers < _minPlayers) { return false; }

        foreach(var player in RoomPlayers)
        {
            if(!player.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        Logger.Log("RoomPlayers.count: {0}", RoomPlayers.Count);
        Logger.Log("STARTGAME!");
        if(SceneManager.GetActiveScene().path == _menuScene)
        {
            if(!IsGameReadyToStart()) { return;  }

            ServerChangeScene("TestLoadScene");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // from menu to game
        if (SceneManager.GetActiveScene().path == _menuScene) // scene name starts with level prefix
            //if (SceneManager.GetActiveScene().path == _menuScene && newSceneName.StartsWith("TestLoadScene")) // scene name starts with level prefix
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                NetworkConnection conn = RoomPlayers[i].connectionToClient;
                NetworkGamePlayer gamePlayerInstance = Instantiate(_gamePlayerPrefab);
                gamePlayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        //if(sceneName.StartsWith("TestLoadScene")) // scene name starts with level prefix
        //{
            GameObject playerSpawnSystemInstance = Instantiate(_playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        //}
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}