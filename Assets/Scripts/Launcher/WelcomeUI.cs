using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WelcomeUI : MonoBehaviour
{
    //public static WelcomeUI Instance;
    [SerializeField] private Launcher _launcher;

    [SerializeField] private GameObject _hostGameButtonGO = null;
    [SerializeField] private GameObject _joinGameButtonGO = null;

    [SerializeField] private InputField _playerNameField = null;
    [SerializeField] private InputField _roomNameField = null;

    public void Awake()
    {
        Guard.CheckIsNull(_playerNameField, "_playerNameField", gameObject);
        //Guard.CheckIsNull(_roomNameField, "_roomNameField", gameObject);

        Guard.CheckIsNull(_hostGameButtonGO, "_hostGameButtonGO", gameObject);
        Guard.CheckIsNull(_joinGameButtonGO, "_joinGameButtonGO", gameObject);
    }

    // WelcomeUI is turned on by the launcher as soon as we are connected
    public void TurnOn()
    {
        EventSystem.current.SetSelectedGameObject(_playerNameField.gameObject);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    Logger.Log(EventSystem.current.currentSelectedGameObject.name);
        //    if (EventSystem.current.currentSelectedGameObject.Equals(_playerNameField.gameObject))
        //    {
        //        EventSystem.current.SetSelectedGameObject(_roomNameField.gameObject);
        //    }
        //    else if (EventSystem.current.currentSelectedGameObject.Equals(_roomNameField.gameObject))
        //    {
        //        EventSystem.current.SetSelectedGameObject(_joinRoomButtonGO);
        //    }
        //}
    }

    public void HostGame()
    {
        //Create new game romo
        _launcher.JoinGameRoom();
    }

    public void JoinGame()
    {
        //Show game list
        _launcher.OpenGameList();
    }

}
