
using UnityEngine;

public class TileBaseGround : MonoBehaviour, ITileBackground, ITileConnectable
{
    public Tile Tile;
    public string ParentId;
    public IBaseBackgroundType TileGroundType;

    [SerializeField] protected Sprite _sprite;
    [SerializeField] protected TileSpriteContainer _tileSpriteContainer;

    protected int _connectionScore = -1;
    protected int _spriteNumber = -1; 
    protected int _sortingOrder;

    public int ConnectionScore { get => _connectionScore; set => _connectionScore = value; }
    public int SpriteNumber { get => _spriteNumber; set => _spriteNumber = value; }

    public string GetSubtypeAsString()
    {
        return TileGroundType.Name;
    }

    public void WithType(IBackgroundType tileGroundType)
    {
        TileGroundType = tileGroundType as IBaseBackgroundType;
    }

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    public virtual void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
        //_spriteRenderer.sortingOrder = SpriteManager.BaseBackgroundSortingOrder;
    }

    public virtual void WithConnectionScoreInfo(TileConnectionScoreInfo score)
    { }
}
