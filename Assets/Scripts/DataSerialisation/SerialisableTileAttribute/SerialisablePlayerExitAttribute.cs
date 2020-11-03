using System;

[Serializable]
public class SerialisablePlayerExitAttribute : SerialisableTileAttribute
{
    public SerialisablePlayerExitAttribute()
    {
        TileAttributeId = PlayerExitCode;
    }
}