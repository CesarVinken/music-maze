using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnly : MonoBehaviour, IMazeTileAttribute
{
    public Tile Tile;
    public string ParentId; 
    
    public PlayerOnlyType PlayerOnlyType;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        if (PlayerOnlyType == PlayerOnlyType.Bush)
        {
            _spriteRenderer.sprite = SpriteManager.Instance.Bush[0];
        }
    }

    public void WithPlayerOnlyType(PlayerOnlyType playerOnlyType)
    {
        PlayerOnlyType = playerOnlyType;
    }

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
    }

}
