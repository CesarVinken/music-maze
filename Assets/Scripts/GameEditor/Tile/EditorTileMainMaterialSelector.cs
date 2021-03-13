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

            int modifierCount = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[previousEditorTileModifierCategory].Count;
            int lastAvailableIndex = modifierCount - 1;

            selectedTileModifierContainer.SetSelectedTileModifierCategory(previousEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedTileModifier(lastAvailableIndex);
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

        EditorTileMainMaterialModifier mainMaterial = _editorSelectedModifierContainer.EditorTileMainMaterials[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(mainMaterial.Name); 
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = mainMaterial.GetSprite();

        IEditorTileModificationPanel selectedPanel = EditorModificationPanelContainer.Instance.SelectedPanel as IEditorTileModificationPanel;
        selectedPanel.SetMainMaterialIcon(mainMaterial.GetSprite());

        EditorManager.SelectedTileMainMaterialModifierIndex = modifierIndex;
    }
}
