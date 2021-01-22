using UnityEngine;

public class EditorMazeTileTransformationTriggererSelector : EditorMazeTileModifierSelector
{
    public EditorMazeTileTransformationTriggererSelector(EditorSelectedModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        int selectedBackgroundIndex = EditorManager.SelectedMazeTileBackgroundModifierIndex;
        int newIndex = selectedBackgroundIndex + newValue;

        if (newIndex < 0)
        {
            // switch from backgrounds to last TileAttribute, because TileAttribute come before TransformTriggerer
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifierType(EditorMazeTileModifierType.Attribute);
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifier(EditorSelectedModifierContainer.Instance.EditorMazeTileAttributes.Count - 1); //  There is only one type of TransformationTriggerer
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorMazeTileBackgrounds.Count)
        {
            // switch from backgrounds to first TileBackground, because Backgrounds come after TransformTriggerer
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifierType(EditorMazeTileModifierType.Background);
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifier(0);
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
    }

}
