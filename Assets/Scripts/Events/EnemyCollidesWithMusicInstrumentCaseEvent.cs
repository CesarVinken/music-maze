using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class EnemyCollidesWithMusicInstrumentCaseEvent
{
    public byte Code = EventCode.EnemyCollidesWithMusicInstrumentCaseEventCode;

    public void SendEnemyCollidesWithMusicInstrumentCaseEvent(GridLocation tileLocation, EnemyCharacter enemyCharacter)
    {
        object[] content = new object[] {
            tileLocation.X,
            tileLocation.Y,
            enemyCharacter.PhotonView.ViewID
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log("sent EnemyCollidesWithMusicInstrumentCaseEvent");
    }
}
