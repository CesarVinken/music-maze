using UnityEngine;

public class TileObstacle : MonoBehaviour, IMazeTileAttribute, ITileConnectable, ITransformable
{
    public Tile Tile;
    public string ParentId;
    
    public ObstacleType ObstacleType;

    [SerializeField] protected SpriteRenderer _spriteRenderer;

    private int _connectionScore = -1;
    private int _spriteNumber = -1;

    private int _sortingOrderBase = 500;
    private const float _sortingOrderCalculationOffset = 1.5f;

    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public int ConnectionScore { get => _connectionScore; set => _connectionScore = value; }
    public int SpriteNumber { get => _spriteNumber; set => _spriteNumber = value; }

    public virtual void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        if(ObstacleType == ObstacleType.Bush)
        {
            _spriteRenderer.sprite = SpriteManager.Instance.DefaultWall[0];
        }

        _spriteRenderer.sortingOrder = (int)(_sortingOrderBase - transform.position.y - _sortingOrderCalculationOffset) * 10;
    }

    public virtual void TriggerTransformation()
    {
        if (ObstacleType == ObstacleType.Bush)
        {
            _spriteRenderer.sprite = SpriteManager.Instance.DefaultWallColourful[SpriteNumber - 1];
        }
    }

    public virtual void WithConnectionScoreInfo(TileConnectionScoreInfo obstacleConnectionScoreInfo)
    {
        ConnectionScore = obstacleConnectionScoreInfo.RawConnectionScore;
        SpriteNumber = obstacleConnectionScoreInfo.SpriteNumber;

        _spriteRenderer.sprite = SpriteManager.Instance.DefaultWall[SpriteNumber - 1];
    }

    public void WithObstacleType(ObstacleType obstacleType)
    {
        ObstacleType = obstacleType;
    }

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
    }
    
    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    public string GetSubtypeAsString()
    {
        return ObstacleType.ToString();
    }
}
