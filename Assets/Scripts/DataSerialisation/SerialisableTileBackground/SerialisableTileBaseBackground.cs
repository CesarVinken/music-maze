using System;

[Serializable]
public class SerialisableTileBaseBackground : SerialisableTileBackground
{
    public SerialisableTileBaseBackground(int connectionScore)
    {
        TileBackgroundId = BaseBackgroundCode;
        TileConnectionScore = connectionScore;
    }
}
