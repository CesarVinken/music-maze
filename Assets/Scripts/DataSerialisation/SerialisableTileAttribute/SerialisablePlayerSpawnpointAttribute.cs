using System;

[Serializable]
public class SerialisablePlayerSpawnpointAttribute : SerialisableTileAttribute
{
    public SerialisablePlayerSpawnpointAttribute()
    {
        TileAttributeId = PlayerSpawnpointCode;
    }
}
