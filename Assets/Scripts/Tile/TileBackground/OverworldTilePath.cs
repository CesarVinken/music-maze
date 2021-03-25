public class OverworldTilePath : TilePath, ITileBackground, ITileConnectable
{
    public override void WithConnectionScoreInfo(TileConnectionScoreInfo connectionScoreInfo)
    {
        ConnectionScore = connectionScoreInfo.RawConnectionScore;
        SpriteNumber = connectionScoreInfo.SpriteNumber;

        _sprite = OverworldSpriteManager.Instance.Path[SpriteNumber - 1];
        _spriteRenderer.sprite = _sprite;
    }
}
