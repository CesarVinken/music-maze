using Character;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerControlsFerryEvent
{
    public byte Code = EventCode.PlayerControlsFerryEventCode;

    public void SendPlayerControlsFerryEvent(GridLocation tileLocation, PlayerCharacter playerCharacter, bool isControlling)
    {
        int isControllingAsInt = isControlling ? 1 : 0;

        object[] content = new object[] {
            tileLocation.X,
            tileLocation.Y,
            playerCharacter.PlayerNumber,
            isControllingAsInt
        };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log("sent PlayerControlsFerryEvent");
    }
}
