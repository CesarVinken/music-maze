using System;

[Serializable]
public class SerialisableTilePathBackground : SerialisableTileBackground
{
    public SerialisableTilePathBackground(int pathConnectionScore)
    {
        TileBackgroundId = PathBackgroundCode;
        PathConnectionScore = pathConnectionScore;
    }
}