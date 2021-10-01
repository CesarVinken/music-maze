
using Character;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionIndicator : MonoBehaviour
{
    [SerializeField] private Image _imageRenderer;
    private Direction _direction;
    private Camera _cameraToUse = null;
    private Transform _relatedWorldSpaceObject; //eg., the Ferry object that we should follow for the indicator, notwithstanding any camera movements
    private Vector2 _baseVectorWorldPositionOffset = new Vector2(0.5f, 0.5f);
    private ObjectSelectionIndicatorType _objectSelectionIndicatorType;
    public PlayerCharacter PlayerCharacter;

    public virtual void Awake()
    {
        Guard.CheckIsNull(_imageRenderer, "_imageRenderer", gameObject);
    }

    public void Update()
    {
        UpdatePosition();
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

        _imageRenderer.sprite = GetSprite();
        UpdatePosition();
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
                return ObjectSelectionIndicatorPool.Instance.FerrySelectionIndicatorSprite;
            default:
                Logger.Error($"Unknown ObjectSelectionIndicatorType {_objectSelectionIndicatorType}");
                return null;
        }
    }

    private void UpdatePosition()
    {
        if (_relatedWorldSpaceObject != null)
        {
            Vector2 positionAdjustedForScreenPoint = _cameraToUse.WorldToScreenPoint(new Vector2(_relatedWorldSpaceObject.position.x + _baseVectorWorldPositionOffset.x, _relatedWorldSpaceObject.position.y + +_baseVectorWorldPositionOffset.y));
            transform.position = new Vector2(positionAdjustedForScreenPoint.x, positionAdjustedForScreenPoint.y);
        }
    }

}
