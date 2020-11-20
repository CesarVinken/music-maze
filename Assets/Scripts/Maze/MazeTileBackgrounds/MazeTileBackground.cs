using UnityEngine;

public class MazeTileBackground : MonoBehaviour
{
    [SerializeField] private int _pathConnectionScore;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void SetSprite(int pathConnectionScore)
    {
        _pathConnectionScore = pathConnectionScore;
        _sprite = SpriteManager.Instance.DefaultPath[_pathConnectionScore];
        _spriteRenderer.sprite = _sprite;
    }
}
