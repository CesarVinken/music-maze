using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObstacle : MonoBehaviour, IMazeTileAttribute
{
    public Tile Tile;
    public string ParentId;
    
    public ObstacleType ObstacleType;

    [SerializeField] protected SpriteRenderer _spriteRenderer;


    public int ObstacleConnectionScore = -1; // on which sides does this obstacle border other obstacles

    public void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        if(ObstacleType == ObstacleType.Wall)
        {
            _spriteRenderer.sprite = SpriteManager.Instance.DefaultWall[0];
        }
    }

    public virtual void WithObstacleConnectionScore(int obstacleConnectionScore)
    {
        ObstacleConnectionScore = obstacleConnectionScore;

        if (obstacleConnectionScore == -1) return;

        _spriteRenderer.sprite = SpriteManager.Instance.DefaultWall[ObstacleConnectionScore - 1];
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
