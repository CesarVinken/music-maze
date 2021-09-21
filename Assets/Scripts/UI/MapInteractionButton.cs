using Character;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MapInteractionButton : MonoBehaviour, IMapInteractionButton
    {
        [SerializeField] private Image _image;
        [SerializeField] private Text _mapTextLabel;
        [SerializeField] private Button _mapInteractionButton;

        private Vector2 _buttonWorldBasePosition;
        [SerializeField] public PlayerCharacter TriggerPlayer;
        private Camera _cameraToUse = null;

        private void Awake()
        {
            Guard.CheckIsNull(_image, "image", gameObject);
            Guard.CheckIsNull(_mapTextLabel, "MapTextLabel", gameObject);
        }

        public void Update()
        {
            if (gameObject.activeSelf)
            {
                Vector2 positionAdjustedForScreenPoint = _cameraToUse.WorldToScreenPoint(new Vector2(_buttonWorldBasePosition.x + 0.5f, _buttonWorldBasePosition.y + 1));
                transform.position = new Vector2(positionAdjustedForScreenPoint.x, positionAdjustedForScreenPoint.y);
            }
        }

        private void SetMapInteractionButtonLabel(string mapText)
        {
            _mapTextLabel.text = mapText;
        }

        private void SetMapInteractionButtonSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public void ShowMapInteractionButton(PlayerCharacter player, Vector2 pos, MapInteractionAction mapInteractionAction, string mapText, Sprite sprite = null)
        {
            _buttonWorldBasePosition = pos;
            TriggerPlayer = player;

            if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer &&
                player.PlayerNumber == PlayerNumber.Player2)
            {
                _cameraToUse = CameraManager.Instance.CameraControllers[1].GetCamera();
            }
            else
            {
                _cameraToUse = CameraManager.Instance.CameraControllers[0].GetCamera();
            }

            Vector2 positionAdjustedForScreenPoint = _cameraToUse.WorldToScreenPoint(new Vector2(_buttonWorldBasePosition.x + 0.5f, _buttonWorldBasePosition.y + 1));

            _mapInteractionButton.onClick.AddListener(() =>
            {
                ExecuteMapInteraction(mapInteractionAction);
            });

            SetMapInteractionButtonLabel(mapText);
            SetMapInteractionButtonSprite(sprite);
            transform.position = new Vector2(positionAdjustedForScreenPoint.x, positionAdjustedForScreenPoint.y);
            gameObject.SetActive(true);
        }

        public void DestroyMapInteractionButton()
        {
            Destroy(gameObject);
        }

        public void ExecuteMapInteraction(MapInteractionAction actionToExecute)
        {
            switch (actionToExecute)
            {
                case MapInteractionAction.PerformMazeLevelEntryAction:
                    PerformMazeLevelEntryAction();
                    break;
                case MapInteractionAction.PerformControlFerryAction:
                    PerformControlFerryAction();
                    break;
                default:
                    break;
            }

        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        private void PerformControlFerryAction()
        {
            Logger.Log("PerformControlFerryAction");
        }

        private void PerformMazeLevelEntryAction()
        {
            OverworldPlayerCharacter triggeringPlayerCharacter = TriggerPlayer as OverworldPlayerCharacter;

            if (triggeringPlayerCharacter == null) return;

            string mazeName = triggeringPlayerCharacter.OccupiedMazeLevelEntry.MazeLevelName;
            triggeringPlayerCharacter.PerformMazeLevelEntryAction(mazeName);
        }
    }
}