using Character;
using UnityEngine;

namespace UI
{
    public class OverworldMainScreenOverlayCanvas : MonoBehaviour
    {
        public static OverworldMainScreenOverlayCanvas Instance;

        [SerializeField] private GameObject _mazeLevelInvitationPanelPrefab;
        [SerializeField] private GameObject _mazeLevelInvitationRejectionPanelPrefab;
        [SerializeField] private GameObject _playerMessage1Prefab;
        [SerializeField] private GameObject _playerMessage2Prefab;

        public void Awake()
        {
            Guard.CheckIsNull(_mazeLevelInvitationPanelPrefab, "MazeLevelInvitationPanelPrefab", gameObject);
            Guard.CheckIsNull(_mazeLevelInvitationRejectionPanelPrefab, "_mazeLevelInvitationRejectionPanelPrefab", gameObject);
            Guard.CheckIsNull(_playerMessage1Prefab, "_playerMessage1Prefab", gameObject);
            Guard.CheckIsNull(_playerMessage2Prefab, "_playerMessage2Prefab", gameObject);

            Instance = this;
        }

        public void ShowMazeInvitation(string playerName, string mazeName)
        {
            if (MazeLevelInvitation.Instance == null)
            {
                GameObject mazeInvitationGO = Instantiate(_mazeLevelInvitationPanelPrefab, transform);
                mazeInvitationGO.SetActive(true);
                MazeLevelInvitation mazeInvitation = mazeInvitationGO.GetComponent<MazeLevelInvitation>();
                mazeInvitation.Show(playerName, mazeName);
            }
            else
            {
                MazeLevelInvitation.Instance.Show(playerName, mazeName);
            }
        }

        public void ShowMazeInvitationRejection(string rejectorName, string mazeName, ReasonForRejection reason)
        {
            if (MazeLevelInvitationRejection.Instance == null)
            {
                GameObject mazeInvitationRejectionGO = Instantiate(_mazeLevelInvitationRejectionPanelPrefab, transform);
                mazeInvitationRejectionGO.SetActive(true);
                MazeLevelInvitationRejection mazeInvitationRejection = mazeInvitationRejectionGO.GetComponent<MazeLevelInvitationRejection>();
                mazeInvitationRejection.Show(rejectorName, mazeName, reason);
            }
            else
            {
                MazeLevelInvitationRejection.Instance.Show(rejectorName, mazeName, reason);
            }
        }

        public void ShowPlayerMessagePanel(string message, PlayerNumber playerNumber = PlayerNumber.Player1)
        {
            GameObject playerMessagePanelGO = Instantiate(_playerMessage1Prefab, transform);
            PlayerMessagePanel playerMessagePanel = playerMessagePanelGO.GetComponent<PlayerMessagePanel>();
            playerMessagePanel.ShowMessage(message, playerNumber);
        }

        public void ClosePlayerMessagePanel()
        {
            PlayerMessagePanel.Instance.CloseMessagePanel();
        }

        public void ShowPlayerWarning(string message)
        {
            GameObject playerMessagePanelGO = Instantiate(_playerMessage2Prefab, transform);
            PlayerMessagePanel playerMessagePanel = playerMessagePanelGO.GetComponent<PlayerMessagePanel>();
            playerMessagePanel.ShowMessage(message, PlayerNumber.Player1);
        }
    }
}