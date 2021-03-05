using UnityEngine;

public class MainScreenOverlayCanvas : MonoBehaviour
{
    public static MainScreenOverlayCanvas Instance;

    public GameObject ConsoleContainer;
    public GameObject ConsoleButton;

    [SerializeField] private GameObject _gameEditorUIPrefab;

    public BlackOutSquare BlackOutSquare;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(ConsoleContainer, "ConsoleContainer", gameObject);
        Guard.CheckIsNull(ConsoleButton, "ConsoleButton", gameObject);

        Guard.CheckIsNull(_gameEditorUIPrefab, "GameEditorUIPrefab", gameObject);

        Guard.CheckIsNull(BlackOutSquare, "BlackOutSquare", gameObject);

        if (PersistentGameManager.CurrentPlatform == Platform.Android)
        {
            ConsoleButton.SetActive(true);
        }
    }

    public void Start()
    {
        if (EditorCanvasUI.Instance == null)
        {
            GameObject editorUI = Instantiate(_gameEditorUIPrefab);
            editorUI.SetActive(false);
        }
        else
        {
            EditorCanvasUI.Instance.UpdateCanvasForSceneChange();
        }
    }


}
