public class EditorTileMainMaterialSelector : EditorTileModifierSelector
{
    public EditorTileMainMaterialSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        EditorTileModifierCategory currentCategory = EditorTileModifierCategory.MainMaterial;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        if (EditorModificationPanelContainer.Instance.SelectedPanel is IEditorTileModificationPanel)
        {
            IEditorTileModificationPanel selectedPanel = EditorModificationPanelContainer.Instance.SelectedPanel as IEditorTileModificationPanel;
            selectedPanel.DestroyModifierActions();
        }

        int selectedMainMaterialIndex = EditorManager.SelectedTileMainMaterialModifierIndex;
        int newIndex = selectedMainMaterialIndex + newValue;

        if (newIndex < 0)
        {
            EditorTileModifierCategory previousEditorTileModifierCategory = PreviousEditorTileModfierCategory(currentCategory);

            int modifierCount = selectedTileModifierContainer.ModifierCountByCategories[previousEditorTileModifierCategory];

            selectedTileModifierContainer.SetSelectedTileModifierCategory(previousEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedTileModifier(modifierCount - 1);
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorTileMainMaterials.Count)
        {
            EditorTileModifierCategory nextEditorTileModifierCategory = NextEditorTileModfierCategory(currentCategory);

            selectedTileModifierContainer.SetSelectedTileModifierCategory(nextEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedTileModifier(0);
        }
        else
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        if (_editorSelectedModifierContainer.EditorTileMainMaterials.Count == 0) return;

        EditorTileMainMaterialModifier attribute = _editorSelectedModifierContainer.EditorTileMainMaterials[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(attribute.Name);
        EditorManager.SelectedTileMainMaterialModifierIndex = modifierIndex;
    }
}
