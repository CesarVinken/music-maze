using System.Collections;
using UnityEngine;

public class OverworldTileBaseBackground : MonoBehaviour, ITileBackground
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] private TileSpriteContainer _tileSpriteContainer;

    private int _sortingOrder;

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;

        _sortingOrder = OverworldSpriteManager.BaseBackgroundSortingOrder;
        _tileSpriteContainer.SetSortingOrder(_sortingOrder);
    }

    public void WithPathConnectionScore(int score)
    {
        Sprite sprite = OverworldSpriteManager.Instance.DefaultMazeTileBackground[0];
        _tileSpriteContainer.SetSprite(sprite);
    }

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }
}
