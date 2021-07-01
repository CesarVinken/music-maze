using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerCollidesWithMusicInstrumentCaseEvent
{
    public const byte PlayerCollidesWithMusicInstrumentCaseEventCode = 7;

    public void SendPlayerCollidesWithMusicInstrumentCaseEvent(GridLocation tileLocation, PlayerCharacter playerCharacter)
    {
        object[] content = new object[] {
            tileLocation.X,
            tileLocation.Y,
            playerCharacter.PlayerNumber
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(PlayerCollidesWithMusicInstrumentCaseEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log("sent PlayerCollidesWithMusicInstrumentCaseEvent");
    }
}
