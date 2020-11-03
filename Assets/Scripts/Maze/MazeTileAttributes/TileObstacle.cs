using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObstacle : MonoBehaviour, IMazeTileAttribute
{
    public Tile Tile;
    public string ParentId;
    
    public ObstacleType ObstacleType;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    //[SerializeField] private List<Sprite> _defaultWall = new List<Sprite>();
    [SerializeField] private Sprite[] _defaultWall;
    [SerializeField] private int _obstacleConnectionScore; // on which sides does this obstacle border other obstacles

    public void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        if(ObstacleType == ObstacleType.Wall)
        {
            _spriteRenderer.sprite = _defaultWall[0];
        }
    }

    public void WithObstacleConnectionScore(int obstacleConnectionScore)
    {
        _obstacleConnectionScore = obstacleConnectionScore;

        if (obstacleConnectionScore == -1) return;

        _spriteRenderer.sprite = _defaultWall[obstacleConnectionScore - 1];
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
}
