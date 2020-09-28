using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private MazeNetworkManager _networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject _landingPagePanel = null;
    [SerializeField] private InputField _ipAddressInputField = null;
    [SerializeField] private Button _joinButton = null;

    private void OnEnable()
    {
        MazeNetworkManager.OnClientConnected += HandleClientConnected;
        MazeNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        MazeNetworkManager.OnClientConnected -= HandleClientConnected;
        MazeNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        Logger.Log("join lobby");
        string ipAddress = _ipAddressInputField.text;

        _networkManager.networkAddress = ipAddress;
        _networkManager.StartClient();
        Logger.Log("_networkManager.networkAddress {0}", _networkManager.networkAddress);

        _joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        _joinButton.interactable = true;

        gameObject.SetActive(false);
        _landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        _joinButton.interactable = true;
    }
}
