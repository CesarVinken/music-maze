using UnityEngine;

public class MazeTileBaseBackground : MonoBehaviour, IMazeTileBackground
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] private Sprite _sprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
    }

    public void WithPathConnectionScore(int score)
    {
        _sprite = SpriteManager.Instance.DefaultMazeTileBackground[0];
        _spriteRenderer.sprite = _sprite;

        transform.position = new Vector3(transform.position.x, transform.position.y, 1f); //Adjusted Z value to have it further away from the camera
    }

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }
}
