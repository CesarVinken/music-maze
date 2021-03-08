using System;

[Serializable]
public class SerialisablePlayerExitAttribute : ISerialisableTileAttribute
{
    public int ConnectionScore;
    public int SpriteNumber;

    public SerialisablePlayerExitAttribute(TileConnectionScoreInfo ConnectionScoreInfo)
    {
        ConnectionScore = ConnectionScoreInfo.RawConnectionScore;
        SpriteNumber = ConnectionScoreInfo.SpriteNumber;
    }
}