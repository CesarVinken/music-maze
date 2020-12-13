using UnityEngine;

public class MazeTilePath : MonoBehaviour, IMazeTileBackground, ITileConnectable
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] private Sprite _sprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public MazeTilePathType MazeTilePathType;

    private int _connectionScore = -1;
    private int _spriteNumber = -1;

    public int ConnectionScore { get => _connectionScore; set => _connectionScore = value; }
    public int SpriteNumber { get => _spriteNumber; set => _spriteNumber = value; }

    public void WithPathType(MazeTilePathType mazeTilePathType)
    {
        MazeTilePathType = mazeTilePathType;
    }

    public void WithConnectionScoreInfo(TileConnectionScoreInfo connectionScoreInfo)
    {
        ConnectionScore = connectionScoreInfo.RawConnectionScore;
        SpriteNumber = connectionScoreInfo.SpriteNumber;

        _sprite = SpriteManager.Instance.DefaultPath[SpriteNumber - 1];
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
