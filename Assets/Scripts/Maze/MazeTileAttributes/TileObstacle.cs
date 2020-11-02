using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObstacle : MonoBehaviour, IMazeTileAttribute
{
    public Tile Tile;
    public string ParentId;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);
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
