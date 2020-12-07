using System;

[Serializable]
public class SerialisablePlayerOnlyAttribute : SerialisableTileAttribute
{
    public SerialisablePlayerOnlyAttribute()
    {
        TileAttributeId = PlayerOnlyAttributeCode;
    }
}