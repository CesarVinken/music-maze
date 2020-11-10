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
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(PlayerMarksTileEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}


public class LoadNextMazeLevelEvent
{
    public const byte LoadNextMazeLevelEventCode = 2;

    public void SendLoadNextMazeLevelEvent(string levelName)
    {
        object[] content = new object[] {
            levelName
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(LoadNextMazeLevelEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
