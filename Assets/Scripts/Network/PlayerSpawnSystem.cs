using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab = null;

    private static List<Transform> _spawnPoints = new List<Transform>();

    private int nextIndex = 0; // when player spawns in, the server needs to increment this index

    public static void AddSpawnPoint(Transform transform)
    {
        _spawnPoints.Add(transform);
        _spawnPoints = _spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }
    public static void RemoveSpawnPoint(Transform transform) => _spawnPoints.Remove(transform);

    public override void OnStartServer() => MazeNetworkManager.OnServerReadied += SpawnPlayer;

    [ServerCallback]
    private void OnDestroy() => MazeNetworkManager.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = _spawnPoints.ElementAtOrDefault(nextIndex);

        if(spawnPoint == null)
        {
            Logger.Error("Missing spawn point for player {0}", nextIndex);
            return;
        }

        GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoints[nextIndex].position, _spawnPoints[nextIndex].rotation);

        NetworkServer.Spawn(playerInstance, conn);

        nextIndex++;
    }
}
