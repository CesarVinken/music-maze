using System.Collections.Generic;

public class EditorTileAttributeSelector : EditorTileModifierSelector
{
    public EditorTileAttributeSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        EditorTileModifierCategory currentCategory = EditorTileModifierCategory.Attribute;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        if (EditorModificationPanelContainer.Instance.SelectedPanel is IEditorTileModificationPanel)
        {
            IEditorTileModificationPanel selectedPanel = EditorModificationPanelContainer.Instance.SelectedPanel as IEditorTileModificationPanel;
            selectedPanel.DestroyModifierActions();
        }

        int selectedAttributeIndex = EditorManager.SelectedTileAttributeModifierIndex;
        int newIndex = selectedAttributeIndex + newValue;

        if (newIndex < 0)
        {
            EditorTileModifierCategory previousEditorTileModifierCategory = PreviousEditorTileModfierCategory(currentCategory);

            int modifierCount = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[previousEditorTileModifierCategory].Count;
            Logger.Log($"How many attributes do we have? {modifierCount}");
            int lastAvailableIndex = modifierCount - 1;

            selectedTileModifierContainer.SetSelectedTileModifierCategory(previousEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedTileModifier(lastAvailableIndex);
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorTileAttributes.Count)
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
        Logger.Log(modifierIndex);
        List<IEditorTileModifier> currentlyAvailableTileAttributes = _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Attribute];
        
        if (currentlyAvailableTileAttributes.Count == 0)
        {
            EditorManager.SelectedTileAttributeModifierIndex = 0;
            return;
        }

        EditorTileAttributeModifier attribute = currentlyAvailableTileAttributes[modifierIndex] as EditorTileAttributeModifier;
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(attribute.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = attribute.GetSprite();
        EditorManager.SelectedTileAttributeModifierIndex = modifierIndex;

        attribute.InstantiateModifierActions();
    }
}
