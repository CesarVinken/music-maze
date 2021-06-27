using System.Collections.Generic;
using UnityEngine;

public class MusicInstrumentCase : MonoBehaviour, ITileAttribute
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _musicInstrumentCaseSprites;

    private int _sortingOrderBase = 500;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public void Awake()
    {
        if(_musicInstrumentCaseSprites.Length == 0)
        {
            Logger.Error("Could not find MusicInstrumentCaseSprites");
        }
        Guard.CheckIsNull(_musicInstrumentCaseSprites, "_musicInstrumentCaseSprite", gameObject);

        _spriteRenderer.sprite = _musicInstrumentCaseSprites[0];
    }

    public void OpenCase()
    {
        _spriteRenderer.sprite = _musicInstrumentCaseSprites[1];
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
