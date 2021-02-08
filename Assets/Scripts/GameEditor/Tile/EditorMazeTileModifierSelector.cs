public abstract class EditorMazeTileModifierSelector
{
    protected EditorSelectedTileModifierContainer _editorSelectedModifierContainer;

    protected EditorMazeTileModifierSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer)
    {
        _editorSelectedModifierContainer = editorSelectedModifierContainer;
    }

    public virtual void SwitchSelectedModifier(int newValue) { }
    public virtual void SetSelectedModifier(int modifierIndex) { }

    protected string GetSelectedModifierLabel(string modifierName)
    {
        return $"{modifierName}";
    }
}