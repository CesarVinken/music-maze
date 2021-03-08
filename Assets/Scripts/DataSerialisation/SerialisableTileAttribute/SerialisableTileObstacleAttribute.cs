using System;

[Serializable]
public class SerialisableTileObstacleAttribute : ISerialisableTileAttribute
{
    public int ConnectionScore;
    public int SpriteNumber;

    public SerialisableTileObstacleAttribute(TileConnectionScoreInfo obstacleConnectionScoreInfo)
    {
        ConnectionScore = obstacleConnectionScoreInfo.RawConnectionScore;
        SpriteNumber = obstacleConnectionScoreInfo.SpriteNumber;
    }
}