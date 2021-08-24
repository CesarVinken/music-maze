using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MazeLevelInvitationRejection : MonoBehaviour
    {
        public static MazeLevelInvitationRejection Instance;

        [SerializeField] private Text _infoText;
        private string _mazeLevelName = "";

        public void Awake()
        {
            Instance = this;
        }

        public void Show(string playerName, string mazeName, ReasonForRejection reason)
        {
            gameObject.SetActive(true);

            _mazeLevelName = mazeName;
            SetInfoText(playerName, reason);
        }

        private void SetInfoText(string playerName, ReasonForRejection reason)
        {
            if (PlayerMessagePanel.Instance != null)
            {
                PlayerMessagePanel.Instance.CloseMessagePanel();
            }

            switch (reason)
            {
                case ReasonForRejection.LevelNotFound:
                    _infoText.text = $"{playerName} rejected your invitation because they do not have access to the level {_mazeLevelName}.";
                    break;
                case ReasonForRejection.PlayerRejected:
                    _infoText.text = $"{playerName} rejected your invitation to go to {_mazeLevelName}.";
                    break;
                default:
                    Logger.Error($"Unknown reason ${reason}");
                    break;
            }
        }

        public void Close()
        {
            gameObject.SetActive(false);

            MazeLevelInvitation.PendingInvitation = false;
        }
    }
}