using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameListUI : MonoBehaviourPunCallbacks
{
    //public static GameListUI Instance;

    [SerializeField] private Launcher _launcher;

    public void Awake()
    {
        //Instance = this;

        Guard.CheckIsNull(_launcher, "_launcher", gameObject);

        PhotonNetwork.AutomaticallySyncScene = true;

        _launcher.SetErrorText("");
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);    
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void JoinRoom()
    {
        _launcher.LaunchMultiplayerGame();
    }
}
