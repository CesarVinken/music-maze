using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeUI : MonoBehaviour
{
    [SerializeField] private Launcher _launcher;

    [SerializeField] private GameObject _hostGameButtonGO = null;
    [SerializeField] private GameObject _joinGameButtonGO = null;

    [SerializeField] private InputField _roomNameField = null;

    public void Awake()
    {
        Guard.CheckIsNull(_hostGameButtonGO, "_hostGameButtonGO", gameObject);
        Guard.CheckIsNull(_joinGameButtonGO, "_joinGameButtonGO", gameObject);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void HostGame()
    {
        _launcher.HostGame();
    }

    public void JoinGame()
    {
        //Show game list
        _launcher.OpenGameList();
    }

}
