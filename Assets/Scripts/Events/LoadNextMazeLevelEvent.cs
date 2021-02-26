using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class LoadNextMazeLevelEvent
{
    public const byte LoadNextMazeLevelEventCode = 2;

    public void SendLoadNextMazeLevelEvent(string levelName)
    {
        Logger.Log("SendLoadNextMazeLevelEvent");
        object[] content = new object[] {
            levelName
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(LoadNextMazeLevelEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}