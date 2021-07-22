using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LoadOverworldEvent
{
    public byte Code = EventCode.LoadOverworldEventCode;

    public void SendLoadOverworldEvent(string overworldName = "default")
    {
        Logger.Log("SendLoadOverworldEvent");
        object[] content = new object[] {
            overworldName
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
