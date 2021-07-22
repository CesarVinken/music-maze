using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomUI : MonoBehaviourPunCallbacks, IOnEventCallback
{
    //public static GameRoomUI Instance;

    [SerializeField] private Launcher _launcher;
    [SerializeField] private Text _player1Name = null;
    [SerializeField] private Text _player2Name = null;
    [SerializeField] private GameObject _launchGameButtonGO = null;

    [SerializeField] private GameTypeSetter _gameTypeDropdown = null;
    [SerializeField] private Text _gameTypeReadonlyLabel = null;

    public void Awake()
    {
        //Instance = this;

        Guard.CheckIsNull(_launcher, "_launcher", gameObject);
        Guard.CheckIsNull(_player1Name, "_player1Name", gameObject);
        Guard.CheckIsNull(_player2Name, "_player2Name", gameObject);

        Guard.CheckIsNull(_launchGameButtonGO, "_gameTypeDropdown", gameObject);
        Guard.CheckIsNull(_gameTypeReadonlyLabel, "_gameTypeReadonlyLabel", gameObject);

        PhotonNetwork.AutomaticallySyncScene = true;

        _launcher.SetErrorText("");
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            Logger.Log("{0} joined the rumble", PhotonNetwork.PlayerList[0].NickName);
            _launchGameButtonGO.SetActive(true);

            _gameTypeReadonlyLabel.gameObject.SetActive(false);
            _gameTypeDropdown.TurnOn();

            _launcher.SetPlayerStatusText("You created a new Game Room");
            _player1Name.text = "<color=green>" + PhotonNetwork.MasterClient.NickName + "</color>";
        }
        else
        {
            _gameTypeReadonlyLabel.gameObject.SetActive(true);
            _gameTypeDropdown.TurnOff();
            _launchGameButtonGO.SetActive(false);

            Logger.Log("{0} joined the rumble", PhotonNetwork.PlayerList[1].NickName);

            _launcher.SetPlayerStatusText("Connected to " + _launcher.RoomName + ". Waiting for " + PhotonNetwork.MasterClient.NickName + " to launch the game..");
            _player1Name.text = PhotonNetwork.MasterClient.NickName;
            _player2Name.text = "<color=green>" + PhotonNetwork.PlayerList[1].NickName + "</color>";
        }
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Logger.Log("{0} joined the rumble", newPlayer.NickName);

        _player2Name.text = newPlayer.NickName;

        if (PhotonNetwork.IsMasterClient)
        {
            Logger.Log("Tell the client about the current game mode");
            UpdateGameModeEvent updateGameModeEvent = new UpdateGameModeEvent();
            updateGameModeEvent.SendUpdateGameModeEvent(GameRules.GameMode);
        }
    }

    public void Launch()
    {
        _launcher.LaunchMultiplayerGame();
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == UpdateGameModeEvent.UpdateGameModeEventCode)
        {
            Logger.Log("received an update game mode event");
            object[] data = (object[])photonEvent.CustomData;

            UpdateGameTypeLabel((string)data[0]);
        }
    }

    public void UpdateGameTypeLabel(string newName)
    {
        newName = Regex.Replace(newName, "([a-z])([A-Z])", "$1 $2"); 
        _gameTypeReadonlyLabel.text = newName;
    }
}
