using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomPlayer : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _lobbyUI = null;
    [SerializeField] private Text[] _playerNameTexts = new Text[2];

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool _isLeader;
    public bool IsLeader
    {
        set
        {
            _isLeader = value;
        }
    }

    private MazeNetworkManager _room;
    public MazeNetworkManager Room
    {
        get
        {
            if (_room != null) { return _room; }
            return _room = NetworkManager.singleton as MazeNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInputPanel.DisplayName);

        _lobbyUI.SetActive(true);

        CmdReadyUp(); //Ready up once started
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnNetworkDestroy()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) {}
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach(NetworkRoomPlayer player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < _playerNameTexts.Length; i++)
        {
            _playerNameTexts[i].text = "Waiting for Player...";
        }

        for (int j = 0; j < Room.RoomPlayers.Count; j++)
        {
            _playerNameTexts[j].text = Room.RoomPlayers[j].DisplayName;
        }
    }

    public bool GetLeaderRoomPlayer()
    {
        if(!_isLeader) { return false; }

        return true;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        //name validation here

        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = true;

        Room.TryStartGame();
    }

    [Command]
    public void CmdStartGame()
    {
        // make sure RoomPlayers[0] is the leader
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartGame();
    }
}
