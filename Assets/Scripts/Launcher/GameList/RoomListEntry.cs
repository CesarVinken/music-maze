using Console;
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

            // check version number

            PhotonNetwork.JoinRoom(_roomId);
        });
    }

    public void Initialize(string id, string name, byte currentPlayers, byte maxPlayers, string version, Launcher launcher)
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

        bool versionsCompatible = CompareVersionNumbers(PersistentGameManager.VersionNumber, version);
        if (!versionsCompatible)
        {
            RoomNameText.text = $"{name} <color={ConsoleConfiguration.ErrorColour}> - incompatible V.{version}</color>";
            JoinRoomButton.interactable = false;
        }
        else
        {
            RoomNameText.text = name;
        }

        RoomPlayersText.text = currentPlayers + " / " + maxPlayers;
    }

    public void UpdateRoomName(string newRoomName)
    {
        // We do not update the _fixedRoomName because this is used as an identifier of the room by photon and will after creation not be updated.
        RoomNameText.text = newRoomName;
    }

    private bool CompareVersionNumbers(string ourVersion, string theirVersion)
    {
        string[] ourVersionCutUp = ourVersion.Split('.');
        string[] theirVersionCutUp = theirVersion.Split('.');

        if (ourVersionCutUp.Length != 4)
        {
            Logger.Error($"Our version does not have the expected number of sub versions. The full version description is: {ourVersion}");
            return false;
        }
        if (theirVersionCutUp.Length != 4)
        {
            Logger.Error($"Their version does not have the expected number of sub versions. The full version description is: {theirVersion}");
            return false;
        }

        string ourVersionMeta = ourVersionCutUp[0];
        string ourVersionMain = ourVersionCutUp[1];
        string ourVersionSub = ourVersionCutUp[2];
        string ourVersionSubBuild = ourVersionCutUp[3];

        string theirVersionMeta = theirVersionCutUp[0];
        string theirVersionMain = theirVersionCutUp[1];
        string theirVersionSub = theirVersionCutUp[2];
        string theirVersionSubBuild = theirVersionCutUp[3];

        if (ourVersionMeta != theirVersionMeta)
        {
            Logger.Warning($"Our meta version is not compatible with their meta version Us: {ourVersion}. Them: {theirVersion}");
            return false;
        }
        if (ourVersionMain != theirVersionMain)
        {
            Logger.Warning($"Our main version is not compatible with their main version Us: {ourVersion}. Them: {theirVersion}");
            return false;
        }
        if (ourVersionSub != theirVersionSub)
        {
            Logger.Warning($"Our sub version is not compatible with their sub version Us: {ourVersion}. Them: {theirVersion}");
            return false;
        }
        if (ourVersionSubBuild != theirVersionSubBuild)
        {
            Logger.Warning($"Our sub build version is not compatible with their sub build version Us: {ourVersion}. Them: {theirVersion}");
            return false;
        }

        return true;
    }
}
