using System;

[Serializable]
public class SerialisableEnemySpawnpointAttribute : SerialisableTileAttribute
{
    public SerialisableEnemySpawnpointAttribute()
    {
        TileAttributeId = EnemySpawnpointCode;
    }
}