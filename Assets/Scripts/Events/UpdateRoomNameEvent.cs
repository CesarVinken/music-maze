using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class UpdateRoomNameEvent
{
    public byte Code = EventCode.UpdateRoomNameEventCode;

    public void SendUpdateGameNameEvent(string roomName)
    {
        object[] content = new object[] {
            roomName
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
