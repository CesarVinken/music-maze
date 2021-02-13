public abstract class EditorTileModifierSelector
{
    protected EditorSelectedTileModifierContainer _editorSelectedModifierContainer;

    protected EditorTileModifierSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer)
    {
        _editorSelectedModifierContainer = editorSelectedModifierContainer;
    }

    public virtual void SwitchSelectedModifier(int newValue) { }
    public virtual void SetSelectedModifier(int modifierIndex) { }

    protected string GetSelectedModifierLabel(string modifierName)
    {
        return $"{modifierName}";
    }

    protected EditorTileModifierCategory PreviousEditorTileModfierCategory(EditorTileModifierCategory currentTileModifierCategory)
    {
        if (_editorSelectedModifierContainer.UsedTileModifierCategories.Count == 1)
        {
            return currentTileModifierCategory;
        }

        int index = _editorSelectedModifierContainer.UsedTileModifierCategories.IndexOf(currentTileModifierCategory);

        if(index > 0)
        {
            return _editorSelectedModifierContainer.UsedTileModifierCategories[index - 1];
        }
        return _editorSelectedModifierContainer.UsedTileModifierCategories[_editorSelectedModifierContainer.UsedTileModifierCategories.Count - 1];
    }

    protected EditorTileModifierCategory NextEditorTileModfierCategory(EditorTileModifierCategory currentTileModifierCategory)
    {
        if (_editorSelectedModifierContainer.UsedTileModifierCategories.Count == 1)
        {
            return currentTileModifierCategory;
        }

        int index = _editorSelectedModifierContainer.UsedTileModifierCategories.IndexOf(currentTileModifierCategory);

        if (index < _editorSelectedModifierContainer.UsedTileModifierCategories.Count - 1)
        {
            return _editorSelectedModifierContainer.UsedTileModifierCategories[index + 1];
        }
        return _editorSelectedModifierContainer.UsedTileModifierCategories[0];
    }
}