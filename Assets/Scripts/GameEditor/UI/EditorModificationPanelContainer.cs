using UnityEngine;

public class EditorModificationPanelContainer : MonoBehaviour
{
    public static EditorModificationPanelContainer Instance;

    public IEditorModificationPanel SelectedPanel;

    [SerializeField] private GameObject _mazeLevelTabButton;
    [SerializeField] private GameObject _mazeTileTabButton;
    [SerializeField] private GameObject _overworldTabButton;
    [SerializeField] private GameObject _overworldTileTabButton;

    [SerializeField] private EditorMazeModificationPanel _mazeLevelModificationPanel;
    [SerializeField] private EditorMazeTileModificationPanel _mazeTileModificationPanel;
    [SerializeField] private EditorOverworldModificationPanel _overworldModificationPanel;
    [SerializeField] private EditorOverworldTileModificationPanel _overworldTileModificationPanel;

    public void Awake()
    {
        Guard.CheckIsNull(_mazeLevelTabButton, "_mazeLevelTabButton");
        Guard.CheckIsNull(_mazeTileTabButton, "_mazeTileTabButton");
        Guard.CheckIsNull(_overworldTabButton, "_overworldTabButton");
        Guard.CheckIsNull(_overworldTileTabButton, "_overworldTileTabButton");

        Guard.CheckIsNull(_mazeLevelModificationPanel, "_mazeLevelModificationPanel");
        Guard.CheckIsNull(_mazeTileModificationPanel, "_mazeTileModificationPanel");
        Guard.CheckIsNull(_overworldModificationPanel, "_overworldModificationPanel");
        Guard.CheckIsNull(_overworldTileModificationPanel, "_overworldTileModificationPanel");

        Instance = this;
    }

    public void OnEnable()
    {
        UpdateForSceneChange();
    }

    public void UpdateForSceneChange()
    {
        Logger.Warning(Logger.Editor, $"Show tabs. Scene type is {PersistentGameManager.CurrentSceneType}");

        switch (PersistentGameManager.CurrentSceneType)
        {
            case SceneType.Overworld:
                _mazeLevelTabButton.SetActive(false);
                _mazeTileTabButton.SetActive(false);
                _overworldTabButton.SetActive(true);
                _overworldTileTabButton.SetActive(true);

                SelectOverworldModificationPanel();
                break;
            case SceneType.Maze:
                _mazeLevelTabButton.SetActive(true);
                _mazeTileTabButton.SetActive(true);
                _overworldTabButton.SetActive(false);
                _overworldTileTabButton.SetActive(false);

                SelectMazeLevelModificationPanel();
                break;
            default:
                Logger.Error($"Unknown scene type {PersistentGameManager.CurrentSceneType}");
                break;
        }
    }

    private void SelectPanel(IEditorModificationPanel panel)
    {
        if(SelectedPanel != null)
        {
            if(panel == SelectedPanel)
            {
                return;
            }

            SelectedPanel.Close();
        }

        panel.Open();
        SelectedPanel = panel;
    }

    public void SelectMazeLevelModificationPanel()
    {
        SelectPanel(_mazeLevelModificationPanel);
    }

    public void SelectMazeTileModificationPanel()
    {
        SelectPanel(_mazeTileModificationPanel);
    }

    public void SelectOverworldModificationPanel()
    {
        SelectPanel(_overworldModificationPanel);
    }

    public void SelectOverworldTileModificationPanel()
    {
        SelectPanel(_overworldTileModificationPanel);
    }
}
