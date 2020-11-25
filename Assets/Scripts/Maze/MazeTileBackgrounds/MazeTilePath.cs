using UnityEngine;

public class MazeTilePath : MonoBehaviour, IMazeTileBackground
{
    public int _pathConnectionScore;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public MazeTilePathType MazeTilePathType;

    public void SetSprite(int pathConnectionScore)
    {
        _pathConnectionScore = pathConnectionScore;
        _sprite = SpriteManager.Instance.DefaultPath[_pathConnectionScore - 1];
        _spriteRenderer.sprite = _sprite;
    }

    public void WithPathType(MazeTilePathType mazeTilePathType)
    {
        MazeTilePathType = mazeTilePathType;
    }

    public void WithPathConnectionScore(int score)
    {
        _pathConnectionScore = score;
        SetSprite(_pathConnectionScore);
    }

    //public void SetTile(Tile tile)
    //{
    //    if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

    //    Tile = tile;
    //    ParentId = tile.TileId;
    //}

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }
}
