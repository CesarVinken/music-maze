using System.Collections.Generic;

public class EditorTileBackgroundSelector : EditorTileModifierSelector
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
            int modifierCount = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[previousEditorTileModifierCategory].Count;

            // Get currently selected material
            // Get available backgrounds for currently selected material
            // Select last of the currently available Backgrounds


            EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifierCategory(previousEditorTileModifierCategory);

            int lastAvailableIndex = modifierCount - 1;
            EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifier(lastAvailableIndex);
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
        List<IEditorTileModifier> currentlyAvailableTileBackgrounds = _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Background];

        if (currentlyAvailableTileBackgrounds.Count == 0)
        {
            EditorManager.SelectedTileBackgroundModifierIndex = 0;
            return;
        }


        EditorTileBackgroundModifier background = currentlyAvailableTileBackgrounds[modifierIndex] as EditorTileBackgroundModifier;
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(background.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = background.GetSprite();
        EditorManager.SelectedTileBackgroundModifierIndex = modifierIndex;
    }
}
