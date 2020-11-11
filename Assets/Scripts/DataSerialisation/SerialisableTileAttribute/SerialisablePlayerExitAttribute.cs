using System;

[Serializable]
public class SerialisablePlayerExitAttribute : SerialisableTileAttribute
{
    public SerialisablePlayerExitAttribute(int obstacleConnectionScore)
    {
        TileAttributeId = PlayerExitCode;
        ObstacleConnectionScore = obstacleConnectionScore;
    }
}