public class TileConnectionScoreInfo
{
    public int RawConnectionScore;
    public int SpriteNumber;

    public TileConnectionScoreInfo(int rawConnectionScore)  // in case the connection score and the displayed sprite are the same
    {
        RawConnectionScore = rawConnectionScore;
        SpriteNumber = rawConnectionScore;
    }

    public TileConnectionScoreInfo(int rawConnectionScore, int spriteNumber)  // in case the displayed sprite is a variation based onthe connection score
    {
        RawConnectionScore = rawConnectionScore;
        SpriteNumber = spriteNumber;
    }
}