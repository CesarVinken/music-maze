using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.EventSystems;
using System.Collections;

namespace Photon.Pun.Demo.PunBasics
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject _controlPanel;

        [SerializeField]
        private byte maxPlayersPerRoom = 2;

        string gameVersion = "1";
        private readonly string _connectionStatusMessage = "    Connection Status: ";

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
        [SerializeField] private LoginUI _loginUI = null;
        [SerializeField] private WelcomeUI _welcomeUI = null;
        [SerializeField] private GameListUI _gameListUI = null;
        [SerializeField] private GameRoomUI _launchGameUI = null;

        [SerializeField] private GameObject _splitScreenButtonGO = null;

        public string PlayerName { get; private set; }
    public string RoomName { get; private set; }

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
            _loginUI.TurnOn();
        }

        private void Update()
        {
            _connectionStatus.text = _connectionStatusMessage + PhotonNetwork.NetworkClientState;

        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster");
            //this.SetActivePanel(SelectionPanel.name);
            //base.OnConnected();
            //_connectionStatus.text = "Connected to Music Maze!";
            //_connectionStatus.color = Color.green;
            //_roomJoinUI.SetActive(true);

            ShowMainUI();
        }

        public void SetPlayerName(string name)
        {
            string sanatisedName = name.Trim();
            if (string.IsNullOrEmpty(sanatisedName))
            {
                SetErrorText("Please fill in a valid name");
                return;
            }
            SetErrorText("");
            
            PlayerName = name;
        }

        public void SetRoomName(string name)
        {
            SetErrorText("");
            
            RoomName = name;
        }

        public void HostGame()
        {
            SetRoomName($"{PlayerName}'s game");
            //roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            RoomOptions options = new RoomOptions { MaxPlayers = 2, PlayerTtl = 10000 };
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties.Add("Name", RoomName);

            options.CustomRoomProperties = customProperties;
            options.CustomRoomPropertiesForLobby = new string[] { "Name" };

            PhotonNetwork.CreateRoom(RoomName, options, null);

            _welcomeUI.TurnOff();
        }

        //public void JoinGameRoom()
        //{
        //    SetErrorText("");

        //    if (PhotonNetwork.IsConnected)
        //    {
        //        PhotonNetwork.LocalPlayer.NickName = PlayerName;
        //        Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + RoomName);

        //        if (string.IsNullOrWhiteSpace(PlayerName))
        //        {
        //            SetErrorText("Please fill in a name");
        //            Debug.LogWarning("Could not go to game room because no game name was given.");

        //            EventSystem.current.SetSelectedGameObject(null);
        //            return;
        //        }
        //        if (string.IsNullOrWhiteSpace(RoomName))
        //        {
        //            Debug.LogWarning("Could not go to game room because no game name was given.");

        //            EventSystem.current.SetSelectedGameObject(null);
        //            return;
        //        }

        //        _welcomeUI.TurnOff();

        //        RoomOptions roomOptions = new RoomOptions();
        //        TypedLobby typedLobby = new TypedLobby(RoomName, LobbyType.Default);
        //        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, typedLobby);
        //    }
        //}

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

            _launchGameUI.TurnOff();
            _gameListUI.TurnOff();

            _welcomeUI.TurnOn();
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
            Logger.Log("OnJoinedRoom");
            _roomJoinUI.SetActive(false);
            _launchGameUI.TurnOn();
        }

        public void SetErrorText(string errorText)
        {
            StartCoroutine(SetErrorTextCoroutine());
            _errorText.text = errorText;
        }

        private IEnumerator SetErrorTextCoroutine()
        {
            yield return new WaitForSeconds(2f);

            float alphaAmount = 1;
            float fadeSpeed = 1.2f;
            Color color = _errorText.color;

            while (alphaAmount > 0)
            {
                alphaAmount = alphaAmount - (fadeSpeed * Time.deltaTime);

                _errorText.color = new Color(color.r, color.g, color.b, alphaAmount);
                yield return null;
            }
            _errorText.text = "";
            _errorText.color = new Color(color.r, color.g, color.b, 1);
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

        public void ShowMainUI()
        {
            SetPlayerStatusText("");

            _loginUI.TurnOff();
            _launchGameUI.TurnOff();
            _gameListUI.TurnOff();
            _welcomeUI.TurnOn();
        }

        public void ShowGameListUI()
        {
            SetPlayerStatusText("");

            _loginUI.TurnOff();
            _launchGameUI.TurnOff();
            _gameListUI.TurnOn();
            _welcomeUI.TurnOff();
        }
    }
}

