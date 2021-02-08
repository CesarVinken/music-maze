using UnityEngine;

public class EditorModificationPanelContainer : MonoBehaviour
{
    public static EditorModificationPanelContainer Instance;

    public IEditorModificationPanel SelectedPanel;

    [SerializeField] private EditorLevelModificationPanel _levelModificationPanel;
    [SerializeField] private EditorTileModificationPanel _tileModificationPanel;

    public void Awake()
    {
        Guard.CheckIsNull(_levelModificationPanel, "_levelModificationPanel");
        Guard.CheckIsNull(_tileModificationPanel, "_tileModificationPanel");

        Instance = this;
    }

    public void Start()
    {
        SelectLevelModificationPanel();
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

    public void SelectLevelModificationPanel()
    {
        SelectPanel(_levelModificationPanel);
    }

    public void SelectTileModificationPanel()
    {
        SelectPanel(_tileModificationPanel);
    }
}
