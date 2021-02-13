using UnityEngine;

public class EditorTileTransformationTriggererSelector : EditorTileModifierSelector
{
    public EditorMazeTileTransformationTriggererSelector(EditorSelectedMazeTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        //IEditorMazeTileTransformationTriggerer currentMazeTileModifier = _editorSelectedModifierContainer.EditorMazeTileTransformationTriggerers[EditorManager.SelectedMazeTileTransformationTriggererIndex];

        int selectedBackgroundIndex = EditorManager.SelectedMazeTileBackgroundModifierIndex;
        int newIndex = selectedBackgroundIndex + newValue;

        if (newIndex < 0)
        {
            // switch from backgrounds to last TileAttribute, because TileAttribute come before TransformTriggerer
            EditorSelectedMazeTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(EditorTileModifierType.Attribute);
            EditorSelectedMazeTileModifierContainer.Instance.SetSelectedMazeTileModifier(EditorSelectedMazeTileModifierContainer.Instance.EditorMazeTileAttributes.Count - 1); //  There is only one type of TransformationTriggerer

            EditorMazeTileModificationPanel.Instance.DestroyModifierActions();
            //currentMazeTileModifier.DestroyModifierActions();
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorMazeTileBackgrounds.Count)
        {
            // switch from backgrounds to first TileBackground, because Backgrounds come after TransformTriggerer
            EditorSelectedMazeTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(EditorTileModifierType.Background);
            EditorSelectedMazeTileModifierContainer.Instance.SetSelectedMazeTileModifier(0);

            EditorMazeTileModificationPanel.Instance.DestroyModifierActions();
            //currentMazeTileModifier.DestroyModifierActions();
        }
        else
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        IEditorTileTransformationTriggerer transformationTrigger = _editorSelectedModifierContainer.EditorMazeTileTransformationTriggerers[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(transformationTrigger.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = transformationTrigger.GetSprite();
        EditorManager.SelectedMazeTileTransformationTriggererIndex = modifierIndex;

        transformationTrigger.InstantiateModifierActions();
    }
}
