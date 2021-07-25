using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.UI;

public class RoomListEntry : MonoBehaviour
{
    public Text RoomNameText;
    public Text RoomPlayersText;
    public Button JoinRoomButton;

    private string _roomName;
    private byte _currentPlayers;
    private Launcher _launcher;

    public void Start()
    {
        JoinRoomButton.onClick.AddListener(() =>
        {
            //if(_currentPlayers == 2)
            //{
            //    _launcher.SetErrorText("Room is full");
            //    return;
            //}

            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(_roomName);
        });
    }

    public void Initialize(string name, byte currentPlayers, byte maxPlayers, Launcher launcher)
    {
        _roomName = name;
        _currentPlayers = currentPlayers;

        if(currentPlayers == 2)
        {
            JoinRoomButton.interactable = false;
        }
        else
        {
            JoinRoomButton.interactable = true;
        }
        RoomNameText.text = name;
        RoomPlayersText.text = currentPlayers + " / " + maxPlayers;
    }
}
