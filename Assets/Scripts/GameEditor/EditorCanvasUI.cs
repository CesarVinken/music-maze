using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorCanvasUI : MonoBehaviour
{
    public static EditorCanvasUI Instance;

    public GameObject EditorModeStatusTextGO;
    public GameObject PlayableLevelsPanelGO;

    [SerializeField] private GameObject _gameEditorWorldPrefab;

    public Sprite DefaultIcon;
    public Sprite[] TileAttributeIcons;

    public EditorMazeModificationPanel MazeModificationPanel;
    public EditorOverworldModificationPanel OverworldModificationPanel;
    public EditorSelectedTileModifierContainer SelectedTileModifierContainer;

    [Space(10)]
    [Header("Tile Modifier Actions")]
    public GameObject AssignMazeLevelEntryPrefab;
    public GameObject GenerateTileTransformationMapPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorModeStatusTextGO, "EditorModeStatusTextGO", gameObject);
        Guard.CheckIsNull(PlayableLevelsPanelGO, "PlayableLevelsPanelGO", gameObject);

        Guard.CheckIsNull(_gameEditorWorldPrefab, "_gameEditorWorldPrefab");

        Guard.CheckIsNull(MazeModificationPanel, "MazeModificationPanel", gameObject);
        Guard.CheckIsNull(OverworldModificationPanel, "OverworldModificationPanel", gameObject);

        Guard.CheckIsNull(AssignMazeLevelEntryPrefab, "AssignMazeLevelEntryPrefab");
        Guard.CheckIsNull(GenerateTileTransformationMapPrefab, "GenerateTileTransformationMapPrefab");

        GameObject.DontDestroyOnLoad(gameObject);

        if(EditorWorldContainer.Instance == null)
        {
            GameObject editorUI = Instantiate(_gameEditorWorldPrefab);
        }
    }

    public void UpdateCanvasForSceneChange()
    {
        SwitchEditorButton.Instance.SetButtonLabel();
        EditorModificationPanelContainer.Instance.UpdateForSceneChange();
    }

    public void InitialiseEditor()
    {
        EditorModeStatusTextGO.SetActive(true);
        gameObject.SetActive(true);

        EditorWorldContainer.Instance.ShowTileSelector();
    }

    public void CloseEditor()
    {
        EditorModeStatusTextGO.SetActive(false);
        gameObject.SetActive(false);

        EditorWorldContainer.Instance.HideTileSelector();
    }

    public void LoadOverworldEditor()
    {
        PersistentGameManager.SceneLoadOrigin = SceneLoadOrigin.Editor;

        if(PersistentGameManager.OverworldName == "")
        {
            PersistentGameManager.SetOverworldName("overworld");
        }

        PersistentGameManager.SetLastMazeLevelName(PersistentGameManager.CurrentSceneName);
        PersistentGameManager.SetCurrentSceneName(PersistentGameManager.OverworldName);

        PhotonNetwork.LoadLevel("Overworld");
    }

    public void LoadMazeEditor()
    {
        PersistentGameManager.SceneLoadOrigin = SceneLoadOrigin.Editor;

        string mazeName = "default";
        PersistentGameManager.SetLastMazeLevelName(mazeName);
        PersistentGameManager.SetCurrentSceneName(mazeName);

        PhotonNetwork.LoadLevel("Maze");
    }
}
