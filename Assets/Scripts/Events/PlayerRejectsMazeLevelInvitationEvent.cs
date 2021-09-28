using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerRejectsMazeLevelInvitationEvent
{
    public byte Code = EventCode.PlayerRejectsMazeLevelInvitationEventCode;

    public void SendPlayerRejectsMazeLevelInvitationEvent(string rejector, string mazeName, ReasonForRejection reason)
    {
        object[] content = new object[] {
            rejector,
            mazeName,
            reason.ToString()
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log(Logger.Event, "PlayerRejectsMazeLevelInvitationEvent");
    }
}
