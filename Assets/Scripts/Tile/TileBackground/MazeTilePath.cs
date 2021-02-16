public class MazeTilePath : TilePath, ITileBackground, ITileConnectable
{  
    public override void WithConnectionScoreInfo(TileConnectionScoreInfo connectionScoreInfo)
    {
        ConnectionScore = connectionScoreInfo.RawConnectionScore;
        SpriteNumber = connectionScoreInfo.SpriteNumber;

        _sprite = MazeSpriteManager.Instance.DefaultPath[SpriteNumber - 1];
        _spriteRenderer.sprite = _sprite;
    }
}
