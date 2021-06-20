using System.Collections.Generic;

public class EditorTileTransformationTriggererSelector : EditorTileModifierSelector
{
    public EditorTileTransformationTriggererSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        int selectedModifierIndex = EditorManager.SelectedTileTransformationTriggererIndex;
        int newIndex = selectedModifierIndex + newValue;

        SwitchSelectedModifier(newIndex, EditorTileModifierCategory.TransformationTriggerer);
        if (newIndex >= 0 && newIndex < _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.TransformationTriggerer].Count)
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        List<IEditorTileModifier> currentlyAvailableTileTransformationTriggerers = _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.TransformationTriggerer];

        if (currentlyAvailableTileTransformationTriggerers.Count == 0)
        {
            EditorManager.SelectedTileTransformationTriggererIndex = 0;
            return;
        }

        EditorTileTransformationModifier transformationTrigger = currentlyAvailableTileTransformationTriggerers[modifierIndex] as EditorTileTransformationModifier;
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(transformationTrigger.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = transformationTrigger.GetSprite();
        EditorManager.SelectedTileTransformationTriggererIndex = modifierIndex;

        transformationTrigger.InstantiateModifierActions();
    }
}
