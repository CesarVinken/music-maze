using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MazeLevelManager : MonoBehaviour, IOnEventCallback
{
    public static MazeLevelManager Instance;
    public MazeLevel Level;

    public void Awake()
    {
        Instance = this;
    }

    //private void OnEnable()
    //{
    //    PhotonNetwork.AddCallbackTarget(this);
    //}

    //private void OnDisable()
    //{
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //}


    public void LoadLevel(MazeName mazeName = MazeName.Blank6x6)
    {
        Level = MazeLevel.Create(mazeName);
    }

    public void UnloadLevel()
    {
        Destroy(TilesContainer.Instance.gameObject);
        TilesContainer.Instance = null;

        Level.Tiles.Clear();
    }

    public void SetTileMarker(Tile tile, PlayerCharacter player)
    {
        player.LastTile = tile;

        if (player.PlayerNumber == PlayerNumber.Player1)
        {
            tile.PlayerMark.sprite = MainCanvas.Instance.Player1TileMarker;
        }
        else
        {
            tile.PlayerMark.sprite = MainCanvas.Instance.Player2TileMarker;
        }
    }

    public void OnEvent(EventData photonEvent)
    {
    //    byte eventCode = photonEvent.Code;
    //    if (eventCode == PlayerMarksTileEvent.PlayerMarksTileEventCode)
    //    {
    //        object[] data = (object[])photonEvent.CustomData;
    //        GridLocation tileLocation = new GridLocation((int)data[0], (int)data[1]);
    //        Logger.Log("Mark tile at {0},{1}", tileLocation.X, tileLocation.Y);

    //        Tile tile = MazeLevelManager.Instance.Level.TilesByLocation[tileLocation]; // add check

    //        //if (player.PlayerNumber == PlayerNumber.Player1)
    //        //{
    //        tile.PlayerMark.sprite = MainCanvas.Instance.Player1TileMarker;
    //        //}
    //        //else
    //        //{
    //        //    tile.PlayerMark.sprite = MainCanvas.Instance.Player2TileMarker;
    //        //}
    //        //for (int index = 1; index < data.Length; ++index)
    //        //{
    //        //    int unitId = (int)data[index];
    //        //    UnitList[unitId].TargetPosition = targetPosition;
    //        //}
    //    }
    }
}

//public class PlayerMarksTileEvent
//{
//    public const byte PlayerMarksTileEventCode = 1;

//    public void SendPlayerMarksTileEvent(GridLocation tileLocation)
//    {
//        object[] content = new object[] { tileLocation.X, tileLocation.Y}; // Array contains the target position and the IDs of the selected units
//        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
//        PhotonNetwork.RaiseEvent(PlayerMarksTileEventCode, content, raiseEventOptions, SendOptions.SendReliable);
//    }
//}
