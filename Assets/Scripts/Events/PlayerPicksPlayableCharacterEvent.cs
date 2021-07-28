using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerPicksPlayableCharacterEvent
{
    public byte Code = EventCode.PlayerPicksPlayableCharacterEventCode;

    public void SendPlayerPicksPlayableCharacterEvent(string playerNumber, string characterName)
    {
        object[] content = new object[] {
            playerNumber,
            characterName
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log("SendPlayerPicksPlayableCharacterEvent");
    }
}
