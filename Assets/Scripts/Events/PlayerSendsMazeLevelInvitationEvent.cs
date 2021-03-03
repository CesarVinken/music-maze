
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSendsMazeLevelInvitationEvent
{
    public const byte PlayerSendsMazeLevelInvitationEventCode = 5;

    public void SendPlayerSendsMazeLevelInvitationEvent(string invitor, string levelName)
    {
        object[] content = new object[] {
            invitor,
            levelName
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(PlayerSendsMazeLevelInvitationEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log("PlayerSendsMazeLevelInvitationEvent");
    }
}
