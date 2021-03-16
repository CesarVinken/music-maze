using System;

[Serializable]
public class SerialisableTileBaseWater : ISerialisableTileBackground
{
    public int TileConnectionScore;
    public SerialisableTileBaseWater(int pathConnectionScore)
    {
        TileConnectionScore = pathConnectionScore;
    }
}
