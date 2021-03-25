using UnityEngine;

public class TilePath : MonoBehaviour, ITileConnectable
{
    public Tile Tile;
    public string ParentId;

    public IPathType TilePathType;

    [SerializeField] protected Sprite _sprite;
    [SerializeField] protected SpriteRenderer _spriteRenderer;

    protected int _connectionScore = -1;
    protected int _spriteNumber = -1;

    public int ConnectionScore { get => _connectionScore; set => _connectionScore = value; }
    public int SpriteNumber { get => _spriteNumber; set => _spriteNumber = value; }

    public void WithType(IBackgroundType tilePathType)
    {
        TilePathType = tilePathType as IPathType;

        if(TilePathType == null)
        {
            Logger.Error("Attempted to set illigal path type. Cast failed.");
        }
    }

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    public string GetSubtypeAsString()
    {
        return TilePathType.Name;
    }

    public virtual void WithConnectionScoreInfo(TileConnectionScoreInfo connectionScoreInfo)
    {
    }

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
        _spriteRenderer.sortingOrder = SpriteManager.PathSortingOrder;
    }
}