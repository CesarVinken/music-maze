using System;

[Serializable]
public class SerialisableTileObstacleAttribute : SerialisableTileAttribute
{
    public SerialisableTileObstacleAttribute(TileConnectionScoreInfo obstacleConnectionScoreInfo)
    {
        TileAttributeId = ObstacleAttributeCode;
        ObstacleConnectionScore = obstacleConnectionScoreInfo.RawConnectionScore;
        SpriteNumber = obstacleConnectionScoreInfo.SpriteNumber;
    }
}