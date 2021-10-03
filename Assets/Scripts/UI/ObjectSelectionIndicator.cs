
using Character;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Direction _direction;
    private Camera _cameraToUse = null;
    private Transform _relatedWorldSpaceObject; //eg., the Ferry object that we should follow for the indicator, notwithstanding any camera movements
    private Vector2 _baseVectorWorldPositionOffset = new Vector2(0.5f, 0.5f);
    private ObjectSelectionIndicatorType _objectSelectionIndicatorType;
    public PlayerCharacter PlayerCharacter;

    public virtual void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_imageRenderer", gameObject);
    }

    public void Initialise(PlayerCharacter player, Direction direction, Transform relatedWorldSpaceObject, ObjectSelectionIndicatorType objectSelectionIndicatorType)
    {
        _relatedWorldSpaceObject = relatedWorldSpaceObject;
        _objectSelectionIndicatorType = objectSelectionIndicatorType;
        _direction = direction;
        PlayerCharacter = player;

        if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer &&
            player.PlayerNumber == PlayerNumber.Player2)
        {
            _cameraToUse = CameraManager.Instance.CameraControllers[1].GetCamera();
        }
        else
        {
            _cameraToUse = CameraManager.Instance.CameraControllers[0].GetCamera();
        }

        _spriteRenderer.sortingOrder = GetSortingLayerOrder();
        _spriteRenderer.sprite = GetSprite();
    }

    public void Unset()
    {
        _relatedWorldSpaceObject = null;
    }

    private Sprite GetSprite()
    {
        switch (_objectSelectionIndicatorType)
        {
            case ObjectSelectionIndicatorType.Ferry:
                if(_direction == Direction.Left || _direction == Direction.Right)
                {
                    return ObjectSelectionIndicatorPool.Instance.FerrySelectionIndicatorSprites[0];
                }
                return ObjectSelectionIndicatorPool.Instance.FerrySelectionIndicatorSprites[1];
            default:
                Logger.Error($"Unknown ObjectSelectionIndicatorType {_objectSelectionIndicatorType}");
                return null;
        }
    }

    private int GetSortingLayerOrder()
    {
        switch (_objectSelectionIndicatorType)
        {
            case ObjectSelectionIndicatorType.Ferry:
                return SpriteSortingOrderRegister.ObjectSelectionIndicator;
            default:
                return 0;
        }
    }
}
