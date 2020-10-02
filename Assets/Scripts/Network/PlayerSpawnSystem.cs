using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    //[SerializeField] private GameObject _playerPrefab = null;

    private static List<Transform> _spawnPoints = new List<Transform>();

    private int nextIndex = 0; // when player spawns in, the server needs to increment this index

    public static void AddSpawnPoint(Transform transform)
    {
        _spawnPoints.Add(transform);
        _spawnPoints = _spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }
    public static void RemoveSpawnPoint(Transform transform) => _spawnPoints.Remove(transform);

    public override void OnStartServer() => MazeNetworkManager.OnServerReadied += SpawnPlayer; // maybe move to game manager and create spawnsystem there with prefab?

    [ServerCallback]
    private void OnDestroy() => MazeNetworkManager.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Logger.Log("Spawn player");
        Transform spawnPoint = _spawnPoints.ElementAtOrDefault(nextIndex);

        if(spawnPoint == null)
        {
            Logger.Error("Missing spawn point for player {0}", nextIndex);
            return;
        }

        //GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoints[nextIndex].position, _spawnPoints[nextIndex].rotation);
        GridLocation gr = GridLocation.VectorToGrid(_spawnPoints[nextIndex].position);
        Logger.Log("GridLocation for {0},{1} is {2},{3}", _spawnPoints[nextIndex].position.x, _spawnPoints[nextIndex].position.y, gr.X, gr.Y);
        GameObject playerInstance = CharacterManager.Instance.RegisterCharacter(new CharacterBlueprint(CharacterType.Player), GridLocation.VectorToGrid(_spawnPoints[nextIndex].position));

        NetworkServer.Spawn(playerInstance, conn);

        nextIndex++;
    }
}
