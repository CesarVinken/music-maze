using System;

[Serializable]
public class SerialisableTileObstacleAttribute : SerialisableTileAttribute
{
    public SerialisableTileObstacleAttribute(int obstacleConnectionScore)
    {
        TileAttributeId = ObstacleAttributeCode;
        ObstacleConnectionScore = obstacleConnectionScore;
    }
}