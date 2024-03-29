using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameListUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Launcher _launcher;

    [SerializeField] private GameObject _roomListPanel;

    [SerializeField] private GameObject _roomListContent;
    [SerializeField] private GameObject _roomListEntryPrefab;


    private Dictionary<string, RoomInfo> _cachedRoomList;
    private Dictionary<string, GameObject> _roomListEntries;
    private Dictionary<int, GameObject> _playerListEntries;

    public void Awake()
    {
        Guard.CheckIsNull(_launcher, "_launcher", gameObject);

        PhotonNetwork.AutomaticallySyncScene = true;

        _launcher.SetErrorText("");

        _cachedRoomList = new Dictionary<string, RoomInfo>();
        _roomListEntries = new Dictionary<string, GameObject>();
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);

        if (!PhotonNetwork.InLobby)
        {
            Logger.Log("join the lobby");
            PhotonNetwork.JoinLobby();
        }
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void JoinRoom()
    {
        _launcher.LaunchMultiplayerGame();
    }

    private void ClearRoomListView()
    {
        foreach (GameObject entry in _roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        _roomListEntries.Clear();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (_cachedRoomList.ContainsKey(info.Id))
                {
                    _cachedRoomList.Remove(info.Id);
                }

                continue;
            }

            // Update cached room info
            if (_cachedRoomList.ContainsKey(info.Id))
            {
                _cachedRoomList[info.Id] = info;
            }

            // Add new room info to cache
            else
            {
                _cachedRoomList.Add(info.Id, info);
            }
        }

        Logger.Warning($"_cachedRoomList with number of rooms: {_cachedRoomList.Count}");
    }

    private void UpdateRoomListView()
    {
        Logger.Log("UpdateRoomListView");
        foreach (RoomInfo info in _cachedRoomList.Values)
        {
            string roomName = info.CustomProperties["Name"].ToString();
            string version = info.CustomProperties["Version"].ToString();
            GameObject entry = Instantiate(_roomListEntryPrefab);
            entry.transform.SetParent(_roomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomListEntry>().Initialize(info.Id, roomName, (byte)info.PlayerCount, info.MaxPlayers, version, _launcher);

            _roomListEntries.Add(info.Id, entry);
        }
    }

    public override void OnJoinedLobby()
    {
        // whenever this joins a new lobby, clear any previous room lists
        _cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnLeftLobby()
    {
        Logger.Log("Left lobby");
        _cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public void BackToMain()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        _launcher.ShowMainUI();
    }

    public void OnRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            Logger.Log("join the lobby");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Logger.Warning("Failed to create room from here?");
        _launcher.ShowMainUI();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == 32765)
        {
            _launcher.SetErrorText("Could not join game because the game was full");
        }
        else if (returnCode == 32758)
        {
            _launcher.SetErrorText("Could not join game because the game was abandoned");
        }
        else
        {
            Logger.Error($"Join Room failed with return code {returnCode} and message {message}");
        }

        if (!PhotonNetwork.InLobby)
        {
            Logger.Log("join the lobby");
            PhotonNetwork.JoinLobby();
        }
    }
}
