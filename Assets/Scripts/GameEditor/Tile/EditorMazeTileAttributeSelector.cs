public class EditorMazeTileAttributeSelector : EditorMazeTileModifierSelector
{
    public EditorMazeTileAttributeSelector(EditorSelectedModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        int selectedAttributeIndex = EditorManager.SelectedMazeTileAttributeModifierIndex;
        int newIndex = selectedAttributeIndex + newValue;

        if (newIndex < 0)
        {
            // switch from attributes to last Background, because Backgrounds come before TileAttributes
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifierType(EditorMazeTileModifierType.Background);
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifier(EditorSelectedModifierContainer.Instance.EditorMazeTileBackgrounds.Count - 1);
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorMazeTileAttributes.Count)
        {
            // switch from attributes to TransformTriggerer, because Transform Triggerer come after TileAttributes
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifierType(EditorMazeTileModifierType.TransformationTriggerer);
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifier(0);
        }
        else
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        IEditorMazeTileAttribute attribute = _editorSelectedModifierContainer.EditorMazeTileAttributes[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(attribute.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = attribute.GetSprite();
        EditorManager.SelectedMazeTileAttributeModifierIndex = modifierIndex;
    }
}
