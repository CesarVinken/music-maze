using Character;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class EnemyCollidesWithSheetmusicEvent
{
    public byte Code = EventCode.EnemyCollidesWithSheetmusicEventCode;

    public void SendEnemyCollidesWithSheetmusicEvent(GridLocation tileLocation, EnemyCharacter enemyCharacter)
    {
        object[] content = new object[] {
            tileLocation.X,
            tileLocation.Y,
            enemyCharacter.PhotonView.ViewID
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        Logger.Log("sent EnemyCollidesWithSheetmusicEvent");
    }
}
