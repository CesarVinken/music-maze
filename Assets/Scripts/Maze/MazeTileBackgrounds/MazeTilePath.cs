using UnityEngine;

public class MazeTilePath : MonoBehaviour, IMazeTileBackground
{
    public Tile Tile;
    public string ParentId;

    public int PathConnectionScore;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public MazeTilePathType MazeTilePathType;

    public void WithPathType(MazeTilePathType mazeTilePathType)
    {
        MazeTilePathType = mazeTilePathType;
    }

    public void WithPathConnectionScore(int score)
    {
        PathConnectionScore = score;
        _sprite = SpriteManager.Instance.DefaultPath[PathConnectionScore - 1];
        _spriteRenderer.sprite = _sprite;
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
