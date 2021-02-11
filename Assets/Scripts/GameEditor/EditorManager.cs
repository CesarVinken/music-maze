
public static class EditorManager
{
    private static bool _inEditor = false;
    public static bool InEditor
    {
        get { return _inEditor; }
        set
        {

            _inEditor = value;
        }
    }

    public static EditorMazeTileModifierType SelectedMazeTileModifierCategory;
    public static IEditorMazeTileModifierType SelectedMazeTileModifier;

    public static int SelectedMazeTileAttributeModifierIndex;
    public static int SelectedMazeTileBackgroundModifierIndex;
    public static int SelectedMazeTileTransformationTriggererIndex;

    public static void ToggleEditorMode()
    {
        if (_inEditor)
        {
            CloseEditor();
            return;
        }
        OpenEditor();
    }

    public static void OpenEditor()
    {
        _inEditor = true;
        EditorCanvasUI.Instance.InitialiseEditor();
        EditorWorldContainer.Instance.InitialiseEditor();
        Logger.Log($"Our current scene type is {GameManager.CurrentSceneType}");
        switch (GameManager.CurrentSceneType)
        {
            case SceneType.Overworld:
                //EditorCanvasUI.Instance.OverworldModificationPanel.GenerateTiles();
                Logger.Log("To do: auto generate overworld tiles");
                break;
            case SceneType.Maze:
                EditorCanvasUI.Instance.MazeModificationPanel.GenerateTiles();
                break;
            default:
                Logger.Error($"Unknown scene type {GameManager.CurrentSceneType} is not implemented");
                break;
        }
    }

    public static void CloseEditor()
    {
        _inEditor = false;
        EditorCanvasUI.Instance.CloseEditor();
        EditorWorldContainer.Instance.CloseEditor();
    }
}
