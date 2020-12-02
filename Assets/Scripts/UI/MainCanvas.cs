using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance;

    [SerializeField] private GameManager _gameManager = null;
    public GameObject ConsoleContainer;
    public GameObject ConsoleButton;

    public BlackOutSquare BlackOutSquare;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(ConsoleContainer, "ConsoleContainer", gameObject);
        Guard.CheckIsNull(ConsoleButton, "ConsoleButton", gameObject);

        Guard.CheckIsNull(BlackOutSquare, "BlackOutSquare", gameObject);

        if (_gameManager.CurrentPlatform == Platform.Android)
        {
            ConsoleButton.SetActive(true);
        }
    }
}
