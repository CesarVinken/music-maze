
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSendsMazeLevelInvitationEvent
{
    public byte Code = EventCode.PlayerSendsMazeLevelInvitationEventCode;

    public void SendPlayerSendsMazeLevelInvitationEvent(string invitor, string levelName)
    {
        object[] content = new object[] {
            invitor,
            levelName
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log(Logger.Event, "PlayerSendsMazeLevelInvitationEvent");
    }
}
