using System;

[Serializable]
public class SerialisableTileObstacleAttribute : SerialisableTileAttribute
{
    public SerialisableTileObstacleAttribute(TileConnectionScoreInfo obstacleConnectionScoreInfo)
    {
        TileAttributeId = ObstacleAttributeCode;
        ConnectionScore = obstacleConnectionScoreInfo.RawConnectionScore;
        SpriteNumber = obstacleConnectionScoreInfo.SpriteNumber;
    }
}