using UnityEngine;
using UnityEngine.UI;

public class MazeLevelInvitationRejection : MonoBehaviour
{
    public static MazeLevelInvitationRejection Instance;

    [SerializeField] private Text _infoText;
    private string _mazeLevelName = "";

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
        if(PlayerMessagePanel.Instance != null)
        {
            PlayerMessagePanel.Instance.CloseMessagePanel();
        }

        _infoText.text = $"{playerName} rejected your invitation to go to {_mazeLevelName}.";
    }

    public void Close()
    {
        gameObject.SetActive(false);

        MazeLevelInvitation.PendingInvitation = false;
    }
}
