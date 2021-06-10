using UnityEngine;

public class TileCornerFiller : MonoBehaviour, ITileBackground
{
    public Tile Tile;
    public string ParentId;
    public IBaseBackgroundType TileGroundType;
    public TileCorner TileCorner { get; private set; }

    [SerializeField] protected Sprite _sprite;
    [SerializeField] protected TileSpriteContainer _tileSpriteContainer;

    protected int _spriteNumber = -1;
    protected int _sortingOrder = -25; // Sprite order should be above the water (atm -30), but below the regular land/grass (atm -20) tiles

    public int SpriteNumber { get => _spriteNumber; set => _spriteNumber = value; }

    public void Awake()
    {
        _tileSpriteContainer.SetSortingOrder(_sortingOrder);
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
    }

    public void WithType(IBackgroundType tileGroundType)
    {
        TileGroundType = tileGroundType as IBaseBackgroundType;
    }

    public void WithCorner(TileCorner tileCorner)
    {
        TileCorner = tileCorner;
        SetSprite();
    }

    private void SetSprite()
    {
        switch (TileCorner)
        {
            case TileCorner.RightUp:
                _sprite = GameManager.Instance.SpriteManager.DefaultTileCornerFiller[0];
                break;
            case TileCorner.RightDown:
                _sprite = GameManager.Instance.SpriteManager.DefaultTileCornerFiller[1];
                break;
            case TileCorner.LeftDown:
                _sprite = GameManager.Instance.SpriteManager.DefaultTileCornerFiller[2];
                break;
            case TileCorner.LeftUp:
                _sprite = GameManager.Instance.SpriteManager.DefaultTileCornerFiller[3];
                break;
            default:
                break;
        }
        _tileSpriteContainer.SetSprite(_sprite);
    }
}