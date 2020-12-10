using UnityEngine;

public class TileObstacle : MonoBehaviour, IMazeTileAttribute, ITileConnectable
{
    public Tile Tile;
    public string ParentId;
    
    public ObstacleType ObstacleType;

    [SerializeField] protected SpriteRenderer _spriteRenderer;

    private int _connectionScore = -1;

    public int ConnectionScore { get => _connectionScore; set => _connectionScore = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        if(ObstacleType == ObstacleType.Wall)
        {
            _spriteRenderer.sprite = SpriteManager.Instance.DefaultWall[0];
        }
    }

    public virtual void WithConnectionScore(int obstacleConnectionScore)
    {
        ConnectionScore = obstacleConnectionScore;

        if (obstacleConnectionScore == -1) return;
        _spriteRenderer.sprite = SpriteManager.Instance.DefaultWall[ConnectionScore - 1];
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
