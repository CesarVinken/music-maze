using System;

[Serializable]
public class SerialisablePlayerExitAttribute : SerialisableTileAttribute
{
    public SerialisablePlayerExitAttribute(TileConnectionScoreInfo ConnectionScoreInfo)
    {
        TileAttributeId = PlayerExitCode;
        ConnectionScore = ConnectionScoreInfo.RawConnectionScore;
        SpriteNumber = ConnectionScoreInfo.SpriteNumber;
    }
}