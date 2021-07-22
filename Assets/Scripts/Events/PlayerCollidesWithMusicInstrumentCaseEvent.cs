using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerCollidesWithMusicInstrumentCaseEvent
{
    public byte Code = EventCode.PlayerCollidesWithMusicInstrumentCaseEventCode;

    public void SendPlayerCollidesWithMusicInstrumentCaseEvent(GridLocation tileLocation, PlayerCharacter playerCharacter)
    {
        object[] content = new object[] {
            tileLocation.X,
            tileLocation.Y,
            playerCharacter.PlayerNumber
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log("sent PlayerCollidesWithMusicInstrumentCaseEvent");
    }
}
