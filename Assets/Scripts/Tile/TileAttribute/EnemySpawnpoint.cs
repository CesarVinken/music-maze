using CharacterType;
using UnityEngine;

public class EnemySpawnpoint : CharacterSpawnpoint
{
    [SerializeField] private Sprite _enemySpawnpointSprite;

    // TODO:: Implement way to customise type for different enemy spawnpoints
    private ICharacter EnemyType = new EvilViolin();

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

        
        CharacterBlueprint = new CharacterBlueprint(EnemyType);

        MazeLevelManager.Instance.Level.EnemyCharacterSpawnpoints.Add(this);
    }
}
