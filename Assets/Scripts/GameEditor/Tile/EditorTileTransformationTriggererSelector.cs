using System.Collections.Generic;

public class EditorTileTransformationTriggererSelector : EditorTileModifierSelector
{
    public EditorTileTransformationTriggererSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        EditorTileModifierCategory currentCategory = EditorTileModifierCategory.TransformationTriggerer;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        int selectedBackgroundIndex = EditorManager.SelectedTileBackgroundModifierIndex;
        int newIndex = selectedBackgroundIndex + newValue;

        if (newIndex < 0)
        {
            EditorTileModifierCategory previousEditorTileModifierCategory = PreviousEditorTileModfierCategory(currentCategory);
            List<EditorTileModifier> registeredModifiers = selectedTileModifierContainer.ModifiersByCategories[previousEditorTileModifierCategory];

            selectedTileModifierContainer.SetSelectedMazeTileModifierCategory(previousEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedMazeTileModifier(registeredModifiers.Count - 1);

            EditorMazeTileModificationPanel.Instance.DestroyModifierActions();
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorTileBackgrounds.Count)
        {
            EditorTileModifierCategory nextEditorTileModifierCategory = NextEditorTileModfierCategory(currentCategory);

            selectedTileModifierContainer.SetSelectedMazeTileModifierCategory(nextEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedMazeTileModifier(0);

            EditorMazeTileModificationPanel.Instance.DestroyModifierActions();
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
