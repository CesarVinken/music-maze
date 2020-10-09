using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;

namespace Photon.Pun.Demo.PunBasics
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject _controlPanel;

        [SerializeField]
        private byte maxPlayersPerRoom = 2;

        bool isConnecting;

        string gameVersion = "1";

        [Space(10)]
        [Header("Custom Variables")]

        [Space(5)]
        [SerializeField] private Text _playerStatus;
        [SerializeField] private Text _connectionStatus;
        [SerializeField] private Text _errorText;

        [Space(5)]
        [SerializeField] private GameObject _roomJoinUI;
        [SerializeField] private InputField _playerNameField;
        [SerializeField] private InputField _roomNameField;
        [SerializeField] private GameObject _joinRoomButtonGO;

        [Space(5)]
        [SerializeField] private GameObject _launchGameUI;
        [SerializeField] private Text _player1Name;
        [SerializeField] private Text _player2Name;
        [SerializeField] private GameObject _launchGameButtonGO;

        string playerName = "";
        string roomName = "";

        public void Awake()
        {
            if (_playerNameField == null)
                Logger.Error(Logger.Initialisation, "Could not find _playerNameField component on Launcher");
            if (_roomNameField == null)
                Logger.Error(Logger.Initialisation, "Could not find _roomNameField component on Launcher");
            if (_playerStatus == null)
                Logger.Error(Logger.Initialisation, "Could not find _playerStatus component on Launcher");
            if (_playerNameField == null)
                Logger.Error(Logger.Initialisation, "Could not find _connectionStatus component on Launcher");
            if (_errorText == null)
                Logger.Error(Logger.Initialisation, "Could not find _errorText component on Launcher");

            Guard.CheckIsNull(_roomJoinUI, "Could not find _roomJoinUI");
            Guard.CheckIsNull(_launchGameButtonGO, "Could not find _loadArenaButtonGO");
            Guard.CheckIsNull(_joinRoomButtonGO, "Could not find _joinRoomButtonGO");

            PhotonNetwork.AutomaticallySyncScene = true;

            SetErrorText("");
        }

        void Start()
        {
            PlayerPrefs.DeleteAll();

            Debug.Log("Connecting to Maze World");

            _roomJoinUI.SetActive(false);
            _launchGameButtonGO.SetActive(false);

            ConnectToPhoton();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                Logger.Log(EventSystem.current.currentSelectedGameObject.name);
                if (EventSystem.current.currentSelectedGameObject.Equals(_playerNameField.gameObject))
                {
                    EventSystem.current.SetSelectedGameObject(_roomNameField.gameObject);
                }
                else if(EventSystem.current.currentSelectedGameObject.Equals(_roomNameField.gameObject))
                {
                    EventSystem.current.SetSelectedGameObject(_joinRoomButtonGO);
                }
            }
        }

        public void SetPlayerName(string name)
        {
            SetErrorText("");
            
            playerName = name;
        }

        public void SetRoomName(string name)
        {
            SetErrorText("");
            
            roomName = name;
        }

        void ConnectToPhoton()
        {
            _connectionStatus.text = "Connecting...";
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        public void JoinRoom()
        {
            SetErrorText("");

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + _roomNameField.text);

                if (string.IsNullOrWhiteSpace(playerName))
                {
                    SetErrorText("Please fill in a name");
                    Debug.LogWarning("Could not go to game room because no game name was given.");

                    EventSystem.current.SetSelectedGameObject(null);
                    return;
                }
                if (string.IsNullOrWhiteSpace(roomName))
                {
                    SetErrorText("Please fill in a game name");
                    Debug.LogWarning("Could not go to game room because no game name was given.");

                    EventSystem.current.SetSelectedGameObject(null);
                    return;
                }

                RoomOptions roomOptions = new RoomOptions();
                TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default);
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
            }
        }

        public void LoadArena()
        {
            SetErrorText("");
            
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                PhotonNetwork.LoadLevel("SampleScene");
            }
            else
            {
                SetErrorText("You need a second player to start your game");
                Debug.LogWarning("Could not launch the game because there is only 1 player in the game room.");
            }
        }

        public override void OnConnected()
        {
            base.OnConnected();
            _connectionStatus.text = "Connected to Music Maze!";
            _connectionStatus.color = Color.green;
            _roomJoinUI.SetActive(true);
            _launchGameUI.SetActive(false);

            EventSystem.current.SetSelectedGameObject(_playerNameField.gameObject);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            isConnecting = false;
            _roomJoinUI.SetActive(false);
            _launchGameUI.SetActive(false);

            _connectionStatus.text = "Disconnected. Please check your internet connection and restart.";
            _connectionStatus.color = Color.red;

            Debug.LogError("Disconnected. Please check your Internet connection.");
        }

        public override void OnJoinedRoom()
        {
            _roomJoinUI.SetActive(false);
            _launchGameUI.SetActive(true);

            if (PhotonNetwork.IsMasterClient)
            {
                Logger.Log("{0} joined the rumble", PhotonNetwork.PlayerList[0].NickName);
                _launchGameButtonGO.SetActive(true);

                _playerStatus.text = "You created a new Game Room";
                _player1Name.text = "<color=green>" + PhotonNetwork.MasterClient.NickName + "</color>";
            }
            else
            {
                Logger.Log("{0} joined the rumble", PhotonNetwork.PlayerList[1].NickName);

                _playerStatus.text = "Connected to " + roomName + ". Waiting for " + PhotonNetwork.MasterClient.NickName + " to launch the game..";
                _player1Name.text = PhotonNetwork.MasterClient.NickName;
                _player2Name.text = "<color=green>" + PhotonNetwork.PlayerList[1].NickName + "</color>";
            }

            _playerNameField.readOnly = true;
            _roomNameField.readOnly = true;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Logger.Log("{0} joined the rumble", newPlayer.NickName);

            _player2Name.text = newPlayer.NickName;
        }

        private void SetErrorText(string errorText)
        {
            _errorText.text = errorText;
        }
    }
}
