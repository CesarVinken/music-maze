public class EditorMazeTileAttributeSelector : EditorMazeTileModifierSelector
{
    public EditorMazeTileAttributeSelector(EditorSelectedModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        int selectedAttributeIndex = EditorManager.SelectedMazeTileAttributeModifierIndex;
        int newIndex = selectedAttributeIndex + newValue;

        if (newIndex < 0)
            newIndex = _editorSelectedModifierContainer.EditorMazeTileAttributes.Count - 1;
        if (newIndex >= _editorSelectedModifierContainer.EditorMazeTileAttributes.Count)
            newIndex = 0;

        IEditorMazeTileAttribute attribute = _editorSelectedModifierContainer.EditorMazeTileAttributes[newIndex];
        SetSelectedModifier(newIndex);
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        IEditorMazeTileAttribute attribute = _editorSelectedModifierContainer.EditorMazeTileAttributes[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(attribute.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = attribute.GetSprite();
        EditorManager.SelectedMazeTileAttributeModifierIndex = modifierIndex;
    }
}
