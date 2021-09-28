using Character;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerControlsFerryEvent
{
    public byte Code = EventCode.PlayerControlsFerryEventCode;

    public void SendPlayerControlsFerryEvent(string ferryRouteId, PlayerCharacter playerCharacter, bool isControlling)
    {
        int isControllingAsInt = isControlling ? 1 : 0;

        object[] content = new object[] {
            ferryRouteId,
            playerCharacter.CurrentGridLocation.X,
            playerCharacter.CurrentGridLocation.Y,
            playerCharacter.PlayerNumber,
            isControllingAsInt
        };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log(Logger.Event, $"sent PlayerControlsFerryEvent. Player is located at {playerCharacter.CurrentGridLocation.X} {playerCharacter.CurrentGridLocation.Y}");
    }
}
