using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject _controlPanel;

        [SerializeField]
        private Text _feedbackText;

        [SerializeField]
        private byte maxPlayersPerRoom = 2;

        bool isConnecting;

        string gameVersion = "1";

        [Space(10)]
        [Header("Custom Variables")]
        [SerializeField] private InputField _playerNameField;
        [SerializeField] private InputField _roomNameField;

        [Space(5)]
        [SerializeField] private Text _playerStatus;
        [SerializeField] private Text _connectionStatus;

        [Space(5)]
        [SerializeField] private GameObject _roomJoinUI;
        [SerializeField] private GameObject _loadArenaButtonGO;
        [SerializeField] private GameObject _joinRoomButtonGO;

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

            Guard.CheckIsNull(_roomJoinUI, "Could not find _roomJoinUI");
            Guard.CheckIsNull(_loadArenaButtonGO, "Could not find _loadArenaButtonGO");
            Guard.CheckIsNull(_joinRoomButtonGO, "Could not find _joinRoomButtonGO");

            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Start()
        {
            PlayerPrefs.DeleteAll();

            Debug.Log("Connecting to Photon Network");

            _roomJoinUI.SetActive(false);
            _loadArenaButtonGO.SetActive(false);

            ConnectToPhoton();
        }

        public void SetPlayerName(string name)
        {
            playerName = name;
        }

        public void SetRoomName(string name)
        {
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
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + _roomNameField.text);
                RoomOptions roomOptions = new RoomOptions();
                TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default);
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
            }
        }

        public void LoadArena()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                PhotonNetwork.LoadLevel("SampleScene");
            }
            else
            {
                _playerStatus.text = "Minimum 2 Players required to Load Arena!";
            }
        }

        public override void OnConnected()
        {
            base.OnConnected();
            _connectionStatus.text = "Connected to Photon!";
            _connectionStatus.color = Color.green;
            _roomJoinUI.SetActive(true);
            _loadArenaButtonGO.SetActive(false);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            isConnecting = false;
            _controlPanel.SetActive(true);
            Debug.LogError("Disconnected. Please check your Internet connection.");
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _loadArenaButtonGO.SetActive(true);
                _joinRoomButtonGO.SetActive(false);
                _playerStatus.text = "Your are Lobby Leader";
            }
            else
            {
                _playerStatus.text = "Connected to Lobby";
            }
        }
    }
}
