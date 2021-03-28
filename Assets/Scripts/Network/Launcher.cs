using UnityEngine;
using UnityEngine.UI;
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

        string gameVersion = "1";

        [Space(10)]
        [Header("Custom Variables")]

        [Space(5)]
        [SerializeField] private Text _playerStatus = null;
        [SerializeField] private Text _connectionStatus = null;
        [SerializeField] private Text _errorText = null;

        [Space(5)]
        [SerializeField] private GameObject _roomJoinUI = null;
        [SerializeField] private InputField _playerNameField = null;
        [SerializeField] private InputField _roomNameField = null;
        [SerializeField] private GameObject _joinRoomButtonGO = null;

        [Space(5)]
        [SerializeField] private LaunchGameUI _launchGameUI = null;
        [SerializeField] private GameObject _splitScreenButtonGO = null;

        string playerName = "";
        public string RoomName = "";

        public void Awake()
        {
            Guard.CheckIsNull(_playerNameField, "_playerNameField", gameObject);
            Guard.CheckIsNull(_roomNameField, "_roomNameField", gameObject);
            Guard.CheckIsNull(_playerStatus, "_playerStatus", gameObject);
            Guard.CheckIsNull(_playerNameField, "_playerNameField", gameObject);
            Guard.CheckIsNull(_errorText, "_errorText", gameObject);

            Guard.CheckIsNull(_roomJoinUI, "_roomJoinUI", gameObject);
            Guard.CheckIsNull(_joinRoomButtonGO, "_joinRoomButtonGO", gameObject);
            Guard.CheckIsNull(_splitScreenButtonGO, "_splitScreenButtonGO", gameObject);

            PhotonNetwork.AutomaticallySyncScene = true;

            SetErrorText("");

            if (!Application.isMobilePlatform)
            {
                _splitScreenButtonGO.SetActive(true);
            }
        }

        void Start()
        {
            PlayerPrefs.DeleteAll();

            Debug.Log("Connecting to Maze World");

            _roomJoinUI.SetActive(false);
            _launchGameUI.TurnOff();

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
            
            RoomName = name;
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
                if (string.IsNullOrWhiteSpace(RoomName))
                {
                    SetErrorText("Please fill in a game name");
                    Debug.LogWarning("Could not go to game room because no game name was given.");

                    EventSystem.current.SetSelectedGameObject(null);
                    return;
                }

                RoomOptions roomOptions = new RoomOptions();
                TypedLobby typedLobby = new TypedLobby(RoomName, LobbyType.Default);
                PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, typedLobby);
            }
        }

        public override void OnConnected()
        {
            base.OnConnected();
            _connectionStatus.text = "Connected to Music Maze!";
            _connectionStatus.color = Color.green;
            _roomJoinUI.SetActive(true);
            _launchGameUI.TurnOff();

            EventSystem.current.SetSelectedGameObject(_playerNameField.gameObject);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _roomJoinUI.SetActive(false);
            _launchGameUI.TurnOff();

            _connectionStatus.text = "Disconnected. Please check your internet connection and restart.";
            _connectionStatus.color = Color.red;

            Debug.LogError("Disconnected. Please check your Internet connection.");
        }

        public override void OnJoinedRoom()
        {
            _roomJoinUI.SetActive(false);
            _launchGameUI.TurnOn();
            _playerNameField.readOnly = true;
            _roomNameField.readOnly = true;
        }

        public void SetErrorText(string errorText)
        {
            _errorText.text = errorText;
        }

        public void SetPlayerStatusText(string statusText)
        {
            _playerStatus.text = statusText;
        }

        public void LaunchMultiplayerGame()
        {
            SetErrorText("");

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                GameRules.SetGamePlayerType(GamePlayerType.NetworkMultiPlayer);

                GameLaunchAction launcher = new GameLaunchAction();
                launcher.Launch();
            }
            else
            {
                SetErrorText("You need a second player to start your game");
                Debug.LogWarning("Could not launch the game because there is only 1 player in the game room.");
            }
        }

        public void LaunchSinglePlayerGame()
        {
            if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                Logger.Log("Cannot play single player because there are already multiple players in this room.");
                return;
            }

            GameRules.SetGamePlayerType(GamePlayerType.SinglePlayer);

            GameLaunchAction launcher = new GameLaunchAction();
            launcher.Launch();
        }

        public void LaunchSplitScreenGame()
        {
            if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                Logger.Log("Cannot play single player because there are already multiple players in this room.");
                return;
            }

            GameRules.SetGamePlayerType(GamePlayerType.SplitScreenMultiPlayer);

            GameLaunchAction launcher = new GameLaunchAction();
            launcher.Launch();
        }
    }
}

