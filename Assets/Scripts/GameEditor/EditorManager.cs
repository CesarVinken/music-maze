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

    public static MazeLevel EditorLevel = null;

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
        EditorUIContainer.Instance.InitialiseEditor();
        EditorWorldContainer.Instance.InitialiseEditor();
    }

    public static void CloseEditor()
    {
        _inEditor = false;
        EditorUIContainer.Instance.CloseEditor();
        EditorWorldContainer.Instance.CloseEditor();
    }
}
