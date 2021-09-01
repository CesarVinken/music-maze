using Character;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerOneOptionMessagePanel : MonoBehaviour, IMessagePanel
    {
        [SerializeField] private Button _optionAButton;
        [SerializeField] private Text _optionAButtonText;

        public static Dictionary<MessagePosition, Vector2> MessageSpawnPosition = new Dictionary<MessagePosition, Vector2>()
        {
            { MessagePosition.Middle, new Vector2(0, 0) },
            { MessagePosition.PlayerLeft, new Vector2(0, 0) },
            { MessagePosition.PlayerRight, new Vector2(0, 0) }
        };

        [SerializeField] private Text _messageText;

        public void Initialise(string message, string buttonText, GameUIAction gameUIAction, PlayerNumber playerNumber)
        {
            //Instance = this;


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
                ExecuteAction(gameUIAction);
            });
            _optionAButtonText.text = buttonText;

            _messageText.text = message;
            gameObject.SetActive(true);

            MainScreenOverlayCanvas.Instance.OpenMessagePanels.Add(this);
        }

        public void ExecuteAction(GameUIAction actionToExecute)
        {
            MainScreenOverlayCanvas.Instance.OpenMessagePanels.Remove(this);
            switch (actionToExecute)
            {
                case GameUIAction.ExitGame:
                    Application.Quit();
                    break;
                case GameUIAction.ToMainMenu:
                    Application.Quit();// TODO go to main
                    break;
                case GameUIAction.ClosePanel:
                    ExecuteClosePanel();
                    break;
                default:
                    break;
            }
        }

        public void ExecuteClosePanel()
        {
            Destroy(gameObject);
        }
    }
}