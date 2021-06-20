using System.Collections.Generic;

public abstract class EditorTileModifierSelector
{
    protected EditorSelectedTileModifierContainer _editorSelectedModifierContainer;

    protected EditorTileModifierSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer)
    {
        _editorSelectedModifierContainer = editorSelectedModifierContainer;
    }

    public virtual void SwitchSelectedModifier(int newValue) { }
    public virtual void SetSelectedModifier(int modifierIndex) { }

    protected void SwitchSelectedModifier(int newIndex, EditorTileModifierCategory currentCategory)
    {
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        if (EditorModificationPanelContainer.Instance.SelectedPanel is IEditorTileModificationPanel)
        {
            IEditorTileModificationPanel selectedPanel = EditorModificationPanelContainer.Instance.SelectedPanel as IEditorTileModificationPanel;
            selectedPanel.DestroyModifierActions();
        }

        // try previous category
        if (newIndex < 0)
        {
            EditorTileModifierCategory previousEditorTileModifierCategory = PreviousEditorTileModifierCategory(currentCategory);

            bool landedAtCategory = false;

            while (!landedAtCategory)
            {
                if (selectedTileModifierContainer.CurrentlyAvailableTileModifiers.TryGetValue(previousEditorTileModifierCategory, out List<IEditorTileModifier> editorTileModifiers))
                {
                    int modifierCount = editorTileModifiers.Count;
                    int lastAvailableIndex = modifierCount - 1;
                    selectedTileModifierContainer.SetSelectedTileModifierCategory(previousEditorTileModifierCategory);
                    selectedTileModifierContainer.SetSelectedTileModifier(lastAvailableIndex);
                    landedAtCategory = true;
                }
                else
                {
                    // there are no modifiers in the previous category. Try the category before that
                    EditorTileModifierCategory lastQueriedModifierCategory = previousEditorTileModifierCategory;
                    previousEditorTileModifierCategory = PreviousEditorTileModifierCategory(lastQueriedModifierCategory);
                }
            }
        }
        else if (newIndex >= _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[currentCategory].Count) // try next category
        {
            EditorTileModifierCategory nextEditorTileModifierCategory = NextEditorTileModifierCategory(currentCategory);

            bool landedAtCategory = false;

            while (!landedAtCategory)
            {
                if (selectedTileModifierContainer.CurrentlyAvailableTileModifiers.TryGetValue(nextEditorTileModifierCategory, out List<IEditorTileModifier> editorTileModifiers))
                {
                    selectedTileModifierContainer.SetSelectedTileModifierCategory(nextEditorTileModifierCategory);
                    selectedTileModifierContainer.SetSelectedTileModifier(0);
                    landedAtCategory = true;
                }
                else
                {
                    // there are no modifiers in the next category. Try the category after that
                    EditorTileModifierCategory lastQueriedModifierCategory = nextEditorTileModifierCategory;
                    nextEditorTileModifierCategory = NextEditorTileModifierCategory(lastQueriedModifierCategory);
                }
            }
        }
    }

    protected string GetSelectedModifierLabel(string modifierName)
    {
        return $"{modifierName}";
    }

    protected EditorTileModifierCategory PreviousEditorTileModifierCategory(EditorTileModifierCategory currentTileModifierCategory)
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

    protected EditorTileModifierCategory NextEditorTileModifierCategory(EditorTileModifierCategory currentTileModifierCategory)
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