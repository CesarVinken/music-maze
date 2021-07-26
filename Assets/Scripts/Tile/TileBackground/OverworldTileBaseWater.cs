using System.Collections;
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

        IEnumerator animateWaterCoroutine = AnimateWater();
        StartCoroutine(animateWaterCoroutine);
    }

    private IEnumerator AnimateWater()
    {
        _animateWater = true;
        yield return new WaitForSeconds(1);

        while (_animateWater)
        {
            _currentWaterSpriteNumber = GetNextWaterSpriteNumber();
            _tileSpriteContainer.SetSprite(OverworldSpriteManager.Instance.DefaultOverworldTileWater[_currentWaterSpriteNumber]);

            yield return new WaitForSeconds(1);
        }
    }

    private int GetNextWaterSpriteNumber()
    {
        if (_currentWaterSpriteNumber < OverworldSpriteManager.Instance.DefaultOverworldTileWater.Length - 1)
        {
            return _currentWaterSpriteNumber + 1;
        }
        else
        {
            return 0;
        }
    }

}
