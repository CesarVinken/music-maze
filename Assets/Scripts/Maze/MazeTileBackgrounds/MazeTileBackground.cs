using UnityEngine;

public class MazeTileBackground : MonoBehaviour, IMazeTileBackground
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void SetSprite(int connectionScore)
    {
        _sprite = SpriteManager.Instance.DefaultMazeTileBackground[0];
        _spriteRenderer.sprite = _sprite;

        transform.position = new Vector3(transform.position.x, transform.position.y, 1f); //Adjusted Z value to have it further away from the camera
    }
}
