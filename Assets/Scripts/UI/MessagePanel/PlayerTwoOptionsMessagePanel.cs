using Character;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerTwoOptionsMessagePanel : PlayerMessagePanel
    {
        [SerializeField] private Button _optionAButton;
        [SerializeField] private Button _optionBButton;

        [SerializeField] private Text _optionAButtonText;
        [SerializeField] private Text _optionBButtonText;
        [SerializeField] private Text _messageText;

        public void Initialise(string message, string optionAButtonText, GameUIAction optionAUIAction, string optionBButtonText, GameUIAction optionBUIAction, PlayerNumber playerNumber)
        {
            if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
            {
                RectTransform rt = gameObject.GetComponent<RectTransform>();

                float fullCanvasWidth = 1280;
                rt.sizeDelta = new Vector2(fullCanvasWidth / 2, rt.sizeDelta.y);

                if (playerNumber == PlayerNumber.Player1)
                {
                    rt.localPosition = new Vector3(-(fullCanvasWidth / 4), rt.localPosition.y, rt.localPosition.z);
                }
                else
                {
                    rt.localPosition = new Vector3(fullCanvasWidth - fullCanvasWidth / 4 * 3, rt.localPosition.y, rt.localPosition.z);
                }
            }

            _optionAButton.onClick.AddListener(() =>
            {
                ExecuteAction(optionAUIAction);
            });
            _optionAButtonText.text = optionAButtonText;

            _optionBButton.onClick.AddListener(() =>
            {
                ExecuteAction(optionBUIAction);
            });
            _optionBButtonText.text = optionBButtonText;

            _messageText.text = message;
            gameObject.SetActive(true);

            MainScreenOverlayCanvas.Instance.OpenMessagePanels.Add(this);
        }
    }
}