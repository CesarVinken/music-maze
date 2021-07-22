using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMarksTileEvent
{
    public byte Code = EventCode.PlayerMarksTileEventCode;

    public void SendPlayerMarksTileEvent(GridLocation tileLocation, PlayerCharacter playerCharacter)
    {
        object[] content = new object[] {
            tileLocation.X,
            tileLocation.Y,
            playerCharacter.PlayerNumber
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
