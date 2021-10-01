using Character;
using UnityEngine;
using UnityEngine.UI;

public class FerryDirectionIndicator : MonoBehaviour
{
    [SerializeField] private Image _imageRenderer;
    public Direction Direction;
    private Ferry _ferryParent;
    private Vector2 _baseVectorWorldPositionOffset;
    private Camera _cameraToUse = null;

    public virtual void Awake()
    {
        Guard.CheckIsNull(_imageRenderer, "_imageRenderer", gameObject);
    }

    public void Initialise(PlayerCharacter player, Direction direction, Tile tile, Ferry ferryParent)
    {
        Direction = direction;
        _ferryParent = ferryParent;

        if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer &&
            player.PlayerNumber == PlayerNumber.Player2)
        {
            _cameraToUse = CameraManager.Instance.CameraControllers[1].GetCamera();
        }
        else
        {
            _cameraToUse = CameraManager.Instance.CameraControllers[0].GetCamera();
        }

        switch (Direction)
        {
            case Direction.Right:
                _baseVectorWorldPositionOffset = new Vector2(1.3f, 0.5f);
                _imageRenderer.sprite = FerryDirectionIndicatorPool.Instance.FerryDirectionIndicatorSprites[0];
                break;
            case Direction.Up:
                _baseVectorWorldPositionOffset = new Vector2(0.5f, 1.3f);
                _imageRenderer.sprite = FerryDirectionIndicatorPool.Instance.FerryDirectionIndicatorSprites[3];
                break;
            case Direction.Down:
                _baseVectorWorldPositionOffset = new Vector2(0.5f, -0.3f);
                _imageRenderer.sprite = FerryDirectionIndicatorPool.Instance.FerryDirectionIndicatorSprites[1];
                break;
            case Direction.Left:
                _baseVectorWorldPositionOffset = new Vector2(-0.3f, 0.5f);
                _imageRenderer.sprite = FerryDirectionIndicatorPool.Instance.FerryDirectionIndicatorSprites[2];
                break;
            default:
                break;
        }

        UpdatePosition();
    }

    public void Unset()
    {
        _ferryParent = null;
    }

    public void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (_ferryParent != null)
        {
            Vector2 positionAdjustedForScreenPoint = _cameraToUse.WorldToScreenPoint(new Vector2(_ferryParent.transform.position.x + _baseVectorWorldPositionOffset.x, _ferryParent.transform.position.y + +_baseVectorWorldPositionOffset.y));
            transform.position = new Vector2(positionAdjustedForScreenPoint.x, positionAdjustedForScreenPoint.y);
        }
    }

    public void SetRendererAlpha(float alphaValue)
    {
        Color color = _imageRenderer.color;
        _imageRenderer.color = new Color(color.r, color.g, color.b, alphaValue);
    }
}
