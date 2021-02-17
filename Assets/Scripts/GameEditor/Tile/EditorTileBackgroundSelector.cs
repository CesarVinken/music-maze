﻿public class EditorTileBackgroundSelector : EditorTileModifierSelector
{
    public EditorTileBackgroundSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        EditorTileModifierCategory currentCategory = EditorTileModifierCategory.Background;
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

            EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifierCategory(previousEditorTileModifierCategory);
            EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifier(modifierCount - 1);
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorTileBackgrounds.Count)
        {
            EditorTileModifierCategory nextEditorTileModifierCategory = NextEditorTileModfierCategory(currentCategory);

            EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifierCategory(nextEditorTileModifierCategory);
            EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifier(0);
        }
        else
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        EditorTileBackgroundModifier background = _editorSelectedModifierContainer.EditorTileBackgrounds[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(background.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = background.GetSprite();
        EditorManager.SelectedTileBackgroundModifierIndex = modifierIndex;
    }
}
