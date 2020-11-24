
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

    public static EditorMazeTileModifierType SelectedMazeTileModifierType; // Are we selecting an Attribute or a Background?
    public static IEditorMazeTileModifierType SelectedMazeTileModifier;

    public static int SelectedMazeTileAttributeModifierIndex;
    public static int SelectedMazeTileBackgroundModifierIndex;

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
