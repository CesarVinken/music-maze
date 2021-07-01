using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class EnemyCollidesWithMusicInstrumentCaseEvent
{
    public const byte EnemyCollidesWithMusicInstrumentCaseEventCode = 8;

    public void SendEnemyCollidesWithMusicInstrumentCaseEvent(GridLocation tileLocation, EnemyCharacter enemyCharacter)
    {
        object[] content = new object[] {
            tileLocation.X,
            tileLocation.Y,
            enemyCharacter.PhotonView.ViewID
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EnemyCollidesWithMusicInstrumentCaseEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log("sent EnemyCollidesWithMusicInstrumentCaseEvent");
    }
}
