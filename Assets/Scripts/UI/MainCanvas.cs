using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance;

    public GameObject ConsoleContainer;
    public GameObject ConsoleButton;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(ConsoleContainer, "ConsoleContainer");
        Guard.CheckIsNull(ConsoleButton, "ConsoleButton");

        //if (GameManager.Instance.CurrentPlatform == Platform.Android)
        //{
        //    ConsoleButton.SetActive(true);
        //}
    }
}
