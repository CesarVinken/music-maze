using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class UpdateGameModeEvent
{
    public const byte UpdateGameModeEventCode = 4;

    public void SendUpdateGameModeEvent(GameMode gameMode)
    {
        Logger.Log("SendUpdateGameModeEvent with game mode " + gameMode.ToString());
        object[] content = new object[] {
            gameMode.ToString()
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(UpdateGameModeEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
