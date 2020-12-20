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

        MazeLevelManager.Instance.Level.PlayerCharacterSpawnpoints.Add(this);
    }
}
