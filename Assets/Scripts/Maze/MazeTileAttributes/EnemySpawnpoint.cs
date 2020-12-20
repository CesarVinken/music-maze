using UnityEngine;

public class EnemySpawnpoint : CharacterSpawnpoint
{
    [SerializeField] private Sprite _enemySpawnpointSprite;

    public override void Awake()
    {
        base.Awake();
        Guard.CheckIsNull(_enemySpawnpointSprite, "_enemySpawnpointSprite", gameObject);

        _spriteRenderer.sprite = _enemySpawnpointSprite;
    }

    public override void RegisterSpawnpoint()
    {
        GridLocation = GridLocation.VectorToGrid(transform.position);

        if (EditorManager.InEditor) return;

        MazeLevelManager.Instance.Level.EnemyCharacterSpawnpoints.Add(this);
    }
}
