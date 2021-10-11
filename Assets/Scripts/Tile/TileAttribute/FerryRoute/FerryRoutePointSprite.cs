using UnityEngine;

public class FerryRoutePointSprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Tile _tile;
    private FerryRouteDirection _direction;


    public void SetTile(Tile tile)
    {
        _tile = tile;
    }

    public void SetDirection(FerryRouteDirection ferryRouteDirection)
    {
        _direction = ferryRouteDirection;
        switch (_direction)
        {
            case FerryRouteDirection.Horizontal:
                SetLayerOrder(SpriteSortingOrderRegister.FerryRoutePointHorizontal);
                _spriteRenderer.sprite = MazeSpriteManager.Instance.FerryRouteSprites[4];
                break;
            case FerryRouteDirection.Vertical:
                SetLayerOrder(SpriteSortingOrderRegister.FerryRoutePointVertical);
                _spriteRenderer.sprite = MazeSpriteManager.Instance.FerryRouteSprites[5];
                break;
            default:
                Logger.Error($"Unknown ferry route direction {_direction}");
                break;
        }
    }

    private void SetLayerOrder(int sortingOrder)
    {
        _spriteRenderer.sortingOrder = sortingOrder;
    }
}
