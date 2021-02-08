using UnityEngine;

public class EditorMazeTileTransformationTriggererSelector : EditorMazeTileModifierSelector
{
    public EditorMazeTileTransformationTriggererSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        IEditorMazeTileTransformationTriggerer currentMazeTileModifier = _editorSelectedModifierContainer.EditorMazeTileTransformationTriggerers[EditorManager.SelectedMazeTileTransformationTriggererIndex];

        int selectedBackgroundIndex = EditorManager.SelectedMazeTileBackgroundModifierIndex;
        int newIndex = selectedBackgroundIndex + newValue;

        if (newIndex < 0)
        {
            // switch from backgrounds to last TileAttribute, because TileAttribute come before TransformTriggerer
            EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(EditorMazeTileModifierType.Attribute);
            EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifier(EditorSelectedTileModifierContainer.Instance.EditorMazeTileAttributes.Count - 1); //  There is only one type of TransformationTriggerer

            currentMazeTileModifier.DestroyModifierActions();
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorMazeTileBackgrounds.Count)
        {
            // switch from backgrounds to first TileBackground, because Backgrounds come after TransformTriggerer
            EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(EditorMazeTileModifierType.Background);
            EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifier(0);

            currentMazeTileModifier.DestroyModifierActions();
        }
        else
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        IEditorMazeTileTransformationTriggerer transformationTrigger = _editorSelectedModifierContainer.EditorMazeTileTransformationTriggerers[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(transformationTrigger.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = transformationTrigger.GetSprite();
        EditorManager.SelectedMazeTileTransformationTriggererIndex = modifierIndex;

        transformationTrigger.InstantiateModifierActions();
    }
}
