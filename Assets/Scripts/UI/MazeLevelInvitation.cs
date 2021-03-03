using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MazeLevelInvitation : MonoBehaviour
{
    public static MazeLevelInvitation Instance;

    [SerializeField] private Text _infoText;
    private string _mazeLevelName = "";

    public static bool PendingInvitation = false;

    public void Awake()
    {
        Instance = this;
    }

    public void Show(string playerName, string mazeName)
    {
        gameObject.SetActive(true);

        _mazeLevelName = mazeName;
        SetInfoText(playerName);
    }

    private void SetInfoText(string playerName)
    {
        _infoText.text = $"{playerName} wants to enter the maze {_mazeLevelName}. Do you accept this invitation?";
    }

    public void Accept()
    {
        gameObject.SetActive(false);

        LoadNextMazeLevelEvent loadNextMazeLevelEvent = new LoadNextMazeLevelEvent();
        loadNextMazeLevelEvent.SendLoadNextMazeLevelEvent(_mazeLevelName);
    }

    public void Reject()
    {
        PlayerNumber ourPlayerCharacterNumber = GameManager.Instance.CharacterManager.GetOurPlayerCharacter();
        PlayerCharacter ourPlayerCharacter = GameManager.Instance.CharacterManager.GetPlayerCharacter<PlayerCharacter>(ourPlayerCharacterNumber);

        PlayerRejectsMazeLevelInvitationEvent playerRejectsMazeLevelInvitationEvent = new PlayerRejectsMazeLevelInvitationEvent();
        playerRejectsMazeLevelInvitationEvent.SendPlayerRejectsMazeLevelInvitationEvent(ourPlayerCharacter.PhotonView.name, _mazeLevelName);

        PendingInvitation = false;

        gameObject.SetActive(false);
    }
}
