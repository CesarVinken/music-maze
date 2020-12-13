using System;

[Serializable]
public class SerialisablePlayerExitAttribute : SerialisableTileAttribute
{
    public SerialisablePlayerExitAttribute(TileConnectionScoreInfo ConnectionScoreInfo)
    {
        TileAttributeId = PlayerExitCode;
        ObstacleConnectionScore = ConnectionScoreInfo.RawConnectionScore;
        SpriteNumber = ConnectionScoreInfo.SpriteNumber;
    }
}