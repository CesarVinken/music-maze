using UnityEngine;

public class BridgeEdge : MonoBehaviour, ITileAttribute
{
    public Tile Tile;
    public string ParentId;

    public BridgeType BridgeType;
    public Direction EdgeSide;

    [SerializeField] private TileSpriteContainer _tileSpriteContainer;
    private int _sortingOrderBase = 500;
    private int _sortingOrder;

    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_tileSpriteContainer, "_tileSpriteContainer", gameObject);

        _sortingOrder = (int)(_sortingOrderBase - transform.position.y) * 10 + 1;
    }

    public void Remove()
    {
        Logger.Log($"Destory bridge edge at {Tile.GridLocation.X}, {Tile.GridLocation.Y}");
        Tile.RemoveBridgeEdge(this);
        Destroy(this);
        Destroy(gameObject);
    }

    public void WithBridgeEdgeSide(Direction edgeSide)
    {
        EdgeSide = edgeSide;
    }

    public void WithBridgeType(BridgeType bridgeType)
    {
        BridgeType = bridgeType;
    }

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
    }

    public void SetSprite()
    {
        switch (EdgeSide)
        {
            case Direction.Up:
                _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[5]);
                break;
            case Direction.Right:
                _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[2]);
                break;
            case Direction.Down:
                _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[4]);
                break;
            case Direction.Left:
                _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[3]);
                break;
            default:
                break;
        }
    }
}
