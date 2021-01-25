using UnityEngine;

public class MazeTileBaseBackground : MonoBehaviour, IMazeTileBackground, ITransformable
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] private Sprite _sprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
        _spriteRenderer.sortingOrder = SpriteManager.BaseBackgroundSortingOrder;
    }

    public void WithPathConnectionScore(int score)
    {
        _sprite = SpriteManager.Instance.DefaultMazeTileBackground[0];
        _spriteRenderer.sprite = _sprite;
    }

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    public void TriggerTransformation()
    {
        _spriteRenderer.sprite = SpriteManager.Instance.DefaultMazeTileBackgroundColourful[0];
    }
}
