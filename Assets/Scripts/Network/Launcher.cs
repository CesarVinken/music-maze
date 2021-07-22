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

        [Space(5)]
        [SerializeField] private GameObject _roomJoinUI = null;

        [Space(5)]
        [SerializeField] private WelcomeUI _welcomeUI = null;
        [SerializeField] private GameListUI _gameListUI = null;
        [SerializeField] private GameRoomUI _launchGameUI = null;

        [SerializeField] private GameObject _splitScreenButtonGO = null;

        public string PlayerName = "";
        public string RoomName = "";
        public void Awake()
        {
            Guard.CheckIsNull(_playerStatus, "_playerStatus", gameObject);
            Guard.CheckIsNull(_errorText, "_errorText", gameObject);

            Guard.CheckIsNull(_welcomeUI, "_welcomeUI", gameObject);

            Guard.CheckIsNull(_gameListUI, "_gameListUI", gameObject);
            Guard.CheckIsNull(_launchGameUI, "_launchGameUI", gameObject);

            Guard.CheckIsNull(_roomJoinUI, "_roomJoinUI", gameObject);
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

        //private void Update()
        //{
        //    if(Input.GetKeyDown(KeyCode.Tab))
        //    {
        //        Logger.Log(EventSystem.current.currentSelectedGameObject.name);
        //        if (EventSystem.current.currentSelectedGameObject.Equals(_playerNameField.gameObject))
        //        {
        //            EventSystem.current.SetSelectedGameObject(_roomNameField.gameObject);
        //        }
        //        else if(EventSystem.current.currentSelectedGameObject.Equals(_roomNameField.gameObject))
        //        {
        //            EventSystem.current.SetSelectedGameObject(_joinRoomButtonGO);
        //        }
        //    }
        //}

        public void SetPlayerName(string name)
        {
            SetErrorText("");
            
            PlayerName = name;
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


        public void JoinGameRoom()
        {
            SetErrorText("");
            RoomName = "Temp name"; // TODO: clicking Host game should trigger input field for game name

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = PlayerName;
                Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + RoomName);

                if (string.IsNullOrWhiteSpace(PlayerName))
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

                _welcomeUI.TurnOff();

                RoomOptions roomOptions = new RoomOptions();
                TypedLobby typedLobby = new TypedLobby(RoomName, LobbyType.Default);
                PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, typedLobby);
            }
        }

        public void OpenGameList()
        {
            _welcomeUI.TurnOff();
            _gameListUI.TurnOn();
        }

        public override void OnConnected()
        {
            base.OnConnected();
            _connectionStatus.text = "Connected to Music Maze!";
            _connectionStatus.color = Color.green;
            //_roomJoinUI.SetActive(true);

            _launchGameUI.TurnOff();
            _gameListUI.TurnOff();

            _welcomeUI.TurnOn();
            //EventSystem.current.SetSelectedGameObject(_playerNameField.gameObject);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _roomJoinUI.SetActive(false);
            _launchGameUI.TurnOff();
            _gameListUI.TurnOff();

            _connectionStatus.text = "Disconnected. Please check your internet connection and restart.";
            _connectionStatus.color = Color.red;

            Debug.LogError("Disconnected. Please check your Internet connection.");
        }

        public override void OnJoinedRoom()
        {
            _roomJoinUI.SetActive(false);
            _launchGameUI.TurnOn();
            //_playerNameField.readOnly = true;
            //_roomNameField.readOnly = true;
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
                GameRules.SetGamePlayerType(GamePlayerType.NetworkMultiplayer);

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

            GameRules.SetGamePlayerType(GamePlayerType.SplitScreenMultiplayer);

            GameLaunchAction launcher = new GameLaunchAction();
            launcher.Launch();
        }
    }
}

