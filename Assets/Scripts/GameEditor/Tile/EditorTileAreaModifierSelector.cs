using System.Collections.Generic;


public class EditorTileAreaModifierSelector : EditorTileModifierSelector
{
    public EditorTileAreaModifierSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        EditorTileModifierCategory currentCategory = EditorTileModifierCategory.Area;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        if (EditorModificationPanelContainer.Instance.SelectedPanel is IEditorTileModificationPanel)
        {
            IEditorTileModificationPanel selectedPanel = EditorModificationPanelContainer.Instance.SelectedPanel as IEditorTileModificationPanel;
            selectedPanel.DestroyModifierActions();
        }

        int selectedModifierIndex = EditorManager.SelectedTileAreaModifierIndex;
        int newIndex = selectedModifierIndex + newValue;

        if (newIndex < 0)
        {
            EditorTileModifierCategory previousEditorTileModifierCategory = PreviousEditorTileModfierCategory(currentCategory);

            int modifierCount = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[previousEditorTileModifierCategory].Count;

            int lastAvailableIndex = modifierCount - 1;

            selectedTileModifierContainer.SetSelectedTileModifierCategory(previousEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedTileModifier(lastAvailableIndex);
        }
        else if (newIndex >= _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Area].Count)
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
        List<IEditorTileModifier> currentlyAvailableTileAreaModifiers = _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Area];

        if (currentlyAvailableTileAreaModifiers.Count == 0)
        {
            EditorManager.SelectedTileAreaModifierIndex = 0;
            return;
        }

        EditorTileAreaModifier areaModifier = currentlyAvailableTileAreaModifiers[modifierIndex] as EditorTileAreaModifier;
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(areaModifier.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = areaModifier.GetSprite();
        EditorManager.SelectedTileAreaModifierIndex = modifierIndex;

        areaModifier.InstantiateModifierActions();
    }
}
