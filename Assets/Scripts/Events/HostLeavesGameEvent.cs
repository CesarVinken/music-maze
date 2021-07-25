using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class HostLeavesGameEvent 
{
    public byte Code = EventCode.HostLeavesGameEventCode;

    public void SendHostLeavesGameEvent()
    {
        object[] content = new object[] {};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log("sent HostLeavesGameEvent");
    }
}
