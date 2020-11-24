public abstract class EditorMazeTileModifierSelector
{
    protected EditorSelectedModifierContainer _editorSelectedModifierContainer;

    protected EditorMazeTileModifierSelector(EditorSelectedModifierContainer editorSelectedModifierContainer)
    {
        _editorSelectedModifierContainer = editorSelectedModifierContainer;
    }

    public virtual void SwitchSelectedModifier(int newValue) { }
    public virtual void SetSelectedModifier(int modifierIndex) { }

    protected string GetSelectedModifierLabel(string modifierName)
    {
        return $"Selected: {modifierName}";
    }
}