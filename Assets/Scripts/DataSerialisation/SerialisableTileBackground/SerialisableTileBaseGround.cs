using System;

[Serializable]
public class SerialisableTileBaseGround : ISerialisableTileBackground
{
    public int TileConnectionScore;
    public SerialisableTileBaseGround(int pathConnectionScore)
    {
        TileConnectionScore = pathConnectionScore;
    }
}
