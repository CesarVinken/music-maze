using UI;
using UnityEngine;

namespace UI
{
    public class PlayerMessagePanel : MonoBehaviour
    {
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