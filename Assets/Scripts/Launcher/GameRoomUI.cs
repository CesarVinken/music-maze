using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomUI : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private Launcher _launcher;
    [SerializeField] private RoomPlayerEntry _player1Entry = null;
    [SerializeField] private RoomPlayerEntry _player2Entry = null;
    [SerializeField] private GameObject _launchGameButtonGO = null;

    [SerializeField] private InputField _roomNameInputField = null;
    [SerializeField] private Button _updateRoomNameButton = null;

    [SerializeField] private GameTypeSetter _gameTypeDropdown = null;
    [SerializeField] private Text _gameTypeReadonlyLabel = null;

    [SerializeField] private GameObject _playerMessagePrefab = null;
    [SerializeField] private GameObject _UIControlsGO = null;

    public void Awake()
    {
        Guard.CheckIsNull(_launcher, "_launcher", gameObject);

        Guard.CheckIsNull(_player1Entry, "_player1Entry", gameObject);
        Guard.CheckIsNull(_player2Entry, "_player2Entry", gameObject);

        Guard.CheckIsNull(_roomNameInputField, "_roomNameInputField", gameObject);
        Guard.CheckIsNull(_updateRoomNameButton, "_updateRoomNameButton", gameObject);

        Guard.CheckIsNull(_launchGameButtonGO, "_gameTypeDropdown", gameObject);
        Guard.CheckIsNull(_gameTypeReadonlyLabel, "_gameTypeReadonlyLabel", gameObject);

        Guard.CheckIsNull(_playerMessagePrefab, "_playerMessagePrefab", gameObject);
        Guard.CheckIsNull(_UIControlsGO, "_welcomeUIGO", gameObject);

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

            _updateRoomNameButton.gameObject.SetActive(false);
            _roomNameInputField.text = PhotonNetwork.CurrentRoom.CustomProperties["Name"].ToString();
            _roomNameInputField.interactable = true;

            _gameTypeReadonlyLabel.gameObject.SetActive(false);
            _gameTypeDropdown.TurnOn();

            _launcher.SetPlayerStatusText("You created a new Game Room");
            _player1Entry.SetPlayerName("<color=green>" + PhotonNetwork.MasterClient.NickName + "</color>");
            _player1Entry.PickCharacter("Emmon");
        }
        else
        {
            _gameTypeReadonlyLabel.gameObject.SetActive(true);
            _gameTypeDropdown.TurnOff();
            _launchGameButtonGO.SetActive(false);

            _updateRoomNameButton.gameObject.SetActive(false);
            _roomNameInputField.text = PhotonNetwork.CurrentRoom.Id;
            _roomNameInputField.interactable = false;

            Logger.Log("{0} joined the rumble", PhotonNetwork.PlayerList[1].NickName);

            _launcher.SetPlayerStatusText("Connected to " + _launcher.RoomName + ". Waiting for " + PhotonNetwork.MasterClient.NickName + " to launch the game..");
            _player1Entry.SetPlayerName(PhotonNetwork.MasterClient.NickName);
            _player2Entry.SetPlayerName("<color=green>" + PhotonNetwork.PlayerList[1].NickName + "</color>");

            //  TÖDO: get selected palyer character from host
            // TODO: send selected player character to host
        }
    }

    public void OnUpdateRoomNameInputFieldValue()
    {
        if(_roomNameInputField.text != _launcher.RoomName)
        {
            _updateRoomNameButton.gameObject.SetActive(true);
        }
        else
        {
            _updateRoomNameButton.gameObject.SetActive(false);
        }
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Logger.Log("{0} joined the rumble", newPlayer.NickName);

        _player2Entry.SetPlayerName(newPlayer.NickName);
        
        if (PhotonNetwork.IsMasterClient)
        {
            Logger.Log("Tell the client about the current game mode");
            UpdateGameModeEvent updateGameModeEvent = new UpdateGameModeEvent();
            updateGameModeEvent.SendUpdateGameModeEvent(GameRules.GameMode);

            //UpdateRoomNameEvent updateRoomNameEvent = new UpdateRoomNameEvent();
            //updateRoomNameEvent.SendUpdateGameNameEvent(_launcher.RoomName);
        }
    }

    public void Launch()
    {
        _launcher.LaunchMultiplayerGame();
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCode.UpdateGameModeEventCode)
        {
            Logger.Log("received an update game mode event");
            object[] data = (object[])photonEvent.CustomData;

            UpdateGameTypeLabel((string)data[0]);
        }
        else if (eventCode == EventCode.HostLeavesGameEventCode)
        {
            Logger.Log("received a HostLeavesGameEventCode event");
            // the host left, let the client leave as well.
            BackToMain();

            // Show message to inform the client what happened
            string message = "The host has left the game. Return to menu.";
            GameObject gameAbandonedMessageGO = GameObject.Instantiate(_playerMessagePrefab, _UIControlsGO.transform);
            PlayerMessagePanel playerMessagePanel = gameAbandonedMessageGO.GetComponent<PlayerMessagePanel>();
            playerMessagePanel.ShowMessage(message, PlayerNumber.Player1);
        }
        else if (eventCode == EventCode.UpdateRoomNameEventCode)
        {
            Logger.Log("received an update room name event");
            object[] data = (object[])photonEvent.CustomData;

            string newRoomName = (string)data[0];
            _launcher.SetRoomName(newRoomName);
            _roomNameInputField.text = newRoomName;
        }
    }

    public void UpdateGameTypeLabel(string newName)
    {
        newName = Regex.Replace(newName, "([a-z])([A-Z])", "$1 $2"); 
        _gameTypeReadonlyLabel.text = newName;
    }

    public void BackToMain()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                HostLeavesGameEvent hostLeavesGameEvent = new HostLeavesGameEvent();
                hostLeavesGameEvent.SendHostLeavesGameEvent();
            }

            // This takes a while to update
            PhotonNetwork.LeaveRoom(true);
        }
        _launcher.ShowMainUI();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Logger.Warning("Failed to create room");
        _launcher.ShowMainUI();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Logger.Warning("Failed to join room");
        _launcher.ShowGameListUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // the client left, reopen the second spot for the game
            _launcher.SetPlayerStatusText("Connected to " + _launcher.RoomName + ". Waiting for a second player...");
            _player2Entry.SetPlayerName("");   
        }
    }

    // called when the host picks the update room name button
    public void UpdateRoomName()
    {
        string newRoomName = _roomNameInputField.text;

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "Name", newRoomName } });

        _launcher.SetRoomName(newRoomName);

        // update client about new room name
        UpdateRoomNameEvent updateRoomNameEvent = new UpdateRoomNameEvent();
        updateRoomNameEvent.SendUpdateGameNameEvent(newRoomName);

        _updateRoomNameButton.gameObject.SetActive(false);
    }
}
