using UnityEngine;

public class PlayerSpawnpoint : CharacterSpawnpoint
{
    [SerializeField] private Sprite _playerSpawnpointSprite;

    public override void Awake()
    {
        base.Awake();
        Guard.CheckIsNull(_playerSpawnpointSprite, "_playerSpawnpointSprite", gameObject);

        _spriteRenderer.sprite = _playerSpawnpointSprite;
    }

    public override void RegisterSpawnpoint()
    {
        GridLocation = GridLocation.VectorToGrid(transform.position);

        if (EditorManager.InEditor) return;

        int registeredSpawnpoints = GameManager.Instance.CurrentGameLevel.PlayerCharacterSpawnpoints.Count;

        if(registeredSpawnpoints == 0)
        {
            GameManager.Instance.CurrentGameLevel.PlayerCharacterSpawnpoints.Add(PlayerNumber.Player1, this);
        }
        else if(registeredSpawnpoints == 1)
        {
            GameManager.Instance.CurrentGameLevel.PlayerCharacterSpawnpoints.Add(PlayerNumber.Player2, this);
        }
        else
        {
            Logger.Error($"Found {registeredSpawnpoints} registered spawnpoints, but there can be only a maximum of 2");
        }

        Logger.Log("Registered spawnpoint");
    }
}
