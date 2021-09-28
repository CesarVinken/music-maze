using Character;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerFerryBoardingEvent
{
    public byte Code = EventCode.PlayerFerryBoardingEventCode;

    public void SendPlayerFerryBoardingEvent(string ferryRouteId, PlayerCharacter playerCharacter, bool isBoarding)
    {
        int isControllingAsInt = isBoarding ? 1 : 0;

        object[] content = new object[] {
            ferryRouteId,
            playerCharacter.PlayerNumber,
            isControllingAsInt
        };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log(Logger.Event, $"sent PlayerFerryBoardingEvent. {playerCharacter.Name} is isBoarding? {isBoarding}");
    }
}
