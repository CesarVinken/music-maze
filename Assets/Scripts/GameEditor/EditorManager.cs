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
        EditorContainer.Instance.InitialiseEditor();
    }

    public static void CloseEditor()
    {
        _inEditor = false;
        EditorContainer.Instance.CloseEditor();
    }
}
