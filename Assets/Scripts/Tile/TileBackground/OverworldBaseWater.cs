using UnityEngine;

public class OverworldTileBaseWater : TileWater, ITileBackground
{
    [SerializeField] private TileSpriteContainer _tileSpriteContainer;

    private int _sortingOrder;

    public override void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;

        _sortingOrder = SpriteManager.BaseWaterSortingOrder;
        _tileSpriteContainer.SetSortingOrder(_sortingOrder);
    }

    public override void WithConnectionScoreInfo(TileConnectionScoreInfo connectionScoreInfo)
    {
        ConnectionScore = connectionScoreInfo.RawConnectionScore;
        SpriteNumber = connectionScoreInfo.SpriteNumber;

        _sprite = OverworldSpriteManager.Instance.DefaultOverworldTileWater[SpriteNumber - 1];
        _tileSpriteContainer.SetSprite(_sprite);
    }
}
