using CharacterType;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnpoint : CharacterSpawnpoint
{
    [SerializeField] private Sprite _enemySpawnpointSprite;
    public List<TileArea> TileAreas = new List<TileArea>();

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

        MazeLevelGameplayManager.Instance.Level.EnemyCharacterSpawnpoints.Add(this);
    }

    public void AddTileArea(TileArea tileArea)
    {
        TileAreas.Add(tileArea);
    }

    public void RemoveTileArea(TileArea tileArea)
    {
        TileAreas.Remove(tileArea);
    }
}
