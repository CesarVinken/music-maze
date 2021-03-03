using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMainScreenOverlayCanvas : MonoBehaviour
{
    public static OverworldMainScreenOverlayCanvas Instance;

    [SerializeField] private GameObject _mazeLevelInvitationPanelPrefab;
    [SerializeField] private GameObject _mazeLevelInvitationRejectionPanelPrefab;

    public void Awake()
    {
        Guard.CheckIsNull(_mazeLevelInvitationPanelPrefab, "MazeLevelInvitationPanelPrefab", gameObject);

        Instance = this;
    }

    public void ShowMazeInvitation(string playerName, string mazeName)
    {
        if (MazeLevelInvitation.Instance == null)
        {
            GameObject mazeInvitationGO = Instantiate(_mazeLevelInvitationPanelPrefab);
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
            GameObject mazeInvitationRejectionGO = Instantiate(_mazeLevelInvitationRejectionPanelPrefab);
            mazeInvitationRejectionGO.SetActive(true);
            MazeLevelInvitationRejection mazeInvitationRejection = mazeInvitationRejectionGO.GetComponent<MazeLevelInvitationRejection>();
            mazeInvitationRejection.Show(rejectorName, mazeName);
        }
        else
        {
            MazeLevelInvitationRejection.Instance.Show(rejectorName, mazeName);
        }
    }
}
