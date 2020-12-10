using UnityEngine;

public class MazeTilePath : MonoBehaviour, IMazeTileBackground, ITileConnectable
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] private Sprite _sprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public MazeTilePathType MazeTilePathType;

    private int connectionScore = -1;

    public int ConnectionScore { get => connectionScore; set => connectionScore = value; }

    public void WithPathType(MazeTilePathType mazeTilePathType)
    {
        MazeTilePathType = mazeTilePathType;
    }

    public void WithConnectionScore(int score)
    {
        ConnectionScore = score;
        _sprite = SpriteManager.Instance.DefaultPath[ConnectionScore - 1];
        _spriteRenderer.sprite = _sprite;
    }

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
        _spriteRenderer.sortingOrder = SpriteManager.PathSortingOrder;
    }

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    public string GetSubtypeAsString()
    {
        return MazeTilePathType.ToString();
    }
}
