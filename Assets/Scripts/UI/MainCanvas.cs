using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance;

    [SerializeField] private GameManager _gameManager = null;
    public GameObject ConsoleContainer;
    public GameObject ConsoleButton;

    [Space(10)]
    [Header("Player graphics")]
    public Sprite Player1TileMarker;    // TODO maybe move to PlayerCharacter class so we can say player.marker
    public Sprite Player2TileMarker;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(ConsoleContainer, "ConsoleContainer");
        Guard.CheckIsNull(ConsoleButton, "ConsoleButton");

        if (Player1TileMarker == null)
            Logger.Error(Logger.Initialisation, "Could not find Player1TileMarker component on MainCanvas");
        if (Player2TileMarker == null)
            Logger.Error(Logger.Initialisation, "Could not find Player2TileMarker component on MainCanvas");

        if (_gameManager.CurrentPlatform == Platform.Android)
        {
            ConsoleButton.SetActive(true);
        }
    }
}
