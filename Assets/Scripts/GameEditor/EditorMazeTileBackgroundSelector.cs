public class EditorMazeTileBackgroundSelector : EditorMazeTileModifierSelector
{
    public EditorMazeTileBackgroundSelector(EditorSelectedModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        int selectedBackgroundIndex = EditorManager.SelectedMazeTileBackgroundModifierIndex;
        int newIndex = selectedBackgroundIndex + newValue;

        if (newIndex < 0)
            newIndex = _editorSelectedModifierContainer.EditorMazeTileBackgrounds.Count - 1;
        if (newIndex >= _editorSelectedModifierContainer.EditorMazeTileBackgrounds.Count)
            newIndex = 0;

        SetSelectedModifier(newIndex);
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        IEditorMazeTileBackground background = _editorSelectedModifierContainer.EditorMazeTileBackgrounds[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(background.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = background.Sprite;
        EditorManager.SelectedMazeTileBackgroundModifierIndex = modifierIndex;
    }
}
