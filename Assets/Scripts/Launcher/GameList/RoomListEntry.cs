using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.UI;

public class RoomListEntry : MonoBehaviour
{
    public Text RoomNameText;
    public Text RoomPlayersText;
    public Button JoinRoomButton;

    private string _roomId;
    private byte _currentPlayers;
    private Launcher _launcher;

    public void Start()
    {
        JoinRoomButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(_roomId);
        });
    }

    public void Initialize(string id, string name, byte currentPlayers, byte maxPlayers, Launcher launcher)
    {
        _roomId = id;
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

    public void UpdateRoomName(string newRoomName)
    {
        // We do not update the _fixedRoomName because this is used as an identifier of the room by photon and will after creation not be updated.
        RoomNameText.text = newRoomName;
    }
}
