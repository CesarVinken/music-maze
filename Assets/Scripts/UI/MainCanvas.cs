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

        Guard.CheckIsNull(ConsoleContainer, "ConsoleContainer", gameObject);
        Guard.CheckIsNull(ConsoleButton, "ConsoleButton", gameObject);

        Guard.CheckIsNull(Player1TileMarker, "Player1TileMarker", gameObject);
        Guard.CheckIsNull(Player2TileMarker, "Player2TileMarker", gameObject);

        if (_gameManager.CurrentPlatform == Platform.Android)
        {
            ConsoleButton.SetActive(true);
        }
    }
}
