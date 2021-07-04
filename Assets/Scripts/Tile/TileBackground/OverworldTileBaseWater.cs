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

        _sortingOrder = SpriteSortingOrderRegister.BaseWaterSortingOrder;
        _tileSpriteContainer.SetSortingOrder(_sortingOrder);

        Sprite sprite = OverworldSpriteManager.Instance.DefaultOverworldTileWater[0];
        _tileSpriteContainer.SetSprite(sprite);
    }
}
