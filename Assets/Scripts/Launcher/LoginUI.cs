using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private Launcher _launcher;

    [SerializeField] private InputField _playerNameField = null;

    public void Awake()
    {
        Guard.CheckIsNull(_playerNameField, "_playerNameField", gameObject);
    }

    // LoginUI is instantly turned on by the launcher
    public void TurnOn()
    {
        EventSystem.current.SetSelectedGameObject(_playerNameField.gameObject);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void Login()
    {
        string playerName = _playerNameField.text;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }
}
