using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerRejectsMazeLevelInvitationEvent
{
    public const byte PlayerRejectsMazeLevelInvitationEventCode = 6;

    public void SendPlayerRejectsMazeLevelInvitationEvent(string rejector, string mazeName)
    {
        object[] content = new object[] {
            rejector,
            mazeName
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(PlayerRejectsMazeLevelInvitationEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log("PlayerRejectsMazeLevelInvitationEvent");
    }
}
