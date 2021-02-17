public class EditorTileTransformationTriggererSelector : EditorTileModifierSelector
{
    public EditorTileTransformationTriggererSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        EditorTileModifierCategory currentCategory = EditorTileModifierCategory.TransformationTriggerer;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;
        
        if (EditorModificationPanelContainer.Instance.SelectedPanel is IEditorTileModificationPanel)
        {
            IEditorTileModificationPanel selectedPanel = EditorModificationPanelContainer.Instance.SelectedPanel as IEditorTileModificationPanel;
            selectedPanel.DestroyModifierActions();
        }

        int selectedBackgroundIndex = EditorManager.SelectedTileBackgroundModifierIndex;
        int newIndex = selectedBackgroundIndex + newValue;

        if (newIndex < 0)
        {
            EditorTileModifierCategory previousEditorTileModifierCategory = PreviousEditorTileModfierCategory(currentCategory);
            int modifierCount = selectedTileModifierContainer.ModifierCountByCategories[previousEditorTileModifierCategory];

            selectedTileModifierContainer.SetSelectedTileModifierCategory(previousEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedTileModifier(modifierCount - 1);

            //EditorMazeTileModificationPanel.Instance.DestroyModifierActions();
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorTileBackgrounds.Count)
        {
            EditorTileModifierCategory nextEditorTileModifierCategory = NextEditorTileModfierCategory(currentCategory);

            selectedTileModifierContainer.SetSelectedTileModifierCategory(nextEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedTileModifier(0);

            //EditorMazeTileModificationPanel.Instance.DestroyModifierActions();
        }
        else
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        EditorTileTransformationModifier transformationTrigger = _editorSelectedModifierContainer.EditorTileTransformationTriggerers[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(transformationTrigger.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = transformationTrigger.GetSprite();
        EditorManager.SelectedTileTransformationTriggererIndex = modifierIndex;

        transformationTrigger.InstantiateModifierActions();
    }
}
