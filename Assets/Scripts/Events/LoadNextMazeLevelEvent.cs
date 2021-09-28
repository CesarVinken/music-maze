using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class LoadNextMazeLevelEvent
{
    public byte Code = EventCode.LoadNextMazeLevelEventCode;

    public void SendLoadNextMazeLevelEvent(string levelName)
    {
        object[] content = new object[] {
            levelName
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log(Logger.Event, "SendLoadNextMazeLevelEvent");
    }
}