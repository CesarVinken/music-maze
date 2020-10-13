using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMarksTileEvent
{
    public const byte PlayerMarksTileEventCode = 1;

    public void SendPlayerMarksTileEvent(GridLocation tileLocation, PlayerCharacter playerCharacter)
    {
        object[] content = new object[] {
            tileLocation.X,
            tileLocation.Y,
            playerCharacter.PlayerNumber
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(PlayerMarksTileEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
