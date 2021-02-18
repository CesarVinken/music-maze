using UnityEngine;

public class MazeEntry : MonoBehaviour, ITileAttribute
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private int _sortingOrderBase = 500;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public Tile Tile;
    public string ParentId;

    public virtual void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        _spriteRenderer.sortingOrder = (int)(_sortingOrderBase - transform.position.y) * 10;
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
