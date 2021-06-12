using UnityEngine;

public class TileWater : MonoBehaviour, ITileBackground
{
    public Tile Tile;
    public string ParentId;

    public IBaseBackgroundType TileWaterType;

    [SerializeField] protected Sprite _sprite;
    [SerializeField] protected SpriteRenderer _spriteRenderer;

    protected int _connectionScore = -1;
    protected int _spriteNumber = -1;

    public int ConnectionScore { get => _connectionScore; set => _connectionScore = value; }
    public int SpriteNumber { get => _spriteNumber; set => _spriteNumber = value; }

    public void WithType(IBackgroundType tileWaterType)
    {
        TileWaterType = tileWaterType as IBaseBackgroundType;
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
        _spriteRenderer.sortingOrder = SpriteManager.BaseWaterSortingOrder;
    }

    public void MakeUnwalkable()
    {
        gameObject.layer = 8; // set layer to Unwalkable, which is layer 8. Should not be hardcoded
    }

    public void SetWalkabilityForBridge(BridgePieceDirection walkDirection)
    {
        if (walkDirection == BridgePieceDirection.Horizontal)
        {
            gameObject.layer = 6; // set layer to Horizontal Walkability
        }
        else
        {
            gameObject.layer = 7; // set layer to Vertical Walkability
        }
    }
}
