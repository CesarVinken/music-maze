﻿using System;
using UnityEngine;

public class PlayerOnly : MonoBehaviour, IMazeTileAttribute
{
    public Tile Tile;
    public string ParentId; 
    
    public PlayerOnlyType PlayerOnlyType;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    private int _sortingOrderBase = 500; // MAKE SURE that tile should be in front of tile marker and path layers AND player
    private const float _sortingOrderCalculationOffset = .5f;

    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        if (PlayerOnlyType == PlayerOnlyType.Bush)
        {
            _spriteRenderer.sprite = SpriteManager.Instance.Bush[0];
        }

        _spriteRenderer.sortingOrder = (int)(_sortingOrderBase - transform.position.y - _sortingOrderCalculationOffset) * 10 + 1; // plus 1 should place it before a character when it is on the same y as the character
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
