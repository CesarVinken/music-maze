using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance;

    [SerializeField] private GameManager _gameManager;
    public GameObject ConsoleContainer;
    public GameObject ConsoleButton;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(ConsoleContainer, "ConsoleContainer");
        Guard.CheckIsNull(ConsoleButton, "ConsoleButton");

        if (_gameManager.CurrentPlatform == Platform.Android)
        {
            ConsoleButton.SetActive(true);
        }
    }
}
