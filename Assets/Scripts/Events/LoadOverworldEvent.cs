using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LoadOverworldEvent : MonoBehaviour
{
    public const byte LoadOverworldEventCode = 3;

    public void SendLoadOverworldEvent(string overworldName = "default")
    {
        Logger.Log("SendLoadOverworldEvent");
        object[] content = new object[] {
            overworldName
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(LoadOverworldEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
