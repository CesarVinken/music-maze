using Character;
using UnityEngine;

public class OverworldMainScreenOverlayCanvas : MonoBehaviour
{
    public static OverworldMainScreenOverlayCanvas Instance;

    [SerializeField] private GameObject _mazeLevelInvitationPanelPrefab;
    [SerializeField] private GameObject _mazeLevelInvitationRejectionPanelPrefab;
    [SerializeField] private GameObject _playerMessagePrefab;

    public void Awake()
    {
        Guard.CheckIsNull(_mazeLevelInvitationPanelPrefab, "MazeLevelInvitationPanelPrefab", gameObject);
        Guard.CheckIsNull(_mazeLevelInvitationRejectionPanelPrefab, "_mazeLevelInvitationRejectionPanelPrefab", gameObject);
        Guard.CheckIsNull(_playerMessagePrefab, "_playerMessagePrefab", gameObject);

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

    public void ShowMazeInvitationRejection(string rejectorName, string mazeName)
    {
        if (MazeLevelInvitationRejection.Instance == null)
        {
            GameObject mazeInvitationRejectionGO = Instantiate(_mazeLevelInvitationRejectionPanelPrefab, transform);
            mazeInvitationRejectionGO.SetActive(true);
            MazeLevelInvitationRejection mazeInvitationRejection = mazeInvitationRejectionGO.GetComponent<MazeLevelInvitationRejection>();
            mazeInvitationRejection.Show(rejectorName, mazeName);
        }
        else
        {
            MazeLevelInvitationRejection.Instance.Show(rejectorName, mazeName);
        }
    }

    public void ShowPlayerMessagePanel(string message, PlayerNumber playerNumber = PlayerNumber.Player1)
    {
        GameObject playerMessagePanelGO = Instantiate(_playerMessagePrefab, transform);
        PlayerMessagePanel playerMessagePanel = playerMessagePanelGO.GetComponent<PlayerMessagePanel>();
        playerMessagePanel.ShowMessage(message, playerNumber);
    }

    public void ClosePlayerMessagePanel()
    {
        PlayerMessagePanel.Instance.CloseMessagePanel();
    }
}
